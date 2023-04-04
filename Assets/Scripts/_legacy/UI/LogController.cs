using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogController : MonoBehaviour
{
    // Components
    public RectTransform DisplayParent;

    // Message Type Definitions
    public enum MessageType
    {
        generic,
        flavor
    }
    [System.Serializable]
    public struct FormatPair
    {
        public MessageType type;
        public GameObject prefab;
    }
    public FormatPair[] formatTable;
    GameObject getFormatPrefab(MessageType format)
    {
        foreach(FormatPair v in formatTable)
        {
            if (v.type == format) return v.prefab;
        }
        return null;
    }
    
    public float messageDuration = 30;
    public float messageFadeDuration = 0.5f;

    // Message Queue
    void QueueMessage(LogMessageController message)
    {
        Vector3 offset = Vector3.up * message.GetHeight();
        for(int n = 0; n < DisplayParent.childCount; n++)
        {
            DisplayParent.GetChild(n).transform.localPosition += offset;
        }
        message.transform.SetParent(DisplayParent, false);
    }

    /// <summary>
    /// Add a message to the log with the specified text and format
    /// </summary>
    /// <param name="text"></param>
    /// <param name="format"></param>
    public void AddMessage(string text, MessageType format)
    {
        // create the proper prefab for the format
        GameObject prefab = getFormatPrefab(format);
        if (prefab == null) throw new System.Exception($"missing MessageType prefab for {prefab}");
        GameObject newMessage = Instantiate(prefab);

        // Register and setup the messageController
        LogMessageController messageController = newMessage.GetComponent<LogMessageController>();
        messageController.SetText(text);
        messageController.FadeOut(messageFadeDuration, messageDuration);
        QueueMessage(messageController);
    }
}

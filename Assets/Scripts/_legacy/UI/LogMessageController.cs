using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogMessageController : MonoBehaviour
{
    // Components
    TextMeshProUGUI textBox;

    public float creationTime;

    // Public Methods
    public float GetHeight()
    {
        return textBox.preferredHeight;
    }
    public void SetText(string text)
    {
        textBox.text = text;
    }

    private void Awake()
    {
        textBox = GetComponent<TextMeshProUGUI>();
        creationTime = Time.time;
    }

    public void FadeOut(float duration, float delay = 0)
    {
        StartCoroutine(FadeOutAsync(duration, delay));
    }

    IEnumerator FadeOutAsync(float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        float startTime = Time.time;
        Color color = textBox.color;
        float initialAlpha = color.a;

        do
        {
            // fade color this frame
            color.a = Mathf.Lerp(initialAlpha, 0, (Time.time - startTime) / duration);
            textBox.color = color;

            // wait for next frame
            yield return null;
        }
        while (Time.time < startTime + duration);

        // when it's faded completely delete the message
        Destroy(gameObject);
    }
}

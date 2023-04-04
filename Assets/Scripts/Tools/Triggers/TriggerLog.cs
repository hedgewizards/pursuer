using HUD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class TriggerLog : Trigger
    {
        [Tooltip("time in seconds before it triggers again. if zero, no cooldown. if negative, only trigger once")]
        public float cooldown = 0;
        float nextTrigger = 0;

        public string logMessage;
        public LogController.MessageType messageType;


        public override void OnPlayerEnter(GameObject other)
        {
            base.OnPlayerEnter(other);

            if (nextTrigger != -1 && Time.time > nextTrigger)
            {
                // set next trigger time, or set it to -1 if it should only trigger once
                nextTrigger = (cooldown < 0) ? (-1) : (Time.time + cooldown);

                // send the message to the log
                HUDManager.LogMessage(logMessage, messageType);
            }
        }
    }
}

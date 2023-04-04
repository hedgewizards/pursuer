using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entity.Being
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class BeingDebugDisplay : MonoBehaviour
    {
        public bool DisplayStats;
        public bool DisplayActionInformation;

        public BeingActor Target;

        TextMeshProUGUI text;
        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            text.text = BuildMessage();
        }

        private string BuildMessage()
        {
            string text = "";
            if (DisplayStats)
            {
                text += getStatsInformation();
            }
            if (DisplayActionInformation)
            {
                text += getActionInformation();
            }

            return text;
        }

        private string getStatsInformation()
        {
            string text = $"HP: {Target.HealthTank.Health}\n";
            text += $"SPD: {Target.Table.SpeedStat.Value}\n";

            return text;
        }

        private string getActionInformation()
        {
            string text = $"Current Action: {Target.Table.CurrentAction?.Name ?? "None"}\n";
            return text;
        }
    }
}

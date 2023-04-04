using Entity.Dancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HUD
{
    public class HUDManager : MonoBehaviour
    {
        // Components
        public static HUDManager self;
        public LogController logController;
        public CrosshairController crosshairController;
        public GameObject WeaponHUDRoot;

        // Messages
        private void Awake()
        {
            self = this;
        }

        public static void SetCrosshairCone(float angle)
        {
            self.crosshairController.SetCone(angle);
        }

        // Global Functions
        public static void LogMessage(string text, LogController.MessageType type = LogController.MessageType.generic)
        {
            self.logController.AddMessage(text, type);
        }
    }
}
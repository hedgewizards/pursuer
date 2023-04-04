using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Entity.Being.Stances.Modifiers
{
    public class LerpModdableValueModifier : BeingStanceModifier
    {
        public string ModdableValueName;

        float initialTime;
        float initialValue;
        public float FinalValue;
        public float LerpDuration;

        public override void Initialize(BeingActor self, BeingStance modifiedStance)
        {
            base.Initialize(self, modifiedStance);
            modifiedStance.OnEnter += startLerp;
            modifiedStance.OnExit += stopLerp;
        }

        private void startLerp(object sender, EventArgs e)
        {
            initialTime = Time.time;
            initialValue = modifiedStance.CheckModdableValue(ModdableValueName);
            self.OnThink += updateLerp;
        }

        private void updateLerp(object sender, EventArgs e)
        {
            float t = (Time.time - initialTime) / LerpDuration;
            float newValue = Mathf.Lerp(initialValue, FinalValue, t);
            modifiedStance.SetModdableValue(ModdableValueName, newValue);

            if (t > 1)
            {
                self.OnThink -= updateLerp;
            }
        }

        private void stopLerp(object sender, EventArgs e)
        {
            self.OnThink -= updateLerp;
            modifiedStance.SetModdableValue(ModdableValueName, initialValue);
        }
    }
}

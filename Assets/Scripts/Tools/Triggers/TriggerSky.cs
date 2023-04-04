using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class TriggerSky : Trigger
    {
        [Tooltip("time in seconds before it triggers again. if zero, no cooldown. if negative, only trigger once")]
        public float cooldown = 0;
        float nextTrigger = 0;

        public float TransitionDuration;

        public Color SkyColor;

        public Color FogColor;
        public float FogStart;
        public float FogEnd;

        public override void OnPlayerEnter(GameObject other)
        {
            base.OnPlayerEnter(other);

            if (nextTrigger != -1 && Time.time > nextTrigger)
            {
                // set next trigger time, or set it to -1 if it should only trigger once
                nextTrigger = (cooldown < 0) ? (-1) : (Time.time + cooldown);

                // send the message to the log
                AtmosphereManager.SetFog(FogColor, FogStart, FogEnd, TransitionDuration);
                AtmosphereManager.SetSky(SkyColor, TransitionDuration);
            }
        }
    }
}

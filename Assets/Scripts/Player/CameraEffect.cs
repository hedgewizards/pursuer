using System;
using UnityEngine;

namespace Player
{
    public struct CameraEffect
    {
        public enum Type
        {
            Punch
        }
        Vector2 maxPunch;
        public float StartTime;
        public const float Duration = 0.3f;

        public CameraEffect(Vector2 maxPunch, float startTime)
        {
            this.maxPunch = maxPunch;
            this.StartTime = startTime;
        }

        public Vector2 Calculate()
        {
            return maxPunch * calculateScalar();
        }

        float calculateScalar()
        {
            float t = (Time.time - StartTime) / Duration;
            float tr = (4*t - 1)*Mathf.PI;
            return Mathf.Sin(tr) * (1 / tr) * (1.25f - t);
        }
    }
}

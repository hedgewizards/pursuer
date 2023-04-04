using UnityEngine;

namespace Generation
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Light))]
    public class LightShadowCuller : MonoBehaviour
    {
        Light light;
        public LightShadows UnculledSetting = LightShadows.Hard;

        public float CullShadowsRadius = 25;

        public float RefreshesPerMinute = 150;

#if UNITY_EDITOR
        public bool TestInEditor = false;
#endif

        private void Awake()
        {
            light = GetComponent<Light>();

#if UNITY_EDITOR
            if (Application.isPlaying || TestInEditor)
            {
#endif
                float refreshDelay = 60 / RefreshesPerMinute;
                InvokeRepeating("RefreshCulling", Random.value * refreshDelay, refreshDelay);
#if UNITY_EDITOR
            }
            else
            {
                light.shadows = LightShadows.None;
            }
#endif
        }

        public void RefreshCulling()
        {
            float sqrDistance = Vector3.SqrMagnitude(Camera.main.transform.position - transform.position);
            light.shadows = sqrDistance > CullShadowsRadius * CullShadowsRadius ? LightShadows.None : UnculledSetting;
        }
    }
}
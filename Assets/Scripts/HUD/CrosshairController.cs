using UnityEngine;

namespace HUD
{
    public class CrosshairController : MonoBehaviour
    {
        public void SetCone(float angle)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
#if UNITY_EDITOR
            float pixelOffset = (Screen.height / 2f) * Mathf.Tan(Mathf.Deg2Rad * angle) / Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2);
#else
            float pixelOffset = (480 / 2f) * Mathf.Tan(Mathf.Deg2Rad * angle) / Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2);
#endif

            rectTransform.sizeDelta = Vector2.one * pixelOffset * 2;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SilenceEffectUI : MonoBehaviour
{
    RectTransform rect;
    Image silenceBar;
    public float duration;
    float endTime;
    float startTime;

    private void Start()
    {
        startTime = Time.time;
        endTime = startTime + duration;
        rect = GetComponent<RectTransform>();
        silenceBar = GetComponent<Image>();
    }

    private void Update()
    {
        float frac = Mathf.Lerp(1, 0, (Time.time - startTime) / duration);
        silenceBar.fillAmount = frac;
        rect.rotation = Quaternion.Euler(0, 0, -180 * frac);

        if(Time.time > endTime)
        {
            Destroy(gameObject);
        }
    }
}

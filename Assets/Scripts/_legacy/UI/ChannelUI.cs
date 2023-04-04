using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelUI : MonoBehaviour
{
    RectTransform rect;
    Image silenceBar;
    public float duration;
    float startTime;
    bool doRender;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        silenceBar = GetComponent<Image>();
        Stop();
    }

    public void Play(float _duration)
    {
        duration = _duration;
        startTime = Time.time;
        gameObject.SetActive(true);
    }
    public void Stop()
    {
        silenceBar.fillAmount = 0;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        float frac = Mathf.Lerp(0, 1, (Time.time - startTime) / duration);
        silenceBar.fillAmount = frac;
        rect.rotation = Quaternion.Euler(0, 0, -180 * frac);
    }
}

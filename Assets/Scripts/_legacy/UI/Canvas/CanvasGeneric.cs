using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGeneric : MonoBehaviour
{
    Graphic[] graphics;
    public static float ANIMATE_TIME = 0.3f;
    public static float MAX_ALPHA = 0.8f;
    float DRAW_TIME;
    float fadeTime;
    float killTime;
    bool isDrawn;
    bool isFading;

    private void Awake()
    {
        graphics = GetComponents<Graphic>();
        foreach(Graphic g in graphics)
        {
            g.canvasRenderer.SetAlpha(0.0f);
        }
    }

    private void Update()
    {
        if (isDrawn)
        {
            if (!isFading)
            {
                if (fadeTime < Time.time)
                {
                    foreach (Graphic g in graphics)
                    {
                        g.CrossFadeAlpha(0, ANIMATE_TIME, false);
                    }
                    isFading = true;
                }
            }
            else
            {
                if (killTime < Time.time)
                {
                    Destroy(this);
                }
            }
        }
    }

    public void Draw(float DRAW_TIME = 0)
    {
        isDrawn = true;
        fadeTime = Time.time + DRAW_TIME + ANIMATE_TIME;
        killTime = fadeTime + ANIMATE_TIME;

        graphics = GetComponents<Graphic>();
        foreach (Graphic g in graphics)
        {
            g.CrossFadeAlpha(MAX_ALPHA, ANIMATE_TIME, false);
        }
    }
}

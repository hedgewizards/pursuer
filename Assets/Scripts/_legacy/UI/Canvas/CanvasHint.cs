using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHint : MonoBehaviour
{
    public static float ANIMATE_TIME = 0.3f;
    public static float MAX_ALPHA = 0.8f;
    float fadeTime; //Time.time to start fading the hint
    float killTime; //Time.time to destroy this instance
    bool isDrawn;
    bool isFading;
    Image hintImage;
    Text hintText;

    void Awake()
    {
        hintImage = transform.GetChild(0).GetComponent<Image>();
        hintText = transform.GetChild(1).GetComponent<Text>();
        hintImage.canvasRenderer.SetAlpha(0.0f);
        hintText.canvasRenderer.SetAlpha(0.0f);
    }

    void Update()
    {
        if(isDrawn)
        {
            if (!isFading)
            {
                if(fadeTime < Time.time)
                {

                    hintImage.CrossFadeAlpha(0, ANIMATE_TIME, false);
                    hintText.CrossFadeAlpha(0, ANIMATE_TIME, false);
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

    public void ShowHint(Sprite hintSprite, string hintString, float length)
    {
        isDrawn = true;
        hintImage.sprite = hintSprite;
        hintText.text = hintString;
        fadeTime = Time.time + length + ANIMATE_TIME;
        killTime = fadeTime + ANIMATE_TIME;
        hintImage.CrossFadeAlpha(MAX_ALPHA, ANIMATE_TIME, false);
        hintText.CrossFadeAlpha(MAX_ALPHA, ANIMATE_TIME, false);
    }

    public void ShowHintText(string hintString, float length)
    {
        isDrawn = true;
        hintText.text = hintString;
        fadeTime = Time.time + length + ANIMATE_TIME;
        killTime = fadeTime + ANIMATE_TIME;
        hintText.CrossFadeAlpha(MAX_ALPHA, ANIMATE_TIME, false);
    }
}

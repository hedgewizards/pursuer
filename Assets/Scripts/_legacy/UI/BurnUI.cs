using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurnUI : MonoBehaviour
{
    Image image;

    public float duration;
    public float interval;
    float endTime;
    float startTime;

    Color fullColor;
    Color emptyColor;

    private void Start()
    {
        image = GetComponent<Image>();
        fullColor = image.color;
        emptyColor = fullColor;
        emptyColor.a = 0;

        //round duration to the nicest N + 0.75 multiple of interval
        //for a nice looking fade out
        float n = Mathf.Floor(duration / interval) + 0.75f;
        duration = n * interval;
        
        startTime = Time.time;
        endTime = startTime + duration;
    }

    private void Update()
    {
        float n = (Time.time - startTime) / interval % 1;
        float t = Extensions.SharpCurve(n);
        image.color = Color.Lerp(emptyColor, fullColor, t);

        if(Time.time > endTime)
        {
            Destroy(gameObject);
        }
    }
}

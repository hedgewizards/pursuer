using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloaterUI : MonoBehaviour
{
    RectTransform rectTransform;

    float duration = 1; // how long the floater lasts (seconds)
    float fadeStartDuration = 0.8f; // how long before the floater starts to fade
    Vector3 initialPoint; // initial position of the object
    Vector3 finalPoint; // position the object will be the instant it is destroyed
    float destroyTime; // the Time.time at which the object will be destroyed
    float fadeStartTime; // the Time.time at which the object will start to fade
    bool didStartFade; // whether or not the object has already started to fade

    public void Initialize(Vector3 travelVector, string text)
    {
        //initialize variables
        rectTransform = GetComponent<RectTransform>();
        initialPoint = rectTransform.position;
        finalPoint = initialPoint + travelVector;
        destroyTime = Time.time + duration;

        //set string text
        GetComponent<Text>().text = text;

        Destroy(gameObject, destroyTime); //destroy the entire object after duration seconds

        fadeStartTime = Time.time + fadeStartDuration;
        didStartFade = false;
    }

    public void Update()
    {
        //move the object along the path
        float t = 1 - (destroyTime - Time.time) / duration;
        rectTransform.position = Vector3.Lerp(initialPoint, finalPoint, t);

        if(!didStartFade && Time.time > fadeStartTime)
        {
            GetComponent<Graphic>().CrossFadeAlpha(0, destroyTime - Time.time, true);
            didStartFade = true;
        }
    }
}

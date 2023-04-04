using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatUI : MonoBehaviour
{
    public Gradient colorOverFill;

    float heat;
    public float Value
    {
        get { return heat; }
        set
        {
            heat = Mathf.Clamp(value, 0, MAX_VALUE);
            float t = 1 - heat / MAX_VALUE;
            //set fill amounts
            bar1.fillAmount = t;
            bar2.fillAmount = t;
            //set color
            bar1.color = colorOverFill.Evaluate(t);
            bar2.color = colorOverFill.Evaluate(t);

        }
    }
    public float MAX_VALUE;
    public Image bar1;
    public Image bar2;
    public void SetMaxValue(float _MAX_VALUE)
    {
        MAX_VALUE = _MAX_VALUE;
    }
}

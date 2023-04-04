using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    float value1;
    public float Value1
    {
        get { return value1; }
        set
        {
            value1 = Mathf.Clamp(value,0,VALUE_1_MAX);
            resource1Bar.fillAmount = value1 / VALUE_1_MAX;
            resource1Text.text = ((int)value1).ToString();
        }
    }
    float value2;
    public float Value2
    {
        get { return value2; }
        set
        {
            if (!Value2Enabled)
                return;
            value2 = Mathf.Clamp(value, 0, VALUE_2_MAX);
            resource2Bar.fillAmount = value2 / VALUE_2_MAX;
            resource2Text.text = "+" + ((int)value2).ToString();
        }
    }

    bool Value2Enabled = true;

    float VALUE_1_MAX = 100;
    float VALUE_2_MAX = 100;

    public Image resource1Bar;
    public Image resource2Bar;
    public Text resource1Text;
    public Text resource2Text;

    public void SetMaxValues(float max1, float max2 = 100)
    {
        VALUE_1_MAX = max1;
        VALUE_2_MAX = max2;
    }
    public void DisableValue2()
    {
        resource2Text.text = "";
        resource2Bar.fillAmount = 1;
        Value2Enabled = false;
    }
    public void CreateFloater1(int amount)
    {
        string str;
        //create the text string
        if(amount < 0)
        {
            str = "-" + (-amount);
        }
        else
        {
            str = "+" + amount;
        }

        GameObject obj = Instantiate(resource1Text.gameObject, transform, true);
        FloaterUI f = obj.AddComponent<FloaterUI>();
        f.Initialize(new Vector3(0, 100f, 0),str);
    }
    public void CreateFloater2(int amount)
    {
        string str;
        //create the text string
        if (amount < 0)
        {
            str = "-" + (-amount);
        }
        else
        {
            str = "+" + amount;
        }

        GameObject obj = Instantiate(resource2Text.gameObject, transform, true);
        FloaterUI f = obj.AddComponent<FloaterUI>();
        f.Initialize(new Vector3(0, 100f, 0), str);
    }
}

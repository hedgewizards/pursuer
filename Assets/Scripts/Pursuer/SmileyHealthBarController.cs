using Combat;
using Pursuer;
using System;
using UnityEngine;

public class SmileyHealthBarController : MonoBehaviour
{
    public HealthTank Target;
    public GameObject OuterHealthBar;
    public GameObject InnerHealthBar;
    RectTransform innerBarTransform;

    float fullWidth;

    private void Start()
    {
        innerBarTransform = InnerHealthBar.GetComponent<RectTransform>();
        fullWidth = innerBarTransform.sizeDelta.x;
        PursuerGameManager.Instance.OnSkullPickedUp += onSkullPickedUp;
    }

    private void onSkullPickedUp(object sender, int numberOfSkulls)
    {
        if (numberOfSkulls == 5)
        {
            OuterHealthBar.SetActive(true);
        }
    }

    void Update()
    {
        innerBarTransform.sizeDelta = new Vector2(fullWidth * (Target.Health / Target.MaxHealth), innerBarTransform.sizeDelta.y); 
    }
}

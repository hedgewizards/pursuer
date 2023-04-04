using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectController : MonoBehaviour
{
    public EffectReference[] References;

    public void PerformEffect(string EffectName)
    {
        foreach(EffectReference r in References)
        {
            if(EffectName == r.ReferenceName)
            {
                r.tEffectBundle.PerformEffect();
            }
        }
    }
}

public class EffectReference
{
    public string ReferenceName;
    public EffectBundle tEffectBundle;
}
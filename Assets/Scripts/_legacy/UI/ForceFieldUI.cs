using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldUI : MonoBehaviour
{
    public float duration;
    private void Start()
    {
        Destroy(gameObject, duration);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashController : MonoBehaviour
{

    public float effectDuration = 0.1f;
    new ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();    
    }
    
    public void PerformEffect(Vector3 startPosition)
    {
        transform.position = startPosition;
        particleSystem.Play();
    }


}

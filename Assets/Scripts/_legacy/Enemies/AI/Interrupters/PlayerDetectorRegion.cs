using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerDetectorRegion : AIInterrupter
{
    public override bool ShouldInterrupt()
    {
        return playerDetected;
    }
    bool playerDetected;

    private void OnTriggerEnter(Collider other)
    {
        playerDetected = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerDetected = false;
    }
}

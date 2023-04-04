using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectorRadius : AIInterrupter
{
    public float radius;
    public bool ShouldQueryLOS;

    public override bool ShouldInterrupt()
    {
        Vector3 PlayerPos = PlayerController.Instance.transform.position;
        float dist = (PlayerPos - transform.position).magnitude;
        bool inRange = dist < radius;

        return ShouldQueryLOS ? QueryLOS(PlayerPos, dist) : inRange;
    }

    public bool QueryLOS(Vector3 target, float distance)
    {
        return !Physics.Raycast(transform.position, target - transform.position, distance, LAYERMASK.SOLIDS_ONLY);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

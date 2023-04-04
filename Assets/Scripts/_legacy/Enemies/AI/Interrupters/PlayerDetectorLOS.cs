using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class PlayerDetectorLOS : AIInterrupter
{
    public float radius;
    public bool ShouldQueryLOS;

    public override bool ShouldInterrupt()
    {
        // Check if the player is within radius
        GameObject player = PlayerController.Instance.gameObject;

        if (player && (transform.position - player.transform.position).sqrMagnitude < radius * radius)
        {
            if (!ShouldQueryLOS) return true;

            EnemyController self = GetComponentInParent<EnemyController>();
            return self.QueryLOS(player.GetComponent<CapsuleCollider>().bounds.center, false);
        }

        return false;
    }
}

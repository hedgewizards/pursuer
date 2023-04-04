using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wait for the player to get into range, then attack
/// </summary>
public class AIStateReady : AIState
{
    GameObject Target;

    public bool playEnGuard = true;

    public override AIStateData Think(AIStateData data)
    {
        self.RotateTowards(Target.transform.position);

        return base.Think(data);
    }

    public override AIStateData OnEnter(AIStateData data)
    {
        data = base.OnEnter(data);
        self.animator.SetInteger("stance", 1);
        if (playEnGuard)
        {
            self.animator.SetTrigger("enguard");
            playEnGuard = false;
        }

        Target = PlayerController.Instance.gameObject;

        return data;
    }
}

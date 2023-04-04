using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamemode.AI;

[RequireComponent(typeof(EnemyController))]
public abstract class EnemyAttack : MonoBehaviour
{
    //Components
    protected EnemyController enemyController;

    [Tooltip("this attack isn't performed until the player is within this range (and passes an LOS check")]
    public float triggerRange;
    [Tooltip("time before the enemy should return to idle state after performing attack")]
    public float duration;
    [Tooltip("after the attack is successfully performed, how long until the attack will be tried again")]
    public float cooldown;
    [Tooltip("The state the AI is forced into during the attack")]
    public AIState forcedState;
    

    [Tooltip("if name isn't blank, flip this animation trigger at the start of the attack")]
    public string AnimationTrigger;
    [Tooltip("if supplied, this sound will play at the start of the attack")]
    public AudioClip AttackSound;

    [Tooltip("While the attack is being performed, modify the enemy's movement speed by this fraction")]
    public float SpeedModifier = 1;
    
    protected GameObject target;
    AIState cachedState;
    float cooldownEndTime = 0;
    protected float attackStartTime = 0;

    protected virtual void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    public virtual bool CanPerform(GameObject target)
    {
        return Time.time > cooldownEndTime;
    }
    public virtual void StartAttack(GameObject target)
    {
        attackStartTime = Time.time;
        if (AnimationTrigger != "")
            enemyController.FlipAnimationTrigger(AnimationTrigger);
        if (AttackSound != null)
            enemyController.audioSource.PlayOneShot(AttackSound);
        this.target = target;
    }

    /// <summary>
    /// Do think step of attack, return TRUE if attack should continue
    /// </summary>
    /// <returns></returns>
    public virtual bool AttackThink()
    {

        if (Time.time > attackStartTime + duration)
        {
            return false;
        }
        
        return true;
    }
    public virtual void AttackEnd()
    {
        cooldownEndTime = Time.time + cooldown;
    }

}
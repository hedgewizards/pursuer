using Combat;
using Entity.Effects;
using Inputter;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControllerOld : MonoBehaviour
{
    public enum State
    {
        Idle,
        Attacking,
        Deploying,
        Holstering
    }
    State currentState;

    float nextStateTime = 0;
    WeaponAttack activeAttack;

    public WeaponAttack Attack1;
    public WeaponAttack ReloadAttack;

    DamageInfo cachedHurtboxDamageInfo;
    public WeaponHurtbox[] Hurtboxes;
    /// <summary>
    /// Look through Hurtboxes for one whos parent GameObject is named HurtboxName
    /// </summary>
    /// <param name="HurtboxName"></param>
    /// <returns></returns>
    public WeaponHurtbox GetHurtbox(string HurtboxName)
    {
        foreach (WeaponHurtbox h in Hurtboxes)
        {
            if (h.name == HurtboxName) return h;
        }
        return null;
    }
    public void SetHurtboxDamageInfo(DamageInfo damageInfo)
    {
        cachedHurtboxDamageInfo = damageInfo;
    }
    public void SurfaceImpact(Vector3 point)
    {
        animator.SetTrigger("surfaceImpact");
        EffectDataStruct e = new EffectDataStruct
        {
            position = point,
            normal = (Owner.GetShootPos() - point)
        };
        EffectManager.CreateEffect("wallimpact", e);
    }

    public void ActivateHurtbox(string hurtboxName)
    {
        WeaponHurtbox h = GetHurtbox(hurtboxName);
        if (h != null)
        {
            h.Activate(this, cachedHurtboxDamageInfo);
        }
        else
        {
            throw new System.Exception("Could not find Hurtbox \"" + hurtboxName + "\"");
        }
    }

    public void DeactivateHurtbox(string hurtboxName)
    {
        WeaponHurtbox h = GetHurtbox(hurtboxName);
        if (h != null)
        {
            h.Deactivate();
        }
        else
        {
            throw new System.Exception("Could not find Hurtbox \"" + hurtboxName + "\"");
        }
    }

    public int ClipSize = 12;

    public float DeployTime = 0.5f;
    public float HolsterTime = 0.25f;
    public AudioClip DeploySound;
    public AudioClip HolsterSound;
    public AudioClip EmptySound;

    public int CurrentClip = 12;

    //Components
    public Animator animator;
    PlayerController Owner;
    public Transform EffectOrigin;

    public void Deploy(PlayerController Owner)
    {
        this.Owner = Owner;
        currentState = State.Deploying;
        animator.SetTrigger("deploy");
        if (DeploySound)
        {
            Owner.AUD.PlayOnce(DeploySound);
        }
        nextStateTime = Time.time + DeployTime;
    }

    public void Think()
    {
        if (currentState != State.Idle && Time.time > nextStateTime)
        {
            currentState = State.Idle;
        }

        if (currentState == State.Attacking)
        {
            activeAttack.Think(Owner);
        }

        if (currentState == State.Idle)
        {
            if (InputManager.CheckKeyDown(KeyName.Attack1)
                || (Attack1.IsAutomatic && InputManager.CheckKey(KeyName.Attack1)))
            {
                TryAttack(Attack1);
            }
            else if (InputManager.CheckKeyDown(KeyName.Reload)
                || (ReloadAttack.IsAutomatic && InputManager.CheckKey(KeyName.Reload)))
            {
                TryAttack(ReloadAttack);
            }
        }
    }

    public bool TryAttack(WeaponAttack attack)
    {
        WeaponAttack.AttackFailReason failReason = WeaponAttack.AttackFailReason.unknown;
        bool canPerform = attack.CanPerform(Owner, out failReason);

        if (canPerform)
        {
            attack.Perform(Owner);
            currentState = State.Attacking;
            activeAttack = attack;
            animator.SetTrigger(attack.animationTriggerName);
            nextStateTime = Time.time + attack.Duration;

        }
        else if (failReason == WeaponAttack.AttackFailReason.noAmmo)
        {
            Owner.AUD.PlayOnce(EmptySound);
        }

        return canPerform;
    }

}

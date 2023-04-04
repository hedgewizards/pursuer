using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackMelee : EnemyAttack
{
    public EnemyHurtboxController hurtbox;

    public float damage;
    [Tooltip("time in seconds after attack commences that the hurtbox is queried and damage is applied")]
    public DamageInfo.TYPE damageType = DamageInfo.TYPE.melee;
    public float windupTime;

    [Tooltip("When an attack is performed, these afflictions are also performed to the target")]
    public Affliction[] targetAfflictions;

    [Tooltip("Sound that plays if the melee attack connects")]
    public AudioClip HitSound;
    [Tooltip("Sound that plays if the melee attack misses")]
    public AudioClip MissSound;

    bool didQueryHurtbox;
    

    public override bool CanPerform(GameObject target)
    {
        return base.CanPerform(target) && hurtbox.PlayerIsInContact();
    }

    public override void StartAttack(GameObject target)
    {
        didQueryHurtbox = false;

        base.StartAttack(target);
    }

    public override bool AttackThink()
    {

        if (!didQueryHurtbox && Time.time > attackStartTime + windupTime)
        {
            didQueryHurtbox = true;
            QueryHurtbox();
        }

        return base.AttackThink();
    }

    void QueryHurtbox()
    {
        IDamageable player = hurtbox.GetPlayer();

        if (player != null)
        {
            DamageInfo d = new DamageInfo(damage, damageType, enemyController.transform.position, targetAfflictions);
            player.ApplyDamage(d);

            if (HitSound)
                enemyController.audioSource.PlayOneShot(HitSound);
        }
        else
        {
            if (MissSound)
                enemyController.audioSource.PlayOneShot(MissSound);
        }
    }
}

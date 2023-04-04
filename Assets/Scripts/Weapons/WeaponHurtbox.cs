using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponHurtbox : MonoBehaviour
{
    List<IDamageable> hitTargets;
    bool active = false;
    DamageInfo cachedDamageInfo;
    WeaponControllerOld cachedWeaponController;
    
    private void Awake()
    {
        hitTargets = new List<IDamageable>();
    }

    public void Activate(WeaponControllerOld weaponController, DamageInfo damageInfo)
    {
        cachedWeaponController = weaponController;
        cachedDamageInfo = damageInfo;
        hitTargets.Clear();
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!active) return;

        HitBox hitBox = other.GetComponent<HitBox>();

        if(hitBox != null)
        {
            // only hit targets we haven't hit yet
            if (!hitTargets.Contains(hitBox.Target))
            {
                hitTargets.Add(hitBox.Target);
                cachedDamageInfo.OriginPoint = transform.position;
                hitBox.ApplyDamage(cachedDamageInfo);
            }
        }
        else
        {
            // we hit a wall or something
            cachedWeaponController.SurfaceImpact(transform.position);
            Deactivate();
        }
    }
}

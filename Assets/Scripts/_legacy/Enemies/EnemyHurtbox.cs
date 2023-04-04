using Combat;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    List<IDamageable> hitTargets;
    bool active = false;
    [HideInInspector] public DamageInfo cachedDamageInfo;
    EnemyController cachedEnemyController;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        hitTargets = new List<IDamageable>();
    }

    public void Activate(EnemyController enemyController)
    {
        cachedEnemyController = enemyController;
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

        IDamageable p = other.GetComponent<PlayerController>();

        if (p != null)
        {
            // only hit targets we haven't hit yet
            if (!hitTargets.Contains(p))
            {
                hitTargets.Add(p);
                cachedDamageInfo.OriginPoint = transform.position;
                p.ApplyDamage(cachedDamageInfo);
            }
        }
    }
}

using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySegmentController : MonoBehaviour, IDamageable
{
    public bool isHead;

    Animator animator;
    public Material DamagedMaterial;
    
    public GameObject[] Hitboxes;


    public EnemyController enemyController;
    public EnemySegmentController nextSegment;
    public float preferredDistance;
    public float animationOffset;

    bool wasHit = false;
    
    void LateUpdate()
    {
        if (isHead && nextSegment != null)
        {
            nextSegment.MoveTowards(transform.position);
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 newDirection = (target - lastPosition).normalized;
        transform.position = target - newDirection * preferredDistance;
        transform.rotation = Quaternion.LookRotation(target - transform.position, Vector3.up);
        
        animator.SetBool("moving", ((lastPosition - transform.position).sqrMagnitude > 0.00001));

        lastPosition = transform.position;

        if (nextSegment != null)
        {
            nextSegment.MoveTowards(transform.position);
        }
    }

    void IDamageable.ApplyDamage(DamageInfo damageInfo)
    {
        if (!wasHit)
        {
            damageInfo.hitBoxType = HitBox.HitBoxType.critical;
            SetDamaged();
            wasHit = true;
        }
        ((IDamageable)enemyController).ApplyDamage(damageInfo);
    }

    Vector3 lastPosition;
    private void Awake()
    {
        wasHit = false;
        lastPosition = transform.position;
        animator = GetComponent<Animator>();
        animator.SetFloat("cycleOffset", animationOffset);


        foreach (GameObject g in Hitboxes)
        {
            g.GetComponent<HitBox>().Target = this;
        }
    }

    void SetDamaged()
    {
        animator.SetTrigger("damaged");
        GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial = DamagedMaterial;
    }
}

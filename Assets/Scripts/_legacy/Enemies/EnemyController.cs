using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Combat;

public class EnemyController : MonoBehaviour, IDamageable
{
    //Components
    public Animator animator;
    Animator effectAnimator;
    public AudioSource audioSource;
    CharacterController characterController;
    EnemyEffectController enemyEffectController;
    [Tooltip("LOS checks performed from this point")]public Transform eye;
    AIBrain brain;


    void Awake()
    {
        // Initialize Components
        characterController = GetComponent<CharacterController>();
        enemyEffectController = GetComponent<EnemyEffectController>();
        effectAnimator = GetComponent<Animator>();
        if (eye == null) eye = transform;

        brain = GetComponent<AIBrain>();

        foreach (GameObject g in Hitboxes)
        {
            g.GetComponent<HitBox>().Target = this;
        }
    
        // Set values
        health = MaxHealth;
    }
    private void Update()
    {
        //animator.SetFloat("speed", naviAgent.velocity.sqrMagnitude);
        //animator.SetBool("airborne", naviAgent.isAirborne);
    }

    // HP / Damage
    public GameObject[] Hitboxes;
    public float MaxHealth = 100;

    public float health;
    void IDamageable.ApplyDamage(DamageInfo damageInfo)
    {
        float damage = damageInfo.Damage * CalculateDamageMultiplier(damageInfo.hitBoxType);
        health -= damage;

        if (health <= 0)
        {
            DoTakeFatalDamage();
        }
        else
        {
            DoTakeNonFatalDamage();
        }
    }

    float CalculateDamageMultiplier(HitBox.HitBoxType hitboxtype)
    {
        switch (hitboxtype)
        {
            case HitBox.HitBoxType.critical:
                return 2f;
            case HitBox.HitBoxType.leg:
                return 0.75f;
            default:
                return 1;
        }
    }
    void DoTakeNonFatalDamage()
    {
        audioSource.PlayOneShot(hurtSound);
    }
    void SetHitboxesActive(bool value)
    {
        foreach (GameObject g in Hitboxes)
        {
            g.SetActive(value);
        }
    }

    // Death
    void DoTakeFatalDamage()
    {
        audioSource.PlayOneShot(deathSound);
        animator.SetTrigger("die");
        SetHitboxesActive(false);
        StartCoroutine(CleanupCorpse(5));
        brain.EnableFlag(AIStateData.FLAG_DEAD);
    }
    static Vector3 corpseDespawnTransformation = Vector3.down * 2;
    const float corpseDespawnDuration = 2;
    IEnumerator CleanupCorpse(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 initialPos = transform.position;
        Vector3 finalPos = initialPos + corpseDespawnTransformation;
        float endTime = Time.time + corpseDespawnDuration;
        while (Time.time < endTime)
        {
            transform.position = Vector3.Lerp(initialPos, finalPos, 1 - (endTime - Time.time) / corpseDespawnDuration);

            yield return 0;
        }

        Destroy(gameObject);
    }

    // Movement
    float speed;
    public void SetMovementSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public float rotationSpeed = 180;

    // Control
    /// <summary>
    /// when called every frame, rotate towards target point (yaw only)
    /// </summary>
    /// <param name="target"></param>
    public void RotateTowards(Vector3 target)
    {
        Vector3 targetDir = (target - transform.position).Flatten().normalized;
        Vector3 currentDir = transform.forward.Flatten().normalized;

        float maxAngle = rotationSpeed * Time.deltaTime;
        float currentAngle = Vector3.Angle(targetDir, currentDir);
        if (currentAngle < maxAngle)
        {
            transform.rotation = Quaternion.LookRotation(targetDir, Vector3.up);
        }
        else
        {
            Vector3 newForward = Vector3.Slerp(currentDir, targetDir, maxAngle / currentAngle);
            transform.rotation = Quaternion.LookRotation(newForward, Vector3.up);
        }
    }
    /// <summary>
    /// Instantly rotate towards target point (yaw only)
    /// </summary>
    /// <param name="point"></param>
    public void FacePoint(Vector3 point)
    {
        transform.rotation = Quaternion.LookRotation((point - transform.position).Flatten(), Vector3.up);
    }



    /// <summary>
    /// Check if the enemy can see a specified point
    /// </summary>
    /// <param name="target"></param>
    /// <param name="ignoreCone">if true, ignore the enemy's vision cone</param>
    /// <returns>true if visible</returns>
    public bool QueryLOS(Vector3 target, bool ignoreCone)
    {
        float dist = (target - eye.position).magnitude;
        return (ignoreCone || QueryVisionCone(target))
            && !Physics.Raycast(eye.position, target - eye.position, dist, LAYERMASK.SOLIDS_ONLY);
    }
    public bool QueryVisionCone(Vector3 target)
    {
        return Vector3.Angle(eye.forward.Flatten(), (target - eye.position).Flatten()) < 80;
    }

    //Effects
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public void FlipAnimationTrigger(string name)
    {
        animator.SetTrigger(name);
        if(effectAnimator)
        {
            effectAnimator.SetTrigger(name);
        }
    }
}

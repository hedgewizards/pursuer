using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect
{
    private float expireTime;
    private bool isApplied;
    protected PlayerController player;
    
    //constructors
    public StatusEffect()
    {
        expireTime = Time.time;
        isApplied = false;
    }
    public StatusEffect(PlayerController target,float duration)
    {
        expireTime = Time.time + duration;
        isApplied = false;
        player = target;
    }

    //get/set
    public float GetExpireTime()
    {
        return expireTime;
    }

    //public functions
    public void DoApplyEffect()
    {
        if (isApplied)
        {
            //effect already applied
            return;
        }
        OnApplyEffect();
        isApplied = true;
    }
    public void DoRemoveEffect()
    {
        if(!isApplied)
        {
            //effect already removed
            return;
        }
        OnRemoveEffect();
        isApplied = false;
    }
    public void Update()
    {
        if(!isApplied)
        {
            //effect not applied, exit
            return;
        }
        OnUpdate();
    }
    /// <summary>
    /// do any interactions between the two effects, returning true if you should cancel effect
    /// </summary>
    /// <param name="eff">the status effect to compare</param>
    /// <returns>true if the effect is redundant and should be discarded</returns>
    public bool DoInteract(StatusEffect eff)
    {
        if(!isApplied)
        {
            //if the effect isn't active, it's been lazily deleted
            Debug.Log("not activated");
            return false;
        }

        if (!ReferenceEquals(eff, this))
        {
            /* this might be bad practice idk.
            * the above checks if the final type of the two objects are the same
            * so if they are both StatusEffectSlow, it will return true
            */
            //if these aren't the same effect, they don't interact

            Debug.Log("different effect");
            return false;
        }
        Debug.Log("same effect");

        //do any interactions between the two objects
        return OnInteract(eff);
    }

    //Hooks
    protected virtual void OnApplyEffect() { }
    protected virtual void OnRemoveEffect() { }
    protected virtual void OnUpdate() { }
    /// <summary>
    /// do any interactions between the two effects
    /// </summary>
    /// <param name="eff">the status effect to compare</param>
    /// <returns>true if the effect is redundant and should be discarded</returns>
    protected abstract bool OnInteract(StatusEffect eff);
}

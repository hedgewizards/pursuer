using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour
{
    public string devInfo;
    /// <summary>
    /// If true, an AIInterrupter can take control from this state
    /// </summary>
    public bool CanBeInterrupted = true;
    /// <summary>
    /// How likely the brain is to pick this state vs other states
    /// </summary>
    public int BrainPickWeight = 1;

    /// <summary>
    /// If nonzero, after this many seconds return to no state
    /// </summary>
    public float Duration = 0;

    /// <summary>
    /// If not null, go to this state when we exit the current one
    /// </summary>
    [Tooltip("if not null, go to this state next")] public AIState nextState;

    protected EnemyController self;

    private void Awake()
    {
        self = GetComponent<EnemyController>();
    }

    /// <summary>
    /// Called every frame this AIState is in use for a particular enemyController
    /// </summary>
    /// <param name="self"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual AIStateData Think( AIStateData data)
    {

        if ( data.CheckFlag(AIStateData.FLAG_NEXT) || (Duration != 0 && data.lastStateChangeTime + Duration < Time.time))
        {
            data.UpdateState(nextState);
        }

        return data;
    }

    /// <summary>
    /// Called at the end of the frame when this state is entered
    /// </summary>
    /// <param name="self"></param>
    /// <param name="data"></param>
    public virtual AIStateData OnEnter(AIStateData data)
    {
        // reset transient flags
        data.flags = data.flags & ~AIStateData.FLAGS_TRANSIENT;

        return data;
    }

    /// <summary>
    /// Called at the end of the frame when this state is exited (before OnEnter)
    /// </summary>
    /// <param name="self"></param>
    /// <param name="data"></param>
    public virtual AIStateData OnExit(AIStateData data)
    {
        return data;
    }

    public virtual bool CheckInterruptCondition()
    {
        return false;
    }
}

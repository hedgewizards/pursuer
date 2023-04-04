using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIInterrupter : MonoBehaviour
{
    /// <summary>
    /// State to enter if successfully interrupted
    /// </summary>
    public AIState nextState;

    public abstract bool ShouldInterrupt();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AIStateData
{
    public AIState state;
    public float lastStateChangeTime;
    public int flags;

    public static int FLAG_DEAD = 1;
    public static int FLAG_INITIAL = 2;
    public static int FLAG_NEXT = 4;

    /// <summary>
    /// A bitmask for flags that should be reset when we enter a new state
    /// </summary>
    public static int FLAGS_TRANSIENT = FLAG_NEXT + FLAG_INITIAL;

    public AIStateData(AIState initialState)
    {
        state = initialState;
        lastStateChangeTime = Time.time;
        flags = 0;
    }

    public void UpdateState(AIState newState)
    {
        state = newState;
        lastStateChangeTime = Time.time;
    }

    public bool CheckFlag(int mask)
    {
        return (flags & mask) != 0;
    }

    public void EnableFlag(int flag)
    {
        flags = flags | flag;
    }
}

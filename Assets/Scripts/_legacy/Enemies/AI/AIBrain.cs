using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public AIState[] PickableStates;
    int StateWeightTotal = 0;

    public AIInterrupter[] Interrupters;

    public AIState InitialState;
    public AIInterrupter[] InitialStateInterrupters;
    
    AIStateData currentAIStateData;
    public AIState CurrentState
    {
        get => currentAIStateData.state;
    }

    public void EnableFlag(int flag)
    {
        currentAIStateData.EnableFlag(flag);
    }

    private void Awake()
    {
        StateWeightTotal = 0;
        
        foreach(AIState state in PickableStates)
        {
            StateWeightTotal += state.BrainPickWeight;
        }
        
        currentAIStateData = new AIStateData(InitialState);
        currentAIStateData = CurrentState.OnEnter(currentAIStateData);
        currentAIStateData.EnableFlag(AIStateData.FLAG_INITIAL);
    }
    private void Update()
    {
        if (!currentAIStateData.CheckFlag(AIStateData.FLAG_DEAD))
        {
            AIStateData nextAIStateData = currentAIStateData.state.Think(currentAIStateData);
            nextAIStateData = TryInterrupt(nextAIStateData);
            if (nextAIStateData.state != currentAIStateData.state)
            {
                if (nextAIStateData.state == null)
                {
                    nextAIStateData.UpdateState(PickState());
                }

                currentAIStateData.state.OnExit(nextAIStateData);
                currentAIStateData = nextAIStateData;
                currentAIStateData = currentAIStateData.state.OnEnter(currentAIStateData);
            }
        }
    }

    AIState PickState()
    {
        int choice = Random.Range(0, StateWeightTotal);
        int i = 0;
        foreach( AIState state in PickableStates)
        {
            i += state.BrainPickWeight;
            if (i > choice)
            {
                return state;
            }
        }

        return PickableStates[0];
    }
    AIStateData TryInterrupt(AIStateData data)
    {
        if (data.CheckFlag(AIStateData.FLAG_INITIAL))
        {
            foreach (AIInterrupter Interrupter in InitialStateInterrupters)
            {
                if (Interrupter.ShouldInterrupt())
                {
                    data.UpdateState(Interrupter.nextState);
                }
            }
        }
        else if (data.state != null && data.state.CanBeInterrupted)
        {
            foreach (AIInterrupter Interrupter in Interrupters)
            {
                if (Interrupter.ShouldInterrupt())
                {
                    data.UpdateState(Interrupter.nextState);
                }
            }
        }
        
        return data;
    }

}

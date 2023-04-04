using UnityEngine;

namespace Entity.Being
{
    [RequireComponent(typeof(BeingActor))]
    public class BeingBrain : MonoBehaviour
    {
        BeingActor Target;

        float nextThought;

        private void Start()
        {
            Target = GetComponent<BeingActor>();
            nextThought = Time.time + Target.Table.ThoughtDelay;
        }

        private void Update()
        {
            if (nextThought < Time.time)
            {
                nextThought = Time.time + Target.Table.ThoughtDelay;
                if (Target.Table.Decisions.Count > 0)
                {
                    var decision = ChooseRandomDecision();
                    decision.Decide();
                }
            }
        }

        private IDecideable ChooseRandomDecision()
        {
            float TotalWeight = 0;
            foreach(IDecideable decision in Target.Table.Decisions)
            {
                TotalWeight += decision.Weight;
            }

            float chosenWeight = Random.Range(0, TotalWeight);
            foreach(IDecideable decision in Target.Table.Decisions)
            {
                if (chosenWeight < decision.Weight)
                {
                    return decision;
                }
                else
                {
                    chosenWeight -= decision.Weight;
                }
            }
            return Target.Table.Decisions[Target.Table.Decisions.Count - 1];
        }
    }
}

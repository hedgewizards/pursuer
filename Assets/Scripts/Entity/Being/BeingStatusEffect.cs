using Entity.Being.StatusEffects;
using JsonKnownTypes;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Entity.Being
{
    [JsonConverter(typeof(Json.BeingStatusJsonConverter))]
    [JsonDiscriminator(Name = "Type")]
    [JsonKnownType(typeof(PursuerSpeedBoost), nameof(PursuerSpeedBoost))]
    public abstract class BeingStatusEffect : ICloneable
    {
        public string Identifier;

        int currentStacks;
        /// <summary>
        /// How many stacks this Status Effect has. 
        /// </summary>
        public int CurrentStacks => currentStacks;

        /// <summary>
        /// 
        /// </summary>
        public int MaxStacks;
        public float Duration;

        private float lastApplyTime;
        /// <summary>
        /// what time was this status last updated?
        /// </summary>
        public float LastApplyTime => lastApplyTime;

        public EventHandler<UpdateStackCountEventArgs> OnUpdateStackCount;
        
        protected BeingActor target;

        protected virtual void Remove() { }
        protected virtual void Apply() { }

        public BeingStatusEffect()
        {

        }

        public BeingStatusEffect(BeingStatusEffect A)
        {
            Identifier = A.Identifier;
            MaxStacks = A.MaxStacks;
            Duration = A.Duration;
        }

        public void Initialize(BeingActor target)
        {
            this.target = target;
            currentStacks = 0;
        }

        public void AddStack()
        {
            AddStacks(1);
        }

        public void AddStacks(int count)
        {
            int initialStacks = CurrentStacks;
            currentStacks = MaxStacks == 0 ? currentStacks + count
                                           : Math.Min(MaxStacks, currentStacks + count);
            lastApplyTime = Time.time;

            if (initialStacks == 0)
            {
                Apply();
            }
            else
            {
                OnUpdateStackCount?.Invoke(this, new UpdateStackCountEventArgs(initialStacks, currentStacks));
            }
        }

        public void RemoveStacks(int count)
        {
            int initialStacks = CurrentStacks;
            currentStacks = Math.Max(currentStacks - count, 0);


            if (currentStacks == 0)
            {
                Remove();
            }
            else
            {
                OnUpdateStackCount?.Invoke(this, new UpdateStackCountEventArgs(initialStacks, currentStacks));
            }
        }

        public void RemoveAllStacks()
        {
            currentStacks = 0;
            Remove();
        }

        public abstract object Clone();

        public class UpdateStackCountEventArgs : EventArgs
        {
            public int InitialCount => initialCount;
            readonly int initialCount;

            public int FinalCount => finalCount;
            readonly int finalCount;

            public UpdateStackCountEventArgs(int initialCount, int finalCount)
            {
                this.initialCount = initialCount;
                this.finalCount = finalCount;
            }
        }
    }
}

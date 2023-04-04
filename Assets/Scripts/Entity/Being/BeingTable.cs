using Entity.Being.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being
{
    [System.Serializable]
    [JsonConverter(typeof(BeingTableJsonConverter))]
    public class BeingTable
    {
        BeingActor self;
        public string Name;

        #region Stats
        public ScalarStat SpeedStat;
        public float BaseSpeed;

        public float BaseHealth;

        /// <summary>
        /// How many times per second can this being make a decision?
        /// </summary>
        public float ThoughtSpeed = 1;
        public float ThoughtDelay => 1 / (ThoughtSpeed * UnityEngine.Random.Range( 1 - ThoughtSpeedVariance, 1 + ThoughtSpeedVariance));
        /// <summary>
        /// 
        /// </summary>
        public float ThoughtSpeedVariance = 0.15f;

        void setupStats()
        {
            SpeedStat = new ScalarStat(BaseSpeed);
        }

        #endregion

        public void Initialize(BeingActor self)
        {
            this.self = self;
            decisions = new List<IDecideable>();
            transientActions = new List<BeingAction>();

            setupStats();
            setupStatusEffects();
            self.OnThink += Think;

            foreach(BeingAction action in InnateActions)
            {
                action.Initialize(self);
            }
            foreach (BeingStance stance in Stances)
            {
                stance.Initialize(self);
            }
            EnterStance(InitialStance);
        }

        private void Think(object sender, EventArgs e)
        {
            updateStatuses();
        }

        #region Decisions
        private List<IDecideable> decisions;
        public List<IDecideable> Decisions => decisions;

        public void RegisterDecision(IDecideable decision)
        {
            decisions.Add(decision);
        }

        public void UnregisterDecision(IDecideable decision)
        {
            decisions.Remove(decision);
        }

        #endregion

        #region Stances
        [JsonIgnore] // this property is read in as a STRING from json
        internal BeingStance InitialStance;
        public BeingStance[] Stances;

        private BeingStance currentStance;
        public BeingStance CurrentStance => currentStance;

        public void EnterStance(string newStanceName)
        {
            if (currentStance.Name == newStanceName) return;

            foreach (BeingStance stance in Stances)
            {
                if (stance.Name == newStanceName)
                {
                    EnterStance(stance);
                    return;
                }
            }

            throw new KeyNotFoundException($"{Name} Tried to enter unknown stance named {newStanceName}");
        }

        private void EnterStance(BeingStance stance)
        {
            if (currentStance != null)
            {
                currentStance.Exit();
            }
            stance.Enter();
            currentStance = stance;
        }

        #endregion

        #region Actions
        public BeingAction[] InnateActions;

        List<BeingAction> transientActions;

        private BeingAction currentAction;
        public BeingAction CurrentAction => currentAction;

        internal void PerformAction(string actionName)
        {
            foreach (BeingAction action in InnateActions)
            {
                if (action.Name == actionName)
                {
                    PerformAction(actionName);
                    return;
                }
            }

            throw new KeyNotFoundException($"{Name} Tried to perform unknown action named {actionName}");
        }

        public void PerformAction(BeingAction action)
        {
            if (currentAction != null) return;

            if (!action.ActionIsInstantaneous)
            {
                currentAction = action;
                action.OnFinish += onActionFinished;
            }
            
            action.Start();
            self.OnAction?.Invoke(this, new BeingActionArgs(action.Name));
        }

        private void onActionFinished(object sender, EventArgs e)
        {
            currentAction.OnFinish -= onActionFinished;
            currentAction = null;
        }

        public void AddTransientAction(BeingAction action)
        {
            action.Initialize(self);
            transientActions.Add(action);
        }

        public void RemoveTransientAction(BeingAction action)
        {
            transientActions.Remove(action);
            action.Detatch();
        }

        #endregion

        #region Statuses

        public List<BeingStatusEffect> StatusEffects;

        void setupStatusEffects()
        {
            StatusEffects = new List<BeingStatusEffect>();
        }

        internal void ApplyStatus(BeingStatusEffect statusEffect, int stacks)
        {
            BeingStatusEffect myStatusEffect = getStatusEffect(statusEffect.Identifier);
            if (myStatusEffect == null)
            {
                myStatusEffect = statusEffect.Clone() as BeingStatusEffect;
                myStatusEffect.Initialize(self);
                StatusEffects.Add(myStatusEffect);
            }

            myStatusEffect.AddStacks(stacks);
        }

        BeingStatusEffect getStatusEffect(string identifier)
        {
            foreach(BeingStatusEffect effect in StatusEffects)
            {
                if (effect.Identifier == identifier) return effect;
            }
            return null;
        }

        private void updateStatuses()
        {
            List<int> indexesToRemove = new List<int>();
            for (int n = 0; n < StatusEffects.Count; n++)
            {
                var statusEffect = StatusEffects[n];
                if (statusEffect.Duration > 0 && Time.time > statusEffect.Duration + statusEffect.LastApplyTime)
                {
                    statusEffect.RemoveAllStacks();
                    indexesToRemove.Add(n);
                }
                else if (statusEffect.CurrentStacks == 0)
                {
                    indexesToRemove.Add(n);
                }
            }

            for (int n = indexesToRemove.Count - 1; n >= 0; n--)
            {
                StatusEffects.RemoveAt(indexesToRemove[n]);
            }
        }
        #endregion

        public static BeingTable FromJson(string json)
        {
            JsonConverter[] converters =
            {
                new BeingTableJsonConverter(),
            };

            return JsonConvert.DeserializeObject<BeingTable>(json, converters);
        }
    }
}

using Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being.StatusEffects
{
    public class PursuerSpeedBoost : BeingStatusEffect
    {
        public float SpeedIncreasePerStack;
        public float DamagePerStackLoss;

        float currentIncrease;
        float accruedDamage;

        public PursuerSpeedBoost()
        {

        }

        protected override void Apply()
        {
            accruedDamage = 0;
            recalculate();
            target.Table.SpeedStat.OnModifyStat += applyBonus;
            OnUpdateStackCount += onUpdateStackCount;
            target.OnTakeDamage += onTargetTakeDamage;

            base.Apply();
        }

        protected override void Remove()
        {
            target.Table.SpeedStat.OnModifyStat -= applyBonus;
            OnUpdateStackCount -= onUpdateStackCount;
            target.OnTakeDamage -= onTargetTakeDamage;

            base.Remove();
        }

        private void onTargetTakeDamage(object sender, DamageInfo e)
        {
            accruedDamage += e.FinalDamage;

            int stacksToLose = (int)Mathf.Floor(accruedDamage / DamagePerStackLoss);
            accruedDamage -= stacksToLose * DamagePerStackLoss;

            RemoveStacks(stacksToLose);
        }

        private void applyBonus(object sender,ScalarStat.OnModifyEventArgs e)
        {
            e.FlatBonus += currentIncrease;
        }

        private void onUpdateStackCount(object sender, UpdateStackCountEventArgs e)
        {
            recalculate();
            target.Table.SpeedStat.Invalidate();
        }

        void recalculate()
        {
            currentIncrease = Math.Max(0, SpeedIncreasePerStack * CurrentStacks);
        }

        #region Copying
        public PursuerSpeedBoost(PursuerSpeedBoost A) : base(A)
        {
            SpeedIncreasePerStack = A.SpeedIncreasePerStack;
            DamagePerStackLoss = A.DamagePerStackLoss;
        }

        public override object Clone()
        {
            return new PursuerSpeedBoost(this);
        }
        #endregion
    }
}

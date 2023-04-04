using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being.Stances
{
    public class ChaseStance : BeingStance
    {
        public float SpeedMultiplier = 1;

        public override void Enter()
        {
            self.PathfinderEnabled = true;
        }

        public override void Exit()
        {
            self.PathfinderEnabled = false;
        }

        void onApplySpeedMultiplier(object sender, Stat<float>.OnModifyEventArgs e)
        {
            e.Value *= SpeedMultiplier;
        }

        #region ModdableValues
        public override float CheckModdableValue(string key)
        {
            if (key == nameof(SpeedMultiplier))
            {
                return SpeedMultiplier;
            }

            return base.CheckModdableValue(key);
        }

        public override void SetModdableValue(string key, float newValue)
        {
            if (key == nameof(SpeedMultiplier))
            {
                SpeedMultiplier = newValue;
            }
            else
            {
                base.SetModdableValue(key, newValue);
            }
        }

        #endregion
    }
}

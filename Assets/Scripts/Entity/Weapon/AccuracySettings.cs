using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Weapon
{
    public class AccuracySettings
    {
        public float BaseAccuracy = 0;
        public float MovementPenaltyScale = 1;
        public float AttackPenaltyPerShot = 0.5f;
        public float AttackPenaltyScale = 1;
        public float AttackPenaltyDecayPower = 2f;
        public float AttackPenaltyDuration = 1;
        public float JostlePenaltyScale = 1;
    }
}

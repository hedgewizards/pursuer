using Combat;
using Entity.Being.Dancer;
using JsonKnownTypes;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being.Actions
{
    [JsonKnownThisType(nameof(RadialMeleeAttack))]
    public class RadialMeleeAttack : TwoStepBeingAction
    {
        public float Radius;
        Collider target;

        public BeingDance AttackHitDance;
        public BeingDance AttackMissDance;

        public float Damage;
        public Affliction[] Afflictions;

        public override void Start()
        {
            target = self.AcquireMeleeTarget(Radius);

            if (target != null)
            {
                base.Start();
            }
            else
            {
                OnStart?.Invoke(this, EventArgs.Empty);
                OnFinish?.Invoke(this, EventArgs.Empty);
                OnFinish = null;
            }
        }

        protected override void MidStep()
        {
            if (self.RaycastMeleeTarget(out RaycastHit? hit, target, Radius))
            {
                PlayerController player = target.GetComponent<PlayerController>();
                DamageInfo damageInfo = new DamageInfo(Damage, DamageInfo.TYPE.melee, self.AttackOrigin.position, Afflictions);
                player.ApplyDamage(damageInfo);

                self.Dancer.PerformDance(AttackHitDance, ref OnFinish);
            }
            else
            {
                self.Dancer.PerformDance(AttackMissDance, ref OnFinish);
            }
        }
    }
}

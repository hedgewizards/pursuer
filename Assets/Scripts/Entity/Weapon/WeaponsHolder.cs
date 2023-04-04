using UnityEngine;
using Entity.Weapon;

namespace Combat
{
    public abstract class WeaponsHolder : MonoBehaviour
    {
        public abstract Vector3 Origin { get; }
        public abstract Vector3 Direction { get; }
        public abstract float SpeedFraction {get; }

        public abstract AmmoTank AmmoTank { get; }

        public abstract WeaponActor DeployedWeapon { get; }

        float baseJostlePenalty = 0;
        [HideInInspector]
        public float CurrentJostlePenalty => Mathf.Pow(baseJostlePenalty, JostlePenaltyDecayPower);
        public float JostlePenaltyDecayPower = 2f;
        public float JostlePenaltyDecayTime = 1;

        public virtual void Update()
        {
            baseJostlePenalty = Mathf.Max(0, baseJostlePenalty - Time.deltaTime / JostlePenaltyDecayTime);
        }

        public void Jostle()
        {
            baseJostlePenalty = 1;
        }

        public abstract void ApplyForce(Vector3 force);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Weapon
{
    public abstract class WeaponModifier
    {
        protected WeaponActor self;

        public virtual void OnEquip(WeaponActor self)
        {
            this.self = self;
        }

        public virtual void OnDeploy() { }

        public virtual void OnHolster() { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Weapon
{
    public class WeaponActionNone : WeaponAction
    {
        public override bool Interrupt(bool dryRun, bool force)
        {
            return true;
        }

        public override bool Start(bool dryRun, bool force)
        {
            return false;
        }
    }
}

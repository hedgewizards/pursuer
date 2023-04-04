using Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class AmmoPickupController : PickupController
    {
        public Ammunition AmmoType;

        public void Awake()
        {
            item = AmmoType;
        }

    }
}

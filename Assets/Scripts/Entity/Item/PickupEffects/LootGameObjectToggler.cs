using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.PickupEffects
{
    public class LootGameObjectToggler : PickupEffect
    {
        protected override void OnLooted(object sender, EventArgs e)
        {
            gameObject.SetActive(false);
        }

        protected override void OnRespawned(object sender, EventArgs e)
        {
            gameObject.SetActive(true);
        }
    }
}

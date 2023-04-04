using Entity;

namespace Pursuer
{
    public class KeyItemPickupController : PickupController
    {
        public KeyItem Item;

        public void Awake()
        {
            item = Item;
        }

    }
}

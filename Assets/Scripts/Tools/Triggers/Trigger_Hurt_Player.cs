using UnityEngine;
using System.Collections;

namespace World
{
    public class Trigger_Hurt_Player : Trigger
    {
        [SerializeField] int DAMAGE_PER_TICK = 0;
        [SerializeField] float TICK_INTERVAL = 0; //time in seconds before each damage application
        float nextApplication = 0;

        void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            if (TICK_INTERVAL > 0 && Time.time > nextApplication)
            {
                return;
            }
            nextApplication = Time.time;
            other.gameObject.SendMessage("ChangeHealth", -DAMAGE_PER_TICK);
        }
    }
}

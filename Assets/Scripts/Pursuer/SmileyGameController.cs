using UnityEngine;
using Entity.Being;
using Entity.Being.StatusEffects;
using System;

namespace Pursuer
{
    public class SmileyGameController : MonoBehaviour
    {
        public BeingActor Smiley;
        public AudioSource AudioSource;

        private void Start()
        {
            PursuerGameManager.Instance.OnSkullPickedUp += onSkullPickedUp;
            AudioSource = GetComponent<AudioSource>();
            Smiley.OnDeath += onSmileyDeath;
        }

        Entity.Being.Actions.SelfBuff skull2Buff = new Entity.Being.Actions.SelfBuff()
        {
            Name = "Skull2Buff",
            StatusEffect = new PursuerSpeedBoost()
            {
                SpeedIncreasePerStack = 1,
                DamagePerStackLoss = 15,
                MaxStacks = 4
            }
        };

        private void onSkullPickedUp(object sender, int newSkullCount)
        {
            switch (newSkullCount)
            {
                case 1:
                    Smiley.gameObject.SetActive(true);
                    break;
                case 2:
                    Smiley.Table.AddTransientAction(skull2Buff);
                    break;
                case 3:
                    skull2Buff.StatusEffect.MaxStacks = 7;
                    break;
                case 4:
                    Smiley.Table.SpeedStat.OnModifyStat += onApplySkull3SpeedMod;
                    break;
                case 5:
                    Smiley.HealthTank.MaxHealth *= 0.1f;
                    Smiley.HealthTank.Health *= 0.1f;
                    break;
            }

            if (AudioSource != null) AudioSource.Play();
        }

        private void onApplySkull5HealthMod(object sender, ScalarStat.OnModifyEventArgs e)
        {
            e.Scale *= 0.1f;
        }

        private void onApplySkull3SpeedMod(object sender, ScalarStat.OnModifyEventArgs e)
        {
            e.FlatBonus += 3;
        }

        private void onSmileyDeath(object sender, EventArgs e)
        {
            PursuerGameManager.Instance?.EndGame(true);
        }
    }
}
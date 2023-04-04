using Entity.Effects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Combat;
using Entity.Weapon.Json;

namespace Entity.Weapon
{
    [System.Serializable]
    public class WeaponTable
    {
        public int CurrentClip = 0;

        /// <summary>
        /// Set up this weapon table assuming it's a fresh spawn
        /// </summary>
        public void Initialize(WeaponActor self)
        {
            CurrentClip = ClipSize;
            foreach (WeaponComponent c in getComponents())
            {
                c.OnEquip(self);
            }
            foreach (WeaponModifier modifier in Modifiers)
            {
                modifier.OnEquip(self);
            }
        }

        public void Deploy()
        {
            foreach(WeaponComponent c in getComponents())
            {
                c.OnDeploy();
            }
            foreach (WeaponModifier modifier in Modifiers)
            {
                modifier.OnDeploy();
            }
        }

        public void Holster()
        {
            foreach (WeaponComponent c in getComponents())
            {
                c.OnHolster();
            }
            foreach (WeaponModifier modifier in Modifiers)
            {
                modifier.OnHolster();
            }
        }

        WeaponComponent[] getComponents()
        {
            return new WeaponComponent[]
            {
                Reloader,
                PrimaryFire,
                AltFire
            };
        }
        WeaponAction[] getActs()
        {
            return new WeaponAction[]
            {
                Reloader,
                PrimaryFire,
                AltFire
            };
        }

        public string Name;
        public int ClipSize;
        public Ammunition AmmoType;
        public WeaponReloader Reloader;
        public WeaponAttack PrimaryFire;
        public WeaponAttack AltFire;
        public WeaponModifier[] Modifiers;
        public string WeaponUI = "WeaponUI/BasicWeaponHUD";

        #region Dancer
        public float CrosshairCone => PrimaryFire.CalculateSpread();

        public List<string> SoundPaths
        {
            get
            {
                var list = new List<string>();

                foreach (WeaponAction a in getActs())
                {
                    list.AddRange(a.SoundPaths);
                }
                list.AddRange(Reloader.SoundPaths);
                list.AddRange(PrimaryFire.SoundPaths);
                list.AddRange(AltFire.SoundPaths);

                return list;
            }
        }

        public bool EnableWeaponSway = true;
        public bool WeaponSwayIgnorePitch = false;
        public float WeaponSwayWeight = 1;
        #endregion

        public static WeaponTable FromJson(string json)
        {
            JsonConverter[] converters =
            {
                new WeaponTableJsonConverter(),
                new AmmunitionJsonConverter(),
                new WeaponActionJsonConverter(),
                new EffectTemplateJsonConverter(),
                new AccuracySettingsJsonConverter(),
                new WeaponModifierJsonConverter()
            };

            return JsonConvert.DeserializeObject<WeaponTable>(json, converters);
        }
    }
}
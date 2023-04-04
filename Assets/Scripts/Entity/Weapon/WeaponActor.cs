using Combat;
using Entity.Effects;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Entity.Weapon
{
    public class WeaponActor : MonoBehaviour
    {
        public WeaponTable Table;

        #region Messages
        public void Awake()
        {
            WeaponHolder = GetComponentInParent<WeaponsHolder>();

            if (WeaponHolder == null)
            {
                throw new MissingComponentException("Missing WeaponHolder for WeaponActor");
            }

            UpdateTableFromJson();
            Table.Initialize(this);
        }

        public void Update()
        {
            OnThink?.Invoke(this, EventArgs.Empty); 
        }
        #endregion

        #region Acting
        WeaponAction currentAction;

        void startAction(WeaponAction act, bool force)
        {
            if (currentAction != null)
            {
                if (!interruptAct(force)) return;
            }

            if (act.Start(false, force))
            {
                currentAction = act;
            }
        }

        bool interruptAct(bool force)
        {
            if (currentAction == null) return true;

            if (currentAction.Interrupt(false, force))
            {
                currentAction = null;
                return true;
            }

            return false;
        }
        internal void FinishAction(WeaponAction act)
        {

            if (currentAction == act)
                currentAction = null;
        }
        #endregion

        #region Events

        public EventHandler OnDeploy;
        public EventHandler OnHolster;
        public EventHandler OnDrop;
        public EventHandler OnThink;
        public EventHandler<WeaponActionEventArgs> OnAction;

        #endregion

        #region Weapon Holder

        public WeaponsHolder WeaponHolder;

        public Vector3 Origin => WeaponHolder.Origin;
        public Vector3 AimDirection => WeaponHolder.Direction;
        public float SpeedFraction => WeaponHolder.SpeedFraction;
        public float CurrentJostlePenalty => WeaponHolder.CurrentJostlePenalty;

        #endregion
        
        #region Inputs
        public void PrimaryFire(bool AutomaticOnly)
        {
            if (currentAction != null && currentAction == Table.Reloader)
            {
                interruptAct(false);
            }
            else
            {
                if (!AutomaticOnly || Table.PrimaryFire.IsAutomatic)
                {
                    startAction(Table.PrimaryFire, false);
                }
            }
        }
        public void AltFire(bool AutomaticOnly)
        {
            if (!AutomaticOnly || Table.AltFire.IsAutomatic)
            {
                startAction(Table.AltFire, false);
            }
        }
        public void Reload()
        {
            startAction(Table.Reloader, false);
        }
        public void CancelReload(bool force)
        {
            Table.Reloader.Interrupt(false, force);
        }

        public void Deploy()
        {
            OnDeploy?.Invoke(this, EventArgs.Empty);
            Table.Deploy();
        }
        public void Holster()
        {
            OnHolster?.Invoke(this, EventArgs.Empty);
            Table.Holster();
        }

        public float CrosshairCone => Table.CrosshairCone;

        #endregion

        #region Serialization
        public TextAsset Json;
        public void UpdateTableFromJson()
        {
            Table = WeaponTable.FromJson(Json.text);
        }

        #endregion
    }

}

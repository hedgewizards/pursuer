using Inputter;
using System;
using UnityEngine;
using Entity.Weapon;
using Player;

namespace Combat
{
    [RequireComponent(typeof(PlayerController))]
    public class WeaponsController : WeaponsHolder
    {
        PlayerController player;
        public WeaponActor StartingWeapon;

        public override Vector3 Origin => player.GetShootPos();

        public override Vector3 Direction => player.GetAimDir();

        public override float SpeedFraction => player.SpeedFraction;

        WeaponActor deployedWeapon;
        public override WeaponActor DeployedWeapon => deployedWeapon;

        private AmmoTank ammoTank;
        public override AmmoTank AmmoTank => ammoTank;

        public void Awake()
        {
            player = GetComponent<PlayerController>();
            ammoTank = GetComponent<AmmoTank>();
            player.OnJump += (sender, e) => Jostle();
        }

        public void Start()
        {
            DeployWeapon(StartingWeapon);
        }

        private void DeployWeapon(WeaponActor weapon)
        {
            deployedWeapon = weapon;
            DeployedWeapon.Deploy();
        }

        public override void Update()
        {
            base.Update();

            if(player.Paused)
            {
                return;
            }

            HUD.HUDManager.SetCrosshairCone(DeployedWeapon.CrosshairCone);

            if (InputManager.CheckKey(KeyName.Attack1))
            {
                bool attackKeyDown = InputManager.CheckKeyDown(KeyName.Attack1);
                DeployedWeapon.PrimaryFire(!attackKeyDown);
            }

            if (InputManager.CheckKey(KeyName.Attack2))
            {
                bool attackKeyDown = InputManager.CheckKeyDown(KeyName.Attack2);
                DeployedWeapon.AltFire(!attackKeyDown);
            }

            if (InputManager.CheckKey(KeyName.Reload))
            {
                DeployedWeapon.Reload();
            }
        }

        public override void ApplyForce(Vector3 force)
        {
            player.ApplyForce(force);
        }
    }
}

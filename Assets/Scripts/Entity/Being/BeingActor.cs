using Combat;
using Pathfinding;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Being
{
    [RequireComponent(typeof(BeingDancer))]
    [RequireComponent(typeof(HealthTank))]
    public class BeingActor : MonoBehaviour
    {
        public bool DisableMovement;

        public BeingTable Table;
        public Transform AttackOrigin;

        /// <summary>
        /// Should we use the pathfinder?
        /// </summary>
        public bool PathfinderEnabled
        {
            get => pathfinder.enabled;
            set
            {
                pathfinder.enabled = value;
            }
        }

        private void Awake()
        {
            if (HealthTank == null) throw new MissingComponentException($"Missing HealthTank for {name}");

            Dancer = GetComponent<BeingDancer>();

            pathfinder = GetComponent<AIPath>();

            UpdateTableFromJson();
            Table.Initialize(this);
            Dancer.Initialize(this);
        }

        private void Update()
        {
            OnThink?.Invoke(this, EventArgs.Empty);

            pathfinder.maxSpeed = DisableMovement ? 0 : Table.SpeedStat.Value;
            if (Time.time > nextPathfinderUpdate)
            {
                nextPathfinderUpdate = Time.time + MIN_UPDATE_DELAY;
                if (PlayerController.Instance != null)
                    pathfinder.destination = PlayerController.Instance.transform.position;
            }
        }



        #region Components
        [HideInInspector]
        public BeingDancer Dancer;
        public HealthTank HealthTank;
        #endregion

        #region Events
        public EventHandler OnThink;
        public EventHandler<DamageInfo> OnTakeDamage
        {
            get => HealthTank.OnTakeDamage;
            set
            {
                HealthTank.OnTakeDamage = value;
            }
        }

        public EventHandler OnAction;

        public EventHandler OnDeath
        {
            get => HealthTank.OnZeroHealth;
            set
            {
                HealthTank.OnZeroHealth = value;
            }
        }

        #endregion

        #region Movement
        public float CurrentSpeed => PathfinderEnabled ? pathfinder.desiredVelocity.Flatten().magnitude
                                                       : 0;
        #endregion

        #region Navigation
        AIPath pathfinder;

        float MIN_UPDATE_DELAY = 0.05f;
        float nextPathfinderUpdate = 0;
        #endregion

        #region Combat
        public Collider AcquireMeleeTarget(float radius, int layermask = LAYERMASK.player)
        {
            var colliders = Physics.OverlapSphere(AttackOrigin.position, radius, layermask);

            if (colliders.Length >= 1)
            {
                return colliders[0];
            }
            else
            {
                return null;
            }
        }

        public bool RaycastMeleeTarget(out RaycastHit? hit, Collider Target, float radius, int layermask = LAYERMASK.ENEMY_ATTACK)
        {
            var overlap = Physics.OverlapSphere(AttackOrigin.position, 0.1f, layermask);
            foreach(Collider overlappingCollider in overlap)
            {
                if (overlappingCollider == Target)
                {
                    hit = null;
                    return true;
                }
            }

            Ray ray = new Ray(AttackOrigin.position, Target.transform.position - AttackOrigin.position);
            bool didHit = Physics.Raycast(ray, out RaycastHit castHit, radius, layermask);

            hit = castHit;
            return didHit && hit?.collider == Target;
        }
        #endregion

        #region Inputs
        public void SetStance(string StanceName)
        {
            Table.EnterStance(StanceName);
        }

        public void PerformAction(string actionName)
        {
            Table.PerformAction(actionName);
        }

        public void ApplyStatus(BeingStatusEffect statusEffect, int stacks = 1)
        {
            Table.ApplyStatus(statusEffect, stacks);
        }
        #endregion

        #region Serialization
        public TextAsset Json;
        public void UpdateTableFromJson()
        {
            Table = BeingTable.FromJson(Json.text);
        }

        #endregion
    }
}
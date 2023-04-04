using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inputter;
using System;
using Pursuer;

namespace Player
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        public static PlayerController Instance;
        
        [HideInInspector]
        public bool Paused = false;

        [Range(0, 1)] public float soundScale = 1;

        //Classes and Components
        public Camera CAM;
        Animator animator;
        [HideInInspector] public CapsuleCollider CC;
        [HideInInspector] public PlayerStatus playerStatus;

        //Status
        public static float HP_MAX = 100;
        public float HP = HP_MAX;
        public float HEALTH_REGEN = 0.5f; //HP regen per second

        public static float ARMOR_MAX = 100;
        private float ARMOR = 0;
        public float ARMOR_MITIGATION = .75f; //the fraction of damage applied to armor before health

        private float fallVel = 0;
        public float slowModifier = 1; //1 is normal speed, 1.1 faster, 0.9 slower
        public float speedModifier = 1; //1 is normal speed, 1.1 faster, 0.9 slower
        public bool isSilenced = false;
        public bool isLaunched = false;

        //Collisions
        private float slopeLimit = 45;
        public static float skinWidth = 0.05f;
        static float PHYSICS_STEP_LIMIT = 0.275f;
        static float PHYSICS_ANTI_BHOP = 0.3f;
        static float PHYSICS_STAND_HEIGHT = 1.35f;
        static float PHYSICS_DUCK_HEIGHT = 0.7f;

        private bool isCrouched;
        public bool IsCrouched
        {
            get => isCrouched;
            set
            {
                animator.SetBool("crouch", value);
                isCrouched = value;
            }
        }
        public void StartCrouching()
        {
            if (PlayerState == State.AIRBORNE)
            {
                // if this is a midair crouch, we should tuck out legs up instead of crouching down
                transform.position += Vector3.up * (PHYSICS_STAND_HEIGHT - PHYSICS_DUCK_HEIGHT);
                animator.SetTrigger("DoInstantCrouch");
                IsCrouched = true;
            }
            else
            {
                IsCrouched = true;
            }
        }
        IEnumerator DelayedFinishCrouching()
        {
            yield return new WaitForSeconds(0.167f);

            IsCrouched = true;
        }
        void TryStanding()
        {
            if (PlayerState == State.AIRBORNE)
            {
                // can we extend our feet to stand up while in midair?
                if (!SimpleCheckStanding(transform.position - Vector3.up * (PHYSICS_STAND_HEIGHT - PHYSICS_DUCK_HEIGHT)))
                {
                    transform.position -= Vector3.up * (PHYSICS_STAND_HEIGHT - PHYSICS_DUCK_HEIGHT);
                    animator.SetTrigger("DoInstantStand");
                    IsCrouched = false;
                }
            }
            else if (!SimpleCheckStanding(transform.position)) // can we raise our head to stand up while on the ground?
            {
                IsCrouched = false;
            }
        }

        //Movement / FSM
        public enum State { GROUNDED, AIRBORNE, DEAD };
        private State internalState = State.GROUNDED; //ONLY change with changeState so shit changes right
        public State PlayerState
        {
            get { return internalState; }
            set
            {
                if (value == State.GROUNDED)
                {
                    if (internalState == State.AIRBORNE)
                    {
                        groundedTime = Time.time;
                        StopSliding();
                        DoLand();
                    }
                }
                else if (value == State.DEAD)
                {
                    StopSliding();
                }
                internalState = value;
            }
        }


        private float AIR_MULT_WALL = 2.5f; //acceleration multiplier to apply immediately after wall
        private float AIR_MULT_LAUNCHED = 1f; //acceleration multiplier to use while being launched
        private float AIR_ACCEL_WALL_TIME = 0.1f; //time in seconds after wall jump to use AIR_ACCELERATE_WALL
        private float GRAVITY = 11.5f; //acceleration due to gravity
        private float JUMP_SPEED = 3.75f; //speed at which you leave the ground
        private float WALL_JUMP_SPEED = 3.75f; //wall jump magnitude
        float JUMP_BUFFER = 0.3f; //time in seconds preceding touching the ground or a wall where pressing the jump button will cause an instant jump
        float jumpBufferTime; //the time at which a buffered jump will expire
        private float airResetTime = 0; //at what time to stop using wall accelerate
        private float groundedTime = 0; //the time at which the player last became grounded
        private bool airAccelUseWall = false;

        //Modifiable movement variables
        private float BACKPEDAL_MODIFIER = 0.6f; //movement speed modifier while backpedaling
        private float RUN_SPEED = 6f; //default movement speed
        private float WALK_SPEED = 2; //default walk speed
        private float WALK_ACCEL = 60; //how fast you get to max movespeed
        private float AIR_SPEED_MAX = 1f; //max tangent air speed, the lower this value the slower the airstrafe
        private float AIR_ACCELERATE = 60; //acceleration during air strafe
        private int WALL_JUMPS_MAX = 2; //max wall jumps until the player must land again

        bool isSliding = false; //is there a wall to wall jump off of?
        Surface slidingWall; //the surface of the wall
        int wallJumpsLeft;
        public Vector3 vel;

        //Audio stuff
        private float WALK_DIST_MIN = 3f; //min bound distance a step must take place in
        private float WALK_DIST_MAX = 2.7f; //max bound distance for a step
        private float walkDist = 0; //distance left to walk before a step
        [SerializeField] private AudioClip[] snd_step = null;
        [SerializeField] private AudioClip snd_jump = null;
        [SerializeField] private AudioClip snd_land = null;
        [SerializeField] private AudioClip snd_scrape = null;
        [SerializeField] private AudioClip snd_walljump = null;
        [SerializeField] private AudioClip snd_misswalljump = null;
        [SerializeField] private AudioClip snd_falldamage = null;
        [SerializeField] private AudioClip snd_death = null;
        private int snd_scrape_channel;
        public PlayerAudio AUD;
        private int AUDIO_CHANNELS = 4;

        //aiming
        private float SENSITIVITY = 0.1f; //default: 0.5. mouselook speed
        private float SENS_PITCH_SCALE = 0.8f; //fraction of sensitivity to apply for pitch
        private Vector3 internalEyeAngles; // player's aim direction before camera effects
        /// <summary>
        /// player's aim direction, in euler angles
        /// </summary>
        public Vector3 EyeAngles => internalEyeAngles + new Vector3(-1 * currentAimEffect.y, currentAimEffect.x, 0);

        //MESSAGES
        private void Start()
        {
            //setup AUD
            AudioSource[] src = new AudioSource[AUDIO_CHANNELS];
            for (int n = 0; n < AUDIO_CHANNELS; n++)
            {
                src[n] = gameObject.AddComponent<AudioSource>();
            }
            AUD = new PlayerAudio(src, soundScale);
            jumpBufferTime = 0;
            wallJumpsLeft = WALL_JUMPS_MAX;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            CC = GetComponent<CapsuleCollider>();
            internalEyeAngles = transform.eulerAngles;
            playerStatus = new PlayerStatus();
            animator = GetComponent<Animator>();
            cameraEffects = new List<CameraEffect>();
            Instance = this;

            if (Pursuer.PursuerGameManager.Instance != null)
            {
                Pursuer.PursuerGameManager.Instance.OnPauseStateChanged += OnPauseStateChanged;
            }
            SENSITIVITY = Pursuer.PursuerGameManager.Instance.SensitivitySetting.Value;
            Pursuer.PursuerGameManager.Instance.SensitivitySetting.OnChanged += OnSensitivityChanged;
        }

        private void OnDestroy()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }

        void Update()
        {
            if (Paused)
            {
                return;
            }

            DoLook();
            playerStatus.Update();

            //do ducking
            if (InputManager.CheckKey(KeyName.Duck))
            {
                if (!IsCrouched)
                {
                    StartCrouching();
                    //IsCrouched = true;
                }
            }
            else if (IsCrouched)
            {
                // if we are crouched but the player isn't pressing the key, try to uncrouch
                TryStanding();
            }

            fallVel = vel.y;
            //buffer jump
            if (InputManager.CheckKeyDown(KeyName.Jump))
            {
                jumpBufferTime = Time.time + JUMP_BUFFER;
            }

            //check player's state and do whatever based on their state
            if (PlayerState == State.GROUNDED)
            {
                //if the player happens to have jump buffered while grounded, just jump right away
                if (jumpBufferTime > Time.time)
                {
                    jumpBufferTime = 0;
                    DoJump();
                }
                else
                {
                    //putting this here means if you have jump buffered, you won't lose speed from the one ground frame
                    DoGroundMovement();
                }
                DoAux();
            }
            else if (PlayerState == State.AIRBORNE)
            {
                DoAirMovement();
                DoAux();
                DoSlide();
            }
            else if (PlayerState == State.DEAD)
            {
                Move(vel * Time.deltaTime);

                if (InputManager.CheckKeyDown(KeyName.Attack1) || InputManager.CheckKeyDown(KeyName.Jump))
                {
                    DoRespawn();
                }
            }
        }

        #region Pursuer
        private void OnPauseStateChanged(object sender, bool nowPaused)
        {
            Paused = nowPaused;
            Cursor.visible = nowPaused;
            Cursor.lockState = nowPaused ? CursorLockMode.None :  CursorLockMode.Locked;
        }

        private void OnSensitivityChanged(object sender, GameSettingChangedEventArgs e)
        {
            SENSITIVITY = e.FinalValue;
        }
        #endregion

        #region Events

        public EventHandler OnJump;

        #endregion

        #region Health
        public void ApplyDamage(DamageInfo damageInfo)
        {
            foreach (Affliction affliction in damageInfo.Afflictions)
            {
                affliction.ApplyToPlayer(damageInfo, this);
            }

            AimPunch(Mathf.Lerp(0, 10, damageInfo.Damage / 50));

            float amount = damageInfo.Damage;
            float totalArmorDamage = 0;
            float totalHealthDamage = 0;

            //mitigate damage with armor
            float armorAmount = amount * ARMOR_MITIGATION;
            float overkill = RemoveArmor(armorAmount, false);
            totalArmorDamage = armorAmount - overkill; //add armor removed to running count
            amount -= totalArmorDamage; //remove that amount from damage to apply

            if (ARMOR > 0 && amount >= HP && HP >= 2)
            {
                //if you have nonzero armor and this damage would be fatal, AND you aren't
                //already "near death", armor should attempt to mitigate ALL the rest of
                //the damage

                //first, damage is applied to set your HP to 1...
                float dam = HP - 1;
                RemoveHealth(dam, false);
                totalHealthDamage = dam; //add health damage to running count
                amount -= dam; //remove health damage from amount left to apply

                //now that your health is as low as possible, try and do the rest
                //of the damage to the player's health bar
                overkill = RemoveArmor(amount, false);
                totalArmorDamage += amount - overkill; //apply whatever was dealt to totalArmorDamage
                amount = overkill; //the final amount should be whatever didn't get applied
            }

            //remove any health not mitigated by armor
            RemoveHealth(amount, false);
            totalHealthDamage += amount;
        }
        public void RemoveHealth(float amount, bool doCreateFloater = true)
        {
            HP -= amount;

            if (HP <= 0)
            {
                DoDeath();
            }
        }
        public float AddHealth(float amount)
        {
            return AddHealth(amount, true);
        }
        public float AddHealth(float amount, bool doCreateFloater)
        {
            if (PlayerState == State.DEAD)
                return amount;

            float newHP = HP + amount;


            if (newHP > HP_MAX)
            {
                HP = HP_MAX;
                return newHP - HP_MAX;
            }
            HP = newHP;

            return 0;

        }
        /// <summary>
        /// attempt to remove armor from the player, returning the overkill amount
        /// </summary>
        /// <param name="amount">amount of armor to remove</param>
        /// <returns>leftover armor damage that wasn't applied</returns>
        public float RemoveArmor(float amount, bool doCreateFloater = true)
        {
            float newArmor = ARMOR - amount;
            if (newArmor < 0)
            {
                ARMOR = 0;
                return 0 - newArmor;
            }
            ARMOR = newArmor;
            return 0;
        }
        public float AddArmor(float amount, bool doCreateFloater = true)
        {
            float newArmor = ARMOR + amount;

            //if you overheal, cap it
            if (newArmor > ARMOR_MAX)
            {
                ARMOR = ARMOR_MAX;
                //and return the amount that wasn't added
                return newArmor - ARMOR_MAX;
            }
            ARMOR = newArmor;
            return 0;
        }

        #endregion

        /// <summary>
        /// Return a rotation along player's aim direction
        /// </summary>
        /// <returns></returns>
        public Quaternion GetAimQuat()
        {
            return Quaternion.Euler(EyeAngles);
        }
        /// <summary>
        /// Return a rotation of the player's aim direction, flattened
        /// </summary>
        /// <returns></returns>
        public Quaternion GetYawQuat()
        {
            return Quaternion.Euler(0, EyeAngles.y, 0);
        }
        /// <summary>
        /// Returns a vector along player's aim direction
        /// </summary>
        /// <returns name = aimDir></returns>
        public Vector3 GetAimDir()
        {
            return GetAimQuat() * Vector3.forward;
        }
        /// <summary>
        /// returns the camera's position
        /// </summary>
        /// <returns></returns>
        public Vector3 GetShootPos()
        {
            return CAM.transform.position;
        }

        public bool EyeCast(Vector3 direction, float maxDistance, out RaycastHit hitInfo, int layerMask = LAYERMASK.SOLIDS_ONLY)
        {
            return Physics.Raycast(CAM.transform.position, direction, out hitInfo, maxDistance, layerMask);
        }
        public bool EyeCastSphere(Vector3 direction, float maxDistance, out RaycastHit hitInfo, int layerMask = LAYERMASK.SOLIDS_ONLY)
        {
            return SimpleCastSphere(GetShootPos(), direction, maxDistance, out hitInfo, layerMask);
        }
        public bool SimpleCastSphere(Vector3 position, Vector3 direction, float maxDistance, out RaycastHit hitInfo, int layerMask = LAYERMASK.SOLIDS_ONLY)
        {
            if (Physics.SphereCast(position, CC.radius, direction, out hitInfo, maxDistance, layerMask))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// performs a capsuleCast fitting the character's shape
        /// </summary>
        /// <param name="position">initial position (transform.position)</param>
        /// <param name="direction">unit direction to travel in</param>
        /// <param name="distance">float distance to travel for</param>
        /// <param name="hitInfo">RaycastHit info to return</param>
        /// <param name="backstep">how far back from initial point to start</param>
        /// <returns name = didHit>Whether or not a surface was hit</returns>
        public bool SimpleCast(Vector3 position, Vector3 direction, float distance, out RaycastHit hitInfo, int layermask = LAYERMASK.SOLIDS_ONLY)
        {
            Vector3 p1 = position + CC.center + Vector3.down * (CC.height * .5f - CC.radius);
            Vector3 p2 = p1 + Vector3.up * (CC.height - CC.radius * 2);
            bool didHit = Physics.CapsuleCast(p1, p2, CC.radius, direction, out hitInfo, distance, layermask);

            return didHit;
        }
        public bool SimpleCast(Ray ray, float distance, out RaycastHit hitInfo, int layermask = LAYERMASK.SOLIDS_ONLY)
        {
            Vector3 pos = ray.origin;
            Vector3 dir = ray.direction;
            return SimpleCast(pos, dir, distance, out hitInfo, layermask);
        }
        public bool SimpleCheck(Vector3 position, int layerMask = LAYERMASK.SOLIDS_ONLY)
        {
            Vector3 p1 = position + CC.center + Vector3.down * (CC.height * .5f - CC.radius);
            Vector3 p2 = p1 + Vector3.up * (CC.height - CC.radius * 2);
            return Physics.CheckCapsule(p1, p2, CC.radius, layerMask);
        }
        public bool SimpleCheckStanding(Vector3 position, int layerMask = LAYERMASK.SOLIDS_ONLY)
        {
            Vector3 standingCenter = Vector3.up * PHYSICS_STAND_HEIGHT / 2;
            Vector3 p1 = position + standingCenter + Vector3.down * (PHYSICS_STAND_HEIGHT * .5f - CC.radius);
            Vector3 p2 = p1 + Vector3.up * (PHYSICS_STAND_HEIGHT - CC.radius * 2);
            return Physics.CheckCapsule(p1, p2, CC.radius, layerMask);
        }
        public Collider[] SimpleOverlap(Vector3 position, int layermask = LAYERMASK.SOLIDS_ONLY)
        {
            Vector3 p1 = position + CC.center + Vector3.down * (CC.height * .5f - CC.radius);
            Vector3 p2 = p1 + Vector3.up * (CC.height - CC.radius * 2);
            return Physics.OverlapCapsule(p1, p2, CC.radius, layermask);
        }
        public void AddStatus(StatusEffect newEff)
        {
            playerStatus.AddStatus(newEff);
        }
        /// <summary>
        /// Move the player the specified amount, making sure they are moving at least as fast, if it traveled movement distance every interval
        /// </summary>
        /// <param name="movement">the distance to move the player</param>
        /// <param name="interval">over what duration the force was applied (doesnt actually happen over a duration, it's Zto calculate change in velocity)</param>
        public void Shove(Vector3 movement, float interval)
        {
            //move the player the correct distance.
            transform.position += movement;
            Accelerate(movement.normalized, movement.magnitude / interval);
        }
        /// <summary>
        /// Shoves the player to the specified point, making sure they are moving at least as fast, if it traveled movement distance every interval
        /// </summary>
        /// <param name="newPos">the point to move to</param>
        /// <param name="interval">over what duration the force was applied (doesnt actually happen over a duration, it's Zto calculate change in velocity)</param>
        public void ShoveTo(Vector3 newPos, float interval)
        {
            //but it was me, DIO-- i mean Shove() in disguise
            Vector3 movement = newPos - transform.position;
            Shove(movement, interval);
        }
        public void Accelerate(Vector3 direction, float maxSpeed)
        {
            float curSpeed = Vector3.Dot(vel, direction);
            float addSpeed = maxSpeed - curSpeed;
            if (addSpeed <= 0)
            {
                //already going too fast, so who cares
                return;
            }

            //since I'm not going too fast, make me go just the right speed in that direction
            vel += addSpeed * direction;

            //make the player airborne just in case they are going to leave the ground
            //if they aren't, they will just be made grounded again immediately
            PlayerState = State.AIRBORNE;
        }
        public void ApplyForce(Vector3 force)
        {
            vel += force;

            if (vel.y > 0)
                PlayerState = State.AIRBORNE;
        }
        public float GetTopSpeed()
        {
            float curTopSpeed = IsCrouched ? WALK_SPEED : RUN_SPEED; //if holding +speed, use WALK_SPEED else use RUN_SPEED
            curTopSpeed = curTopSpeed * speedModifier * slowModifier;
            return curTopSpeed;
        }
        public float SpeedFraction => Mathf.Clamp01(vel.magnitude / RUN_SPEED);
        /// <summary>
        /// Move player applying collisions, and applying change to your velocity
        /// </summary>
        /// <param name="movement">Distance + Direction to travel (scale by deltaT)</param>
        void Move(Vector3 movement)
        {
            if (movement.magnitude < 0.003)
            {
                //if you aren't moving, at least check if there's ground below you still
                CheckForGround();
                return;
            }

            bool groundFound = false;

            Vector3 mov = movement;
            while (mov.magnitude >= 0.003)
            {
                RaycastHit hit;
                bool didHit = SimpleCast(transform.position, mov.normalized, mov.magnitude + skinWidth, out hit);

                if (didHit)
                {
                    //if you're airborne, try and slide on or stand on the surface
                    float angle = Vector3.Angle(Vector3.up, hit.normal);
                    if (PlayerState == State.AIRBORNE)
                    {

                        //if you aren't already sliding, see if you can slide on this surface
                        if (!isSliding)
                        {

                            if (angle <= 91.01 && angle > slopeLimit)
                            {
                                //91.01 because as long as there's no DISCERNEABLE overhang it's fine,
                                //and the .01 because it's imprecise and won't be consistent on 91 deg slopes

                                StartSliding(new Surface(transform.position, hit.normal));
                            }
                        }

                        //check if this is a surface you can stand on
                        if (angle < slopeLimit)
                        {
                            PlayerState = State.GROUNDED;
                            groundFound = true;
                        }
                    }
                    else if (PlayerState == State.GROUNDED)
                    {
                        if (angle < slopeLimit)
                        {
                            // if the collision point was below our kneecaps, slide up the surface instead

                            groundFound = true;
                        }
                        else if (hit.point.y - transform.position.y < PHYSICS_STEP_LIMIT)
                        {
                            // We need to see if this is a step. fire a raycast a little in front to try and find a step
                            RaycastHit stepHit;

                            // if there IS a step, and the top of it is something we can stand on...
                            if (Physics.Raycast(hit.point + mov.normalized * skinWidth + Vector3.up * skinWidth, Vector3.down, out stepHit, PHYSICS_STEP_LIMIT, LAYERMASK.SOLIDS_ONLY, QueryTriggerInteraction.UseGlobal)
                                && Vector3.Angle(Vector3.up, stepHit.normal) < slopeLimit)
                            {
                                //move us up that step
                                float stepHeight = stepHit.point.y - transform.position.y;

                                // first, move upwards
                                RaycastHit stepClimbHit;
                                if (SimpleCast(transform.position, Vector3.up, stepHeight + skinWidth, out stepClimbHit))
                                {
                                    // if we hit something, the ceiling is too low to climb this step
                                    groundFound = true;
                                }
                                else
                                {
                                    // otherwise, move upwards
                                    transform.position += Vector3.up * (stepHeight + skinWidth);

                                    // then try and move forwards
                                    mov = mov.Flatten();
                                    vel = vel.Flatten();
                                    SimpleCast(transform.position, mov.normalized, mov.magnitude + skinWidth, out hit);
                                }
                            }
                        }
                    }

                    //move up to the object
                    transform.position += (hit.distance - skinWidth) * mov.normalized;

                    //remove distance moved from mov
                    mov -= (hit.distance - skinWidth) * mov.normalized;

                    //remove non-tangent component from mov AND vel
                    mov = mov.GetTangentComponent(hit.normal);
                    vel = vel.GetTangentComponent(hit.normal);
                }
                else
                {
                    transform.position += mov;
                    mov = Vector3.zero;
                }
            }

            if (PlayerState == State.GROUNDED && !groundFound)
            {
                CheckForGround();
            }
            return;
        }
        void DoGroundMovement()
        {
            float slopeSpeedMultiplier = 1;
            Vector3 slopeNormal = Vector3.up;
            Vector3 localForward = GetYawQuat() * Vector3.forward;
            Vector3 movement = InputManager.CheckAxisSet(AxisSetName.Movement);

            //check the slope of the ground
            RaycastHit hitInfo;
            bool didHit = SimpleCast(transform.position, Vector3.down, 0.01f + skinWidth, out hitInfo);
            if (didHit)
            {
                Vector3 desiredMoveDirection = GetYawQuat() * (movement == Vector3.zero ? Vector3.forward : movement);
                transform.position += (hitInfo.distance - skinWidth) * Vector3.down;
                slopeNormal = hitInfo.normal;
                slopeSpeedMultiplier = 1 / desiredMoveDirection.GetTangentComponent(slopeNormal).normalized.Flatten().magnitude;
            }

            //Change vel based on inputs
            Vector3 movementTransformed = (GetYawQuat() * movement).GetTangentComponent(slopeNormal).normalized;

            float curTopSpeed = GetTopSpeed();

            //Adjust top speed if we're backpedaling
            float excessSpeed = Vector3.Dot(movementTransformed, localForward) + BACKPEDAL_MODIFIER;
            if (excessSpeed < 0)
            {
                movementTransformed -= localForward * excessSpeed;
            }

            vel = Vector3.MoveTowards(vel, movementTransformed * curTopSpeed * slopeSpeedMultiplier, WALK_ACCEL * Time.deltaTime);

            //move
            Move(vel * Time.deltaTime);

            //do footstep sounds
            walkDist -= vel.magnitude * Time.deltaTime;
            if (walkDist < 0)
            {
                AUD.PlayOnce(snd_step[(int)Mathf.Floor(UnityEngine.Random.value * snd_step.Length)]);
                walkDist = UnityEngine.Random.Range(WALK_DIST_MIN, WALK_DIST_MAX);
            }
        }
        void DoAirMovement()
        {
            //Change vel based on inputs
            Vector3 movement = InputManager.CheckAxisSet(AxisSetName.Movement);
            Vector3 airMove = GetYawQuat() * new Vector3(movement.x, 0, movement.z);
            vel = AirAccelerate(vel, airMove);

            //apply gravity
            vel += Vector3.down * GRAVITY * Time.deltaTime;

            //move
            Move(vel * Time.deltaTime);
        }
        /// <summary>
        /// Remove a fraction of the player's speed over the cap every time they hit the ground
        /// </summary>
        void DoAntiBhop()
        {
            float targetSpeed = RUN_SPEED;
            float currentSpeed = vel.Flatten().magnitude;

            if (targetSpeed < currentSpeed)
            {
                float mul = 1 - PHYSICS_ANTI_BHOP + PHYSICS_ANTI_BHOP * targetSpeed / currentSpeed;
                vel.x = vel.x * mul;
                vel.z = vel.z * mul;
            }
        }
        void DoLook()
        {
            //change eyeAngles based on mouse
            internalEyeAngles += InputManager.CheckAxisSet(AxisSetName.Aim).Scale(SENSITIVITY * SENS_PITCH_SCALE, SENSITIVITY, SENSITIVITY);
            //clamp pitch
            internalEyeAngles.x = Mathf.Clamp(internalEyeAngles.x, -89.9f, 89.9f);

            recalculateCurrentAimEffect();

            CAM.transform.rotation = GetAimQuat();
        }
        void DoAux()
        {
            DoHealthRegen();
        }
        void DoSlide()
        {
            if (!isSliding) return;

            //if you've gotten too far from the wall, stop sliding
            if (Vector3.Dot(transform.position - slidingWall.GetPosition(), slidingWall.GetNormal()) > 0.1)
            {
                StopSliding();
                return;
            }

            //check if the wall is still there
            RaycastHit hit;
            if (!SimpleCast(transform.position, -slidingWall.GetNormal(), 0.5f, out hit))
            {
                /*
                 * the 0.5f cast distance might seem a little extreme, but because of the statement above this one
                 * the only situation where this could cause a problem is with 2 parallel walls where the second one
                 * is slightly further back, which would be pretty rare, if at all existent architecture
                 */
                //if NOTHING AT ALL is there, stop sliding
                StopSliding();
                return;
            }
            else
            {
                //if there is SOMETHING there, see if you can slide on it
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                if (angle <= 90.1f && angle > slopeLimit)
                {
                    //if you can, then replace the listed sliding wall
                    slidingWall = new Surface(transform.position, hit.normal);
                }
                else
                {
                    //if you can't, then stop sliding
                    StopSliding();
                    return;
                }
            }

            //check if the player tried to walljump

            if (jumpBufferTime > Time.time)
            {
                jumpBufferTime = 0;
                if (wallJumpsLeft > 0)
                    DoWallJump(slidingWall.GetNormal());
                else
                    AUD.PlayOnce(snd_misswalljump);
            }

        }
        /// <summary>
        /// Airstrafing function from quake 3 movement code
        /// </summary>
        /// <param name="vel">Initial Velocity</param>
        /// <param name="mov">player's movement input</param>
        /// <returns>New velocity</returns>
        Vector3 AirAccelerate(Vector3 vel, Vector3 mov)
        {
            float ACCEL = AIR_ACCELERATE;
            //check airAccel reset from walljumping
            if (airAccelUseWall)
            {
                if (airResetTime < Time.time)
                {
                    airAccelUseWall = false;
                }
                else
                {
                    ACCEL *= AIR_MULT_WALL;
                }
            }
            if (isLaunched)
            {
                ACCEL = AIR_MULT_LAUNCHED;
            }

            Vector3 flatVel = vel.Flatten();
            float currentspeed = Vector3.Dot(flatVel, mov); //gets velocity along movement
                                                            //weird step here from the quake guys
                                                            //really, i want to see if AIR_SPEED_MAX is bigger than currentspeed
                                                            //then, if it is, add the amount it is bigger by the player's speed along mov
                                                            //this just cuts out a step because lol
            float addspeed = AIR_SPEED_MAX - currentspeed; //finds how far from max your current speed is

            if (addspeed <= 0) //if you're already accelerating over max, don't change anything
                return vel;

            float accelspeed = ACCEL * Time.deltaTime * AIR_SPEED_MAX; //finds the acceleration to add
            if (accelspeed > addspeed) //if the acceleration would put you over max, cap it at max.
                accelspeed = addspeed;

            return vel + accelspeed * mov;
        }

        void StopSliding()
        {
            if (!isSliding)
                return;
            isSliding = false;
            if (snd_scrape_channel != -1)
            {
                AUD.StopLoop(snd_scrape_channel);
            }

        }
        void StartSliding(Surface newSurface)
        {
            isSliding = true;
            slidingWall = newSurface;
            snd_scrape_channel = AUD.PlayLoop(snd_scrape);

        }
        void DoJump()
        {
            vel.y = JUMP_SPEED * slowModifier;
            PlayerState = State.AIRBORNE;

            //do jump sound & play animation
            AUD.PlayOnce(snd_jump);

            OnJump?.Invoke(this, EventArgs.Empty);
        }
        void DoWallJump(Vector3 wallNormal)
        {
            airAccelUseWall = true;
            airResetTime = Time.time + AIR_ACCEL_WALL_TIME;
            //become airborne if you aren't already
            PlayerState = State.AIRBORNE;

            //do jump sound & play animation
            AUD.PlayOnce(snd_walljump);

            //add velocity
            vel += slidingWall.GetNormal() / 2 * WALL_JUMP_SPEED;
            vel.y = WALL_JUMP_SPEED;

            wallJumpsLeft--;

            OnJump?.Invoke(this, EventArgs.Empty);
        }
        void DoLand()
        {
            wallJumpsLeft = WALL_JUMPS_MAX;
            DoAntiBhop();

            if (fallVel < -20)
            {
                AUD.PlayOnce(snd_falldamage);
                RemoveHealth(10);
            }
            else if (fallVel < -2)
            {
                AUD.PlayOnce(snd_land);
            }
        }
        void DoDeath()
        {
            PursuerGameManager.Instance?.EndGame(false);
        }
        void DoRespawn()
        {
        }
        void DoHealthRegen()
        {
            AddHealth(HEALTH_REGEN * Time.deltaTime, false);
        }
        void CheckForGround()
        {
            //if you didn't hit the ground with the move, double check to see if there is still ground below you
            //note CollisionFlags.Below just means you hit SOMETHING below you, up to 89.999 degree slope. code accordingly
            RaycastHit groundHit;
            if (SimpleCast(transform.position, Vector3.down, 0.01f + skinWidth, out groundHit))
            {
                if (Vector3.Angle(groundHit.normal, Vector3.up) > slopeLimit)
                {
                    PlayerState = State.AIRBORNE;
                }
                else
                {
                    transform.position += (groundHit.distance - skinWidth) * Vector3.down;
                }
            }
            else
            {
                PlayerState = State.AIRBORNE;
            }
        
        }

        Vector2 currentAimEffect;
        public List<CameraEffect> cameraEffects;

        void recalculateCurrentAimEffect()
        {
            Vector2 total = Vector2.zero;
            for(int n = cameraEffects.Count - 1; n >= 0; n--)
            {
                var effect = cameraEffects[n];
                if (effect.StartTime + CameraEffect.Duration < Time.time)
                {
                    cameraEffects.RemoveAt(n);
                }
                else
                {
                    total += effect.Calculate();
                }
            }
            currentAimEffect = total;
        }

        public void AimPunch(float magnitude)
        {
            float angle = UnityEngine.Random.Range(Mathf.PI / 4, 3 * Mathf.PI / 4);
            Vector2 Direction = Vector2.right.RotateRadians(angle);

            AimPunch(Direction * magnitude);
        }

        public void AimPunch(Vector2 maxPunch)
        {
            cameraEffects.Add(new CameraEffect(maxPunch, Time.time));
        }
    }
}
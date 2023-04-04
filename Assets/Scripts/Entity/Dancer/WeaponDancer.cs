using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;
using Entity.Effects;
using HUD;

namespace Entity.Weapon
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(WeaponActor))]
    [RequireComponent(typeof(AudioSource))]
    public class WeaponDancer : MonoBehaviour
    {
        [Range(0f,1f)]
        public float Volume = 1;
        public Transform EffectOrigin;
        new Transform camera;
        WeaponActor weaponActor;
        Animator animator;
        AudioSource audioSource;
        WeaponUIController weaponUI;

        Dictionary<string, AudioClip> audioClips;

        public void Awake()
        {
            camera = Camera.main.transform;
            weaponActor = GetComponent<WeaponActor>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

            weaponActor.OnAction += OnWeaponAction;
            weaponActor.OnDrop += OnDrop;
        }

        public void Start()
        {
            initializeWeaponSway();
            loadAudioClips();
            createWeaponUI();
        }

        private void createWeaponUI()
        {
            string weaponUIPath = weaponActor.Table.WeaponUI;
            GameObject prefab = Resources.Load<GameObject>(weaponUIPath);
            GameObject spawnedUIObject = Instantiate(prefab, HUDManager.self.WeaponHUDRoot.transform);
            weaponUI = spawnedUIObject.GetComponent<WeaponUIController>();
            weaponUI.Initialize(weaponActor);
            if (weaponActor.WeaponHolder.DeployedWeapon == weaponActor)
            {
                weaponUI.OnDeploy(this, EventArgs.Empty);
            }
        }

        public void LateUpdate()
        {
            if (EnableWeaponSway)
            {
                DoWeaponSway();
            }

        }

        #region Event Listening
        void OnWeaponAction(object sender, WeaponActionEventArgs e)
        {
            switch (e.Type)
            {
                case WeaponActionEventArgs.EventType.Sound:
                    playSound(e as WeaponActionSoundEventArgs);
                    break;
                case WeaponActionEventArgs.EventType.Effect:
                    playEffect(e as WeaponActionEffectEventArgs);
                    break;
                case WeaponActionEventArgs.EventType.AnimatorTrigger:
                    animator.SetTrigger(e.Name);
                    break;
                case WeaponActionEventArgs.EventType.AnimatorBool:
                    animator.SetBool(e.Name,(e as WeaponActionBoolEventArgs).Bool);
                    break;
                case WeaponActionEventArgs.EventType.AnimatorFloat:
                    animator.SetFloat(e.Name, (e as WeaponActionFloatEventArgs).Float);
                    break;
            }
        }

        private void OnDrop(object sender, EventArgs e)
        {
            Destroy(weaponUI);
        }

        #endregion

        #region Weapon Sway
        bool EnableWeaponSway
        {
            get
            {
                return weaponActor.Table.EnableWeaponSway;
            }
        }
        bool WeaponSwayIgnorePitch => weaponActor.Table.WeaponSwayIgnorePitch;
        float WeaponSwayWeight => weaponActor.Table.WeaponSwayWeight;

        Vector3 positionOffset;
        Vector3 lastPosition;
        Quaternion rotationOffset;
        Quaternion lastRotation;

        void initializeWeaponSway()
        {
            transform.rotation = GetDesiredRotation();
            positionOffset = Vector3.zero;
            lastPosition = camera.position;
        }
        public void DoWeaponSway()
        {
            // Calculate change in position of the camera, and accumulate that in our positionOffset
            positionOffset -= camera.position - lastPosition;
            positionOffset = Vector3.ClampMagnitude(positionOffset, 1);

            // Decay positionOffset towards zero over time
            positionOffset = Vector3.Lerp(positionOffset, Vector3.zero, Time.deltaTime * 10);

            // Calculate change in rotation of the camera, and accumulate it in our rotationOffset
            rotationOffset = rotationOffset * camera.rotation * Quaternion.Inverse(lastRotation);

            // Decay rotationOffset towards zero over time
            rotationOffset = Quaternion.Slerp(rotationOffset, Quaternion.identity, Time.deltaTime * 40 / WeaponSwayWeight);

            // store the current rotation / position of the camera for next frame
            lastPosition = camera.position;
            lastRotation = camera.rotation;

            // Set the position / rotation of our weapon including the offsets
            transform.localPosition = positionOffset * (WeaponSwayWeight / 10);
            transform.rotation = Quaternion.Inverse(rotationOffset) * GetDesiredRotation();
        }
        public Quaternion GetDesiredRotation()
        {
            float pitch = camera.eulerAngles.x;
            if (WeaponSwayIgnorePitch)
            {
                // for some weapons, we don't want the weapon to pitch up and down when the player does
                // but still apply 1/4 of the value so there's a tiny bit of movement when you aim
                if (pitch > 180) pitch -= 360;
                pitch = Mathf.Lerp(pitch, 0, 0.75f);
            }
            return Quaternion.Euler(pitch, camera.eulerAngles.y, camera.eulerAngles.z);
        }
        #endregion

        #region Effects
        private void playEffect(WeaponActionEffectEventArgs effectArgs)
        {
            var name = effectArgs.Name;
            var data = effectArgs.Data;

            if (effectArgs.CheckFlag(WeaponActionEffectEventArgs.EffectOption.OriginToWeaponOrigin))
            {
                data.origin = EffectOrigin.position;
            }

            if (effectArgs.CheckFlag(WeaponActionEffectEventArgs.EffectOption.PositionToWeaponOrigin))
            {
                data.position = EffectOrigin.position;
            }

            if (effectArgs.CheckFlag(WeaponActionEffectEventArgs.EffectOption.ParentToWeaponOrigin))
            {
                data.parent = EffectOrigin;
            }

            if (effectArgs.CheckFlag(WeaponActionEffectEventArgs.EffectOption.ParentCanReplace))
            {
                EffectReplacer replacer = data.parent.GetComponent<EffectReplacer>();
                if (replacer != null)
                {
                    name = replacer.GetReplacement(name);
                }
            }

            EffectManager.CreateEffect(name, data);
        }


        #region Audio
        private void playSound(WeaponActionSoundEventArgs soundArgs)
        {
            AudioClip clip = audioClips[soundArgs.Name];

            audioSource.PlayOneShot(clip, Volume);
        }

        void loadAudioClips()
        {
            audioClips = new Dictionary<string, AudioClip>();

            foreach (string path in weaponActor.Table.SoundPaths)
            {
                if (audioClips.ContainsKey(path)) continue;

                AudioClip clip = Resources.Load<AudioClip>(path);
                if (clip == null) throw new System.IO.FileNotFoundException($"Missing AudioClip for {weaponActor.Table.Name} at {path}");

                audioClips[path] = clip;
            }
        }
        #endregion
    }

}
#endregion
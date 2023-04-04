using System;

namespace Entity.Weapon
{
    public abstract class WeaponAction : WeaponComponent
    {
        public EventHandler OnActStart;

        public string OverrideActionName;
        public string SoundPath;

        public virtual string[] SoundPaths => (SoundPath == null) ? new string[] { } : new[] { SoundPath };

        /// <summary>
        /// Start performing this act
        /// </summary>
        /// <param name="dryRun">If true, don't actually start, just return if you can</param>
        /// <param name="force">If true, start the act by force</param>
        /// <returns>whether the act was started (or if dryRun=true, whether it would have been)</returns>
        public virtual bool Start(bool dryRun, bool force)
        {
            if (!dryRun)
            {
                OnActStart?.Invoke(this, EventArgs.Empty);
                if ( SoundPath != null) self.OnAction?.Invoke(this, new WeaponActionSoundEventArgs(SoundPath));
            }

            return true;
        }

        /// <summary>
        /// Interrupt this act
        /// </summary>
        /// <param name="dryRun">If true, don't actually interrupt, just return if you can</param>
        /// <param name="force">If true, interrupt the act by force</param>
        /// <returns>whether the act was interrupted (or if dryRun=true, whether it would have been)</returns>
        public abstract bool Interrupt(bool dryRun, bool force);

        protected void FinishAct()
        {
            self.FinishAction(this);
        }
    }
}

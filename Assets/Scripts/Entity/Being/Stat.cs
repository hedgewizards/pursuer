using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being
{
    public abstract class ModifiableStat
    {
        public abstract void Recalculate();

        protected EventHandler OnModifiersChanged;

        public class OnModifyEventArgs : EventArgs
        {
            public bool ReadOnly = false;
        }
    }

    public abstract class InvalidatingModifiableStat : ModifiableStat
    {
        protected bool invalid = true;

        public bool Invalid => invalid;
        public void Invalidate()
        {
            invalid = true;
        }

        public InvalidatingModifiableStat()
        {
            OnModifiersChanged += onModifiersChanged;
        }

        private void onModifiersChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }

    public class ScalarStat : InvalidatingModifiableStat
    {
        private float baseStat;
        private float cachedStat;

        public float Value
        {
            get
            {
                if (invalid) Recalculate();
                return cachedStat;
            }
        }


        public ScalarStat(float baseStat)
        {
            this.baseStat = baseStat;
            Recalculate();
        }

        public override void Recalculate()
        {
            var args = new OnModifyEventArgs(baseStat);
            OnModifyStat?.Invoke(this, args);

            cachedStat = args.FinalValue;
        }

        private EventHandler<OnModifyEventArgs> onModifyStat;
        public EventHandler<OnModifyEventArgs> OnModifyStat
        {
            get => onModifyStat;
            set
            {
                onModifyStat = value;
                Invalidate();
            }
        }


        new public class OnModifyEventArgs : InvalidatingModifiableStat.OnModifyEventArgs
        {

            private float baseStat;
            public float BaseValue => baseStat;



            /// <summary>
            /// Add this amount to your baseStat before scaling. this value can be negative
            /// </summary>
            public float FlatBonus
            {
                get => flatBonus;
                set
                {
                    flatBonus = value;
                }
            }
            float flatBonus = 0;


            /// <summary>
            /// At the end of the calculation, scale the value by this amount
            /// </summary>
            public float Scale
            {
                get => scale;
                set
                {
                    scale = value;
                }
            }
            float scale = 1;


            public float FinalValue
            {
                get
                {
                    return (baseStat + FlatBonus) * Scale;
                }
            }

            public OnModifyEventArgs(float baseStat)
            {
                this.baseStat = baseStat;
            }
        }
    }

    public class Stat<T> : InvalidatingModifiableStat
    {
        private T baseStat;
        protected T cachedStat;
        public T Value
        {
            get
            {
                return cachedStat;
            }
        }

        public Stat(T baseStat)
        {
            this.baseStat = baseStat;
            Recalculate();
        }

        public override void Recalculate()
        {
            var args = new OnModifyEventArgs(baseStat);
            OnModifyStat?.Invoke(this, args);

            cachedStat = args.Value;
        }

        public EventHandler<OnModifyEventArgs> OnModifyStat;
        
        new public class OnModifyEventArgs : ModifiableStat.OnModifyEventArgs
        {

            private T baseStat;
            public T BaseValue => baseStat;
            private T stat;
            public T Value
            {
                get => stat;
                set
                {
                    if (ReadOnly)
                    {
                        Debug.LogWarning("Tried to write to a ReadOnly stat");
                    }
                    else
                    {
                        stat = value;
                    }
                }
            }

            public OnModifyEventArgs(T baseStat)
            {
                stat = baseStat;
                this.baseStat = baseStat;
            }
        }
    }
}

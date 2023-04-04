using System;
using UnityEngine;

namespace Entity.Weapon
{
    public abstract class WeaponActionEventArgs : EventArgs
    {
        public enum EventType
        {
            AnimatorTrigger,
            AnimatorBool,
            AnimatorFloat,
            Effect,
            Sound
        }

        public abstract EventType Type { get; }
        public string Name;


        public WeaponActionEventArgs(string name)
        {
            Name = name;
        }

        public static WeaponActionEventArgs ReloadArgs = new WeaponActionEventArgsTrigger(Names.Reload);
        public static WeaponActionEventArgs ReloadComplete = new WeaponActionEventArgsTrigger(Names.ReloadComplete);

        public static WeaponActionEventArgs DeployArgs = new WeaponActionEventArgsTrigger(Names.Deploy);
        public static WeaponActionEventArgs HolsterArgs = new WeaponActionEventArgsTrigger(Names.Holster);
        public static WeaponActionEventArgs Attack1Args = new WeaponActionEventArgsTrigger(Names.Attack1);

        public class Names
        {
            public const string Reload = "Reload";
            public const string ReloadComplete = "ReloadComplete";
            public const string Deploy = "Deploy";
            public const string Holster = "Holster";
            public const string Attack1 = "Attack1";
        }
    }

    public class WeaponActionEventArgsTrigger : WeaponActionEventArgs
    {
        public override EventType Type => EventType.AnimatorTrigger;

        public WeaponActionEventArgsTrigger(string name) : base(name)
        {
        }
    }

    public class WeaponActionBoolEventArgs : WeaponActionEventArgs
    {
        public override EventType Type => EventType.AnimatorBool;
        public bool Bool;

        public WeaponActionBoolEventArgs(string name, bool _bool) : base(name)
        {
            Bool = _bool;
        }
    }

    public class WeaponActionFloatEventArgs : WeaponActionEventArgs
    {
        public override EventType Type => EventType.AnimatorFloat;
        public float Float;

        public WeaponActionFloatEventArgs(string name, float _float) : base(name)
        {
            Float = _float;
        }
    }

    public class WeaponActionSoundEventArgs : WeaponActionEventArgs
    {
        public override EventType Type => EventType.Sound;

        public WeaponActionSoundEventArgs(string name) : base(name)
        {
        }
    }

public class WeaponActionEffectEventArgs : WeaponActionEventArgs
    {
        public override EventType Type => EventType.Effect;
        public EffectDataStruct Data;
        public int EffectOptions;

        public WeaponActionEffectEventArgs(string name, EffectDataStruct data, int options = 0) : base(name)
        {
            Data = data;
            EffectOptions = options;
        }

        public bool CheckFlag(int flag)
        {
            return (EffectOptions & flag) > 0;
        }

        public class EffectOption
        {
            public const int PositionToWeaponOrigin = 1;
            public const int OriginToWeaponOrigin = 2;
            public const int ParentToWeaponOrigin = 4;
            public const int ParentCanReplace = 8;
        }
    }
}

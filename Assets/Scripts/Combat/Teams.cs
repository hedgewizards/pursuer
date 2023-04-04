using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public static class Teams
    {
        public const int Player = 2;
        public const int Enemy = 4;
        public const int Neutral = 8;

        public static class Masks
        {
            public const int PlayerNoTeamDamage = Enemy + Neutral;
            public const int EnemyNoTeamDamage = Player + Neutral;
            public const int DamageAll = ~0;
        }
    }
}
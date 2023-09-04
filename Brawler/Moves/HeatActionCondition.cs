using System;

namespace Brawler
{
    [Flags]
    public enum HeatActionCondition : ulong
    {
        None = 0,

        EnemyCriticalHealth =  1,
        FighterCriticalHealth = 2,

        EnemyDown = 4,
        EnemyNotDown = 8,

        FighterDown = 16,

        EnemyStunned = 32,
        FighterStunned = 64,

        EnemyMidAir = 128,
        FighterMidAir = 256,

        IsExHero = 512,

        EnemyStandingUp = 1024,
        FighterStandingUp = 4096,

        FighterHealthNotCritical = 8192,
        EnemyHealthNotCritical = 16384,

        FighterGrabbed = 32768,
        EnemyGrabbed = 65536,

        HeatFull = 131072

    }
}

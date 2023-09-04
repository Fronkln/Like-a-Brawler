using System;

namespace Brawler
{
    [Flags]
    internal enum TutorialModifier
    {
        None = 0,
        PlayerDontTakeDamage = 1,
        EnemyDontTakeDamage = 2,
        DontDepleteHeat = 4
    }
}

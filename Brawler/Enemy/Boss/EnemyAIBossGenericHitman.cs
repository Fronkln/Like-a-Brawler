using System;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAIBossGenericHitman : EnemyAIBoss
    {
        public override void Start()
        {
            base.Start();

            EvasionModule.BaseEvasionChance = 30;
            CounterAttacks.Add((RPGSkillID)1781);
        }

        public override bool ShouldDoCounterAttack()
        {
            if (RecentHitsWithoutDefensiveMove >= 2 || RecentDefensiveAttacks > 2)
                return true;

            return false;
        }
    }
}

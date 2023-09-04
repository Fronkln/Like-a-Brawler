using System;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAINanba : EnemyAIBoss
    {
        public override void Start()
        {
            base.Start();

            CounterAttacks.Add((RPGSkillID)1532);
        }

        public override bool ShouldDoCounterAttack()
        {
            if (RecentHitsWithoutAttack > 3)
                return true;

            return false;
        }
    }
}

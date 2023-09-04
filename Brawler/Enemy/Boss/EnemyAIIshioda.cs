using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal class EnemyAIIshioda : EnemyAIBoss
    {
        public override void Start()
        {
            base.Start();

            CounterAttacks.Add((RPGSkillID)1050);
            CounterAttacks.Add((RPGSkillID)1051);
        }

        public override bool DamageTransitCounter(BattleDamageInfoSafe dmg)
        {
            bool countered = false;

            countered = DamageTransitCounterIshioda(dmg);


            if (!countered)
                return base.DamageTransitCounter(dmg);
            else
                return true;
        }

        public bool DamageTransitCounterIshioda(BattleDamageInfoSafe dmg)
        {
            if (BlockModule.BlockProcedure && BlockModule.RecentlyBlockedHits >= 3)
            {
                ExecuteCounterAttack(RPGSkillID.boss_ishioda_counter, true);
                return true;
            }


            return false;
        }

        public override bool ShouldDoCounterAttack()
        {
            if (RecentHitsWithoutAttack > 4)
                return true;

            return false;
        }

        public override Vector2 GetBlockRange()
        {
            return new Vector2(5, 7);
        }
    }
}

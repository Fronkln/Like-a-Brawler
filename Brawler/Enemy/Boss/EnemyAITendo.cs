using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal class EnemyAITendo : EnemyAIBoss
    {
        public override void Start()
        {
            base.Start();

            CounterAttacks.Add((RPGSkillID)1793);
        }

        protected override void OnStartGettingUp()
        {
            if (DistanceToPlayer <= 2f)
                ExecuteCounterAttack((RPGSkillID)1794, true);
        }
    }
}

using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal class EnemyAITendo : EnemyAIBoss
    {
        private bool m_finalHactPlayed = false;

        public override void Start()
        {
            base.Start();

            CounterAttacks.Add((RPGSkillID)1793);
        }


        public override void CombatUpdate()
        {
            base.CombatUpdate();

            if (!m_finalHactPlayed)
                if (Character.IsHPBelowRatio(0.25f))
                {
                    DoHAct((TalkParamID)12978, BrawlerBattleManager.KasugaChara.GetMatrix());
                    m_finalHactPlayed = true;
                }
        }

        protected override void OnStartGettingUp()
        {
            if (DistanceToPlayer <= 2f)
                ExecuteCounterAttack((RPGSkillID)1799, true);

        }

        public override bool CanDieOnHAct()
        {
            return false;
        }
    }
}

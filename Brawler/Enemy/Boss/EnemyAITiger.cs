using System;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAITiger : EnemyAIBoss
    {
        private bool m_usedDownHAct;

        private int m_rngCounter = 3;

        public override void Start()
        {
            base.Start();

            CounterAttacks.Add((RPGSkillID)836);
            CounterAttacks.Add((RPGSkillID)837);
            CounterAttacks.Add((RPGSkillID)1498);
            CounterAttacks.Add((RPGSkillID)1499);

            BrawlerPlayer.OnPlayerStartGettingUp += OnPlayerStartGettingUp;
            OnCounterAttack += OnCounterAttackDeleg;
        }

        public override bool ShouldDoCounterAttack()
        {
            return RecentHitsWithoutAttack > m_rngCounter;
        }


        private void OnCounterAttackDeleg()
        {
            m_rngCounter = new Random().Next(3, 7);
        }

        public void OnPlayerStartGettingUp()
        {
            if (m_usedDownHAct)
                return;

            if (Vector3.Distance(Chara.Get().GetPosCenter(), BrawlerBattleManager.KasugaChara.GetPosCenter()) < 5)
            {
                bool shouldDoDownHact = BrawlerBattleManager.BattleTime > 10 && new Random().Next(0, 100) <= 45;

                if(shouldDoDownHact)
                {
                    DoHAct((TalkParamID)12942, Matrix4x4.Default);
                    m_usedDownHAct = true;
                }
            }
        }
    }
}

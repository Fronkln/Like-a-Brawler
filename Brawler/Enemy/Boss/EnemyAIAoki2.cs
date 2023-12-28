using System;
using System.Runtime.InteropServices;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAIAoki2 : EnemyAIBoss
    {
        private int m_curPhase = 0;

        public override void Start()
        {
            base.Start();

            CounterAttacks.Add((RPGSkillID)1791);
            EvasionModule.BaseEvasionChance = 35;

            OnGetUp += OnGettingUp;
        }

        public void OnGettingUp()
        {
            if (m_curPhase == 0)
            {
                ExecuteCounterAttack((RPGSkillID)1792, false);
                return;
            }
            else
            {
                if (new Random().Next(0, 101) < 50)
                    ExecuteCounterAttack((RPGSkillID)1798, false);
            }

        }

        public override void OnPlayerGettingUp()
        {
            base.OnPlayerGettingUp();

            if (m_curPhase > 0)
            {
                if (!m_performedHacts.Contains((TalkParamID)13013))
                    if (DistanceToPlayer < 5)
                        if (new Random().Next(0, 101) <= 65)
                            DoHAct((TalkParamID)13013, BrawlerBattleManager.KasugaChara.GetMatrix());
            }

        }

        public override void OnHit()
        {
            base.OnHit();


            if(m_curPhase == 0)
            {
                if (Character.IsHPBelowRatio(0.7f))
                {
                    DragonEngine.Log("will enter phase 2 soon");
                }
            }

        }

        public override void CombatUpdate()
        {
            base.CombatUpdate();

            if(m_curPhase == 0)
            {
                if (Character.IsHPBelowRatio(0.6f))
                {
                    EnterSecondPhase();
                    return;
                }
            }
        }

        public override void OnStartAttack()
        {
            base.OnStartAttack();
        }

        private void EnterSecondPhase()
        {
            CounterAttacks.Clear();

            CounterAttacks.Add((RPGSkillID)1797);
            EvasionModule.BaseEvasionChance = 15;

            //Switch to SUD
            m_curPhase = 1;
            Character.GetStatus().GetBattleAI().SwitchEnemyIDSet(560);


            DoHAct((TalkParamID)13000, BrawlerBattleManager.KasugaChara.GetMatrix());

            DragonEngine.Log("entered phase 2");
        }

        public override bool CanDieOnHAct()
        {
            return false;
        }
    }
}

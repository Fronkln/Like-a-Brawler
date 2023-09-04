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

            OnGetUp += OnGettingUp;
        }

        public  void OnGettingUp()
        {
            if (m_curPhase == 1)
                return;

            ExecuteCounterAttack((RPGSkillID)1792, false);
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

            //Switch to SUD
            m_curPhase = 1;
            Character.GetStatus().GetBattleAI().SwitchEnemyIDSet(560);


            DoHAct((TalkParamID)13000, BrawlerBattleManager.KasugaChara.GetPosture().GetRootMatrix());

            DragonEngine.Log("entered phase 2");
        }
    }
}

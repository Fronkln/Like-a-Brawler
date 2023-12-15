using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal class EnemyAIHan : EnemyAIBoss
    {
        private bool m_isMortal = false;
        private bool m_mortalProcedure = false;

        private float m_nextMortalAttackTime = 0;
        private int m_numMortalAttacks = 0;

        private int m_mortalPhase = 0;

        private readonly RPGSkillID[] m_wpkSwayAtks = new RPGSkillID[]
        {
            (RPGSkillID)1795,
            (RPGSkillID)1796
        };

        public override void Start()
        {
            base.Start();

            CounterAttacks.Add((RPGSkillID)1525);
        }

        public override void CombatUpdate()
        {
            base.CombatUpdate();


            switch(m_mortalPhase)
            {
                case 0:
                    if (Character.IsHPBelowRatio(0.6f) && !m_isMortal)
                    {
                        EnterMortal();
                        m_mortalPhase = 1;
                    }
                    break;

                case 1:
                    if (Character.IsHPBelowRatio(0.3f) && !m_isMortal)
                    {
                        EnterMortal();
                        m_mortalPhase = 2;
                    }
                    break;

            }

            if (m_isMortal)
            {
                if (m_mortalProcedure)
                {
                    m_nextMortalAttackTime -= DragonEngine.deltaTime;
                    Character.GetStatus().SetSuperArmor(true, false);

                    if (m_nextMortalAttackTime <= 0 && !Chara.Get().GetMotion().RequestedAnimPlaying())
                    {
                        if (m_numMortalAttacks > 0)
                        {
                            PerformMortalAttack();
                            m_nextMortalAttackTime = 3.5f;

                            m_numMortalAttacks--;
                        }
                        else
                        {
                            ExitMortal();
                        }
                    }
                }
            }
            else
            {
            }
        }

        private void PerformMortalAttack()
        {
            Chara.Get().Components.EffectEvent.Get().PlayEvent((EffectEventCharaID)206);
            SoundManager.PlayCue(SoundCuesheetID.battle_common, 5, 0);

            RPGSkillID counterAttack = m_wpkSwayAtks[new Random().Next(0, m_wpkSwayAtks.Length)];

            BattleTurnManager.ForceCounterCommand(Character, BrawlerBattleManager.Kasuga, counterAttack);

            DragonEngine.Log("mortal... attack!");
        }

        public void EnterMortal()
        {
            m_isMortal = true;
            m_nextMortalAttackTime = 0;
            m_numMortalAttacks = new Random().Next(2, 4);

            SoundManager.PlayCue(5660, 1, 0);
            Chara.Get().Components.EffectEvent.Get().PlayEvent((EffectEventCharaID)86);
            ExecuteCounterAttack((RPGSkillID)1794, false);
            new DETaskTime(0.1f, delegate { m_mortalProcedure = true; });

            DragonEngine.Log("GOING MORTAL!!");
        }

        public void ExitMortal()
        {
            m_isMortal = false;
            m_mortalProcedure = false;
            Character.GetStatus().SetSuperArmor(false, false);
            Chara.Get().Components.EffectEvent.Get().StopEvent((EffectEventCharaID)86, true);
        }
    }
}

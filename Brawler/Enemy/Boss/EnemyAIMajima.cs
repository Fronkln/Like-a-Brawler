using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    //Recent AI changes broke majimas shitton of sidestepping code
    //Reimplement!
    internal class EnemyAIMajima : EnemyAIBoss
    {
        public override MotionID TauntMotion => (MotionID)17084;

        private bool m_isBunshin = false;
        private bool m_bunshinsSpawnedAtAll = false;

        private int m_phase = 0;

        public override void InitResources()
        {
            //gv_fighter_majima_extra
            SoundManager.LoadCuesheet(5572);

            //Boss: Majima_attack
            CounterAttacks.Add((RPGSkillID)1066);

            //Evasive Slash
            CounterAttacks.Add((RPGSkillID)1069);
        }

        public override void LateStart()
        {
            base.LateStart();

            m_isBunshin = Chara.Get().Attributes.soldier_data_id != CharacterNPCSoldierPersonalDataID.yazawa_btl12_0020_000_1;
        }

        public override void CombatUpdate()
        {
            base.CombatUpdate();

            if (!m_isBunshin)
            {
                if (!m_bunshinsSpawnedAtAll)
                    if (EnemyManager.EnemyAIs.Where(x => x.Value is EnemyAIMajima).Count() > 1)
                        m_bunshinsSpawnedAtAll = true;

                if (m_phase == 0)
                {
                    if (Character.IsHPBelowRatio(0.25f))
                    {
                        OnChangePhase(1);
                        return;
                    }
                }
                else if (m_phase == 1)
                {

                }
            }
        }

        private void OnChangePhase(int phase)
        {
            m_phase = phase;

            if(phase == 1)
            {
                EvasionModule.BaseEvasionChance = 40;

                if(!m_isBunshin && !m_bunshinsSpawnedAtAll)
                    ExecuteCounterAttack((RPGSkillID)1315, false);
            }
        }
        /*
        public override bool ShouldBlockAttack(BattleDamageInfoSafe dmgInf)
        {
            return false;
        }
        */
        public override bool DoSpecial(BattleDamageInfoSafe inf)
        {
            if(IsBeingSpammed())
            {
                Sway();
                return true;
            }

            return false;
        }
    }
}

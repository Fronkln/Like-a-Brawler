using System;
using System.Collections.Generic;
using DragonEngineLibrary;
using DragonEngineLibrary.Service;

namespace Brawler
{
    internal class EnemyAISawashiro : EnemyAIBoss
    {
        //SUD > WPB
        private int m_prevPhase = -1;
        private List<RPGSkillID> m_curPhaseCounters = new List<RPGSkillID>();

        private int GetPhase()
        {
            AssetArmsCategoryID rightWep = Asset.GetArmsCategory(Character.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get().AssetID);

            if (rightWep == AssetArmsCategoryID.invalid)
                return 0;
            else
                return 1;
        }

        private void OnPhaseChange(int phase)
        {
            foreach (RPGSkillID counterSkill in m_curPhaseCounters)
                CounterAttacks.Remove(counterSkill);

            m_curPhaseCounters.Clear();

            switch (phase)
            {
                case 0:
                    CounterAttacks.Add((RPGSkillID)1773);
                    m_curPhaseCounters.Add((RPGSkillID)1773);
                    break;
                case 1:
                    CounterAttacks.Add((RPGSkillID)1772);
                    m_curPhaseCounters.Add((RPGSkillID)1772);

                    EvasionModule.CounterAttackPatience = 7;
                    break;
            }
        }

        public override void CombatUpdate()
        {
            base.CombatUpdate();

            int phase = GetPhase();

            if (m_prevPhase != phase)
                OnPhaseChange(phase);
            
            m_prevPhase = phase;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAISawashiro2 : EnemyAIBoss
    {
        //WPE->WPA-> WPA & WPE

        private int m_prevPhase = -1;
        private List<RPGSkillID> m_curPhaseCounters = new List<RPGSkillID>();

        private int GetPhase()
        {
            AssetArmsCategoryID rightWep = Asset.GetArmsCategory(Character.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get().AssetID);
            AssetArmsCategoryID leftWep = Asset.GetArmsCategory(Character.GetWeapon(AttachmentCombinationID.left_weapon).Unit.Get().AssetID);

            if (rightWep == AssetArmsCategoryID.E)
            {
                if (leftWep == AssetArmsCategoryID.A)
                    return 2;
                else
                    return 0;
            }
            else if (rightWep == AssetArmsCategoryID.A)
                return 1;

            return -1;
        }

        private void OnPhaseChange(int phase)
        {
            foreach (RPGSkillID counterSkill in m_curPhaseCounters)
                CounterAttacks.Remove(counterSkill);

            m_curPhaseCounters.Clear();

            switch(phase)
            {
                case 0:
                    CounterAttacks.Add((RPGSkillID)841);
                    m_curPhaseCounters.Add((RPGSkillID)841);
                    break;
            }
        }

        public override void Start()
        {
            base.Start();
        }

        public override void CombatUpdate()
        {
            base.CombatUpdate();

            int phase = GetPhase();

            if (m_prevPhase != phase)
                OnPhaseChange(phase);

            m_prevPhase = phase;
        }


        public override bool DoFinisher(BattleDamageInfoSafe dmgInf)
        {
            if (GetPhase() == 1)
            {
                //y7b1190_saw_wpa_ich_ded
                DoHAct((TalkParamID)12936, Chara.Get().Transform.Position);
                return true;
            }

            return false;
        }
    }
}

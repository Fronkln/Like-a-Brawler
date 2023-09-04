using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal enum KiryuStyle
    {
        None,
        Brawler,
        Rush,
        Crash,
        Legend
    }

    internal class EnemyAIKiryu : EnemyAIBoss
    {
        //0 = Brawler (99)
        //1 = Rush (89)
        //2 = Crash (98)
        //3 = Legend (91)
        private KiryuStyle m_curStyle = KiryuStyle.None;

        private TalkParamID m_brawlerAntiGuard = (TalkParamID)12908;

        private TalkParamID[] m_playerFinishers = new TalkParamID[4]
        {
            (TalkParamID)12909,
            (TalkParamID)12910,
            (TalkParamID)12911,
            (TalkParamID)12912,
        };

        public override void CombatUpdate()
        {
            base.CombatUpdate();

            StyleUpdate();
        }

        private void StyleUpdate()
        {
            KiryuStyle style = GetCurrentStyle();

            if (m_curStyle != style)
                OnStyleSwitch(style);

            switch (m_curStyle)
            {
                case KiryuStyle.Brawler:
                    BrawlerUpdate();
                    break;
                case KiryuStyle.Rush:
                    RushUpdate();
                    break;
                case KiryuStyle.Crash:
                    CrashUpdate();
                    break;
                case KiryuStyle.Legend:
                    LegendUpdate();
                    break;
            }
        }

        private void BrawlerUpdate()
        {
            Character chara = Chara.Get();

            if (BrawlerPlayer.GuardTime > 4.5f)
                if (!chara.GetMotion().RequestedAnimPlaying())
                    if (DistanceToPlayer <= 2)
                        if (!m_performedHacts.Contains(m_brawlerAntiGuard))
                            DoHAct(m_brawlerAntiGuard, chara.Transform.Position);
        }

        protected override void OnStartGettingUp()
        {
            if (DistanceToPlayer <= 2f)
            {
                if (m_curStyle == KiryuStyle.Rush)
                    ExecuteCounterAttack((RPGSkillID)1761, true);
            }
        }

        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        //ONLY DO THIS ON THE STORY KIRYU!!!
        public override bool DamageTransit(BattleDamageInfoSafe dmg)
        {
            return base.DamageTransit(dmg);

            if (WouldDieToDamage(dmg))
            {
                //y7brawler_kru_fin
                DoHAct((TalkParamID)12913, Chara.Get().GetPosture().GetRootMatrix());

                //dont play battle end effect get straight to the results
                new DETaskList(new DETask[]
                {
                    new DETaskNextFrame(),
                    new DETaskNextFrame(),
                    new DETask(delegate { return !HActManager.IsPlaying(); }, delegate { BattleTurnManager.ChangePhase(BattleTurnManager.TurnPhase.End); })
                });

                return true;
            }
        }

        public override bool DamageTransitGuard(BattleDamageInfoSafe dmg)
        {
            if (m_curStyle == KiryuStyle.Rush || m_curStyle == KiryuStyle.Crash || m_curStyle == KiryuStyle.Legend)
                return false;

            return base.DamageTransitGuard(dmg);
        }

        public override bool DamageTransitCounter(BattleDamageInfoSafe dmg)
        {
            bool countered = false;

            switch(m_curStyle)
            {
                case KiryuStyle.Brawler:
                    countered = DamageTransitCounterBrawler(dmg);
                    break;
                case KiryuStyle.Rush:
                    countered = DamageTransitCounterRush(dmg);
                    break;
                case KiryuStyle.Legend:
                    countered = DamageTransitCounterLegend(dmg);
                    break;
            }

            if (!countered)
                return base.DamageTransitCounter(dmg);
            else
                return true;
        }

        private bool DamageTransitCounterBrawler(BattleDamageInfoSafe dmg)
        {
            if (BlockModule.BlockProcedure && RecentHitsWithoutAttack > 4)
            {
                ExecuteCounterAttack((RPGSkillID)1762, true);
                return true;
            }

            return false;
        }

        private bool DamageTransitCounterRush(BattleDamageInfoSafe dmg)
        {
            Random rnd = new Random();
            int swayAtkChance = rnd.Next(0, 101);

            if(swayAtkChance <= 12)
            {
                RPGSkillID rndAttack = (rnd.Next(0, 101) <= 49 ? 
                    RPGSkillID.boss_kiryu_rush_swy_atk_l : RPGSkillID.boss_kiryu_rush_swy_atk_r);

                ExecuteCounterAttack(rndAttack, true);
                return true;
            }
            else if(swayAtkChance <= 35)
            {
                Chara.Get().HumanModeManager.ToSway();
                return true;
            }

            return false;
        }

        private bool DamageTransitCounterLegend(BattleDamageInfoSafe dmg)
        {
            if (IsLegendCounterReady())
            {
                if (dmg.IsDirect)
                {
                    if (dmg.IsSyncStartDmg)
                        ExecuteCounterAttack((RPGSkillID)1228, true);
                    else
                        ExecuteCounterAttack((RPGSkillID)1063, true);
                    
                    return true;
                }
            }
            else
            {
                if (BlockModule.BlockProcedure && BlockModule.RecentlyBlockedHits >= 4)
                {
                    ExecuteCounterAttack(RPGSkillID.boss_kiryu_legend_atk_d, true);
                    return true;
                }
            }

            return false;
        }

        private bool IsLegendCounterReady()
        {
            MotionID counterGmt = Chara.Get().GetMotion().GmtID;

            //Dragon's Gaze
            return 
                counterGmt == MotionID.E_KRL_BTL_SUD_komaki_st || 
                counterGmt == MotionID.E_KRL_BTL_SUD_komaki_lp;
        }

        private void RushUpdate()
        {

        }

        private void CrashUpdate()
        {

        }

        private void LegendUpdate()
        {

        }

        public void OnStyleSwitch(KiryuStyle newStyle)
        {
            m_curStyle = newStyle;


            if (m_curStyle == KiryuStyle.Crash || m_curStyle == KiryuStyle.Legend)
                HeatActionDamageResist = 0;
        }

        private KiryuStyle GetCurrentStyle()
        {
            switch (Character.GetStatus().GetArts())
            {
                default:
                    return KiryuStyle.Brawler;
                case 99:
                    return KiryuStyle.Brawler;
                case 89:
                    return KiryuStyle.Rush;
                case 98:
                    return KiryuStyle.Crash;
                case 91:
                    return KiryuStyle.Legend;
            }
        }

        public override bool DoFinisher(BattleDamageInfoSafe dmgInf)
        {
            DoHAct(m_playerFinishers[(int)m_curStyle - 1], Chara.Get().Transform.Position);
            return true;
        }
    }
}

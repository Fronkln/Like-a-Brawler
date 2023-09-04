using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAIMabuchi : EnemyAIBoss
    {
        public override void Start()
        {
            base.Start();

            CounterAttacks.Add((RPGSkillID)844);
        }

        public bool IsCounterStance()
        {
            Character chara = Chara.Get();

            MotionID id = chara.GetMotion().GmtID;
            ECCharacterEffectEvent effectComponent = chara.Components.EffectEvent;

            bool isCounterAura = effectComponent.IsPlayingEvent(EffectEventCharaID.boss_mabuchi_i_kamae) || effectComponent.IsPlayingEvent(EffectEventCharaID.boss_mabuchi_i_kamae_en);
            bool isCounterGmt = id == MotionID.E_MAB_BTL_WPI_grd_st || id == MotionID.E_MAB_BTL_WPI_grd_lp;

            //Resolute counter
            return isCounterAura || isCounterGmt;
        }

        public bool IsMad()
        {
            ECCharacterEffectEvent effectComponent = Chara.Get().Components.EffectEvent;
            return effectComponent.IsPlayingEvent(EffectEventCharaID.boss_mabuchi_lp);
        }

        public override bool CanDodge()
        {
            return !IsCounterStance();
        }

        public override bool DamageTransitCounter(BattleDamageInfoSafe dmg)
        {
            MotionID id = Chara.Get().GetMotion().GmtID;

            //Resolute counter
            if(IsCounterStance())
            {
             //   ExecuteCounterAttack(RPGSkillID.boss_kiryu_atk_c, true);

                ExecuteCounterAttack((RPGSkillID)1782, false);
                return true;
            }

            return base.DamageTransitCounter(dmg);
        }

        public override bool ShouldDoCounterAttack()
        {
            if (RecentHitsWithoutDefensiveMove >= 4 || RecentDefensiveAttacks > 3)
                return true;

            return false;
        }

        public override bool CanDoCounterAttack()
        {
            return true;
        }
    }
}

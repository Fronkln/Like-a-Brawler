using System;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAIUshio : EnemyAIBoss
    {
        public override MotionID TauntMotion => (MotionID)17085;
        private const TalkParamID m_ushioHact = (TalkParamID)12897;

        protected override float BOSS_TAUNT_COOLDOWN => 12.5f;

        public override void Start()
        {
            base.Start();

            CounterAttacks.Add(RPGSkillID.boxer_atk_b);
        }

        public override void CombatUpdate()
        {
            base.CombatUpdate();
        }

        //Ushio cannot knock down Kasuga. But he can inflict heavy flinch damage
        public override bool ShouldTaunt()
        {
            if (!BrawlerBattleManager.Kasuga.Character.IsAnimDamage())
                return false;

            return BrawlerBattleManager.KasugaChara.GetMotion().PlayInfo.tick_gmt_now_ > 1000; //1 seconds
        }

        public override void OnStartAttack()
        {
            if (!m_performedHacts.Contains(m_ushioHact))
                if (Character.IsHPBelowRatio(0.4f))
                    DoHAct(m_ushioHact, Vector4.zero);
        }
    }
}

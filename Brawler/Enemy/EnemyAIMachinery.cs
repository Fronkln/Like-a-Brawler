using System;
using System.Runtime.CompilerServices;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAIMachinery : EnemyAIBoss
    {
        private bool m_driverOutDoOnce = false;

        public override void Start()
        {
            base.Start();

            Character.GetStatus().SetSuperArmor(true, true);
            EvasionModule.BaseEvasionChance = 0;
            BlockModule.BlockChance = 0;

            BattleTurnManager.OverrideAttackerSelection(EnemyManager.OnAttackerSelectMachineryBattle);
        }



        public override void OnHit()
        {
            base.OnHit();

            //Prevent rare softlock
            if (Chara.Get().GetBattleStatus().CurrentHP <= 10)
                Chara.Get().ToDead();
        }

        public override void CombatUpdate()
        {
            base.CombatUpdate();
            
            if(!m_driverOutDoOnce)
                if(BrawlerBattleManager.Enemies.Length > 1)
                {
                    m_driverOutDoOnce = true;
                    OnDriverOut();
                }

            //Locking in gets enabled if the enemy is thrown out of the crane
            BrawlerBattleManager.DisableTargetingOnce = BrawlerBattleManager.Enemies.Length <= 1;
        }

        public virtual void OnDriverOut()
        {
            DragonEngine.Log("Driver OUT!");
            //BrawlerBattleManager.Enemies[1].GetBattleAI().SwitchEnemyIDSet(557);
        }
    }
}

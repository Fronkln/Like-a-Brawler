using DragonEngineLibrary;
using System;


namespace Brawler
{
    internal class EnemyAIBossGenericWPY : EnemyAIBoss
    {
        public override void Start()
        {
            base.Start();

            EvasionModule.BaseEvasionChance = 25;
            CounterAttacks.Add((RPGSkillID)789);
        }
    }
}

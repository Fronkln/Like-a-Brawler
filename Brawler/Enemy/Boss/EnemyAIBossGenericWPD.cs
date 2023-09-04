using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAIBossGenericWPD : EnemyAIBoss
    {
        public override void Start()
        {
            base.Start();

            CounterAttacks.Add((RPGSkillID)1770);
        }
    }
}

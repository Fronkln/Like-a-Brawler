using DragonEngineLibrary;

namespace Brawler
{
    /// <summary>
    /// Generic boss AI for weapon G (hammer) enemies.
    /// </summary>
    internal class EnemyAIBossGenericWPG : EnemyAIBoss
    {
        public override void Start()
        {
            base.Start();

            //Foreman: Upswing Smash
            CounterAttacks.Add(RPGSkillID.job_wreckingyard_skill_08);
            //Foreman: Hammer Swing
            CounterAttacks.Add(RPGSkillID.job_wreckingyard_skill_04);
        }
    }
}

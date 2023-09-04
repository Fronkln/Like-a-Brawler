using System;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyEvasionModule : EnemyModule
    {
        public int BaseEvasionChance = 10;

        public float LastEvasionTime = 9999;
        public int RecentlyEvadedAttacks = 0;

        public float CurrentCounterAttackCooldown = 0;
        public float CounterAttackCooldown = 4.5f;

        public bool IsCounterAttacking = false;

        public int CounterAttackPatience = 5;


        //Bosses get an evasion boost after they recently get up
        private float m_evasionBoostDuration = 0;

        public override void Update()
        {
            base.Update();

            if (LastEvasionTime > 1.8f)
                RecentlyEvadedAttacks = 0;

            float delta = DragonEngine.deltaTime;

            if (m_evasionBoostDuration > 0)
                m_evasionBoostDuration -= delta;

            CounterAttackCooldown -= delta;
            LastEvasionTime += delta;
        }


        public void OnGetUp()
        {
            if (AI.IsBoss())
                m_evasionBoostDuration = 1.5f;
        }

        public void DoEvasion()
        {
            AI.Chara.Get().HumanModeManager.ToSway();
            LastEvasionTime = 0;
            RecentlyEvadedAttacks++;
            AI.RecentHitsWithoutDefensiveMove = 0;
            AI.RecentDefensiveAttacks++;
            AI.RecentHitsWithoutAttack++;

            //24.05.2023: What???
            //if (AI.ShouldDoCounterAttack())
               // DoCounterAttack();
        }

        public bool ShouldEvade(BattleDamageInfoSafe inf)
        {

            if (AI.BlockModule.BlockProcedure)
                return false;

            if (AI.IsAttacking())
                return false;

            if (AI.Character.IsSync())
                return false;

            if (AI.Character.GetStatus().IsSuperArmor() || IsCounterAttacking)
                return false;

            /*
            if (AI.BlockModule.ShouldBlockAttack(inf))
                return false;
            */

            bool firstEvasion = ShouldEvadeFirstAttack();

            if (firstEvasion)
                return firstEvasion;
            else
                //Make this a proper algorithm later
                return new Random().Next(0, 101) <= (BaseEvasionChance * (m_evasionBoostDuration > 0 ? 2f : 1f));
        }

        //First attack = Hasnt got hit since 2.5 seconds
        public bool ShouldEvadeFirstAttack()
        {
            const float h_firstEvasionChance = 40;

            float chance = h_firstEvasionChance;

            if (!AI.Character.IsBoss())
                chance *= 0.5f;

            if (AI.LastHitTime < 2.5f)
                return false;
            else
                return new Random().Next(0, 101) <= chance;
        }

        public virtual bool ShouldDoCounterAttack()
        {
            return CurrentCounterAttackCooldown <= 0 && AI.RecentDefensiveAttacks >= CounterAttackPatience;
        }

        public virtual void DoCounterAttack(bool immediate = false)
        {
            CurrentCounterAttackCooldown = CounterAttackCooldown;
            //This part is important because this variable only exists for defensive moves
            AI.RecentDefensiveAttacks = 0;
            AI.RecentHitsWithoutDefensiveMove = 0;
            AI.RecentHitsWithoutAttack = 0;

            bool wasSuper = AI.Character.GetStatus().IsSuperArmor();

            if (!wasSuper)
                AI.Character.GetStatus().SetSuperArmor(true);

            DETaskList list = new DETaskList(new DETask[]
            {
               new DETaskTime(immediate ? 0 : 0.45f, delegate
               {
                   Console.WriteLine("Time for a devastating counter attack!");
                   BattleTurnManager.ForceCounterCommand(AI.Character, BrawlerBattleManager.Kasuga, AI.CounterAttacks[ new Random().Next(0, AI.CounterAttacks.Count)]);

                   AI.Chara.Get().Components.EffectEvent.Get().PlayEvent((EffectEventCharaID)206);
                   SoundManager.PlayCue(SoundCuesheetID.battle_common, 5, 0);

                   //Grant superarmor while countering.
                   if(!wasSuper)
                   {
                       AI.Character.GetStatus().SetSuperArmor(true);
                       IsCounterAttacking = true;

                       new DETask(delegate{return !AI.Chara.Get().HumanModeManager.IsAttack(); }, delegate
                       {
                            IsCounterAttacking = false;
                            AI.Character.GetStatus().SetSuperArmor(false);
                       });
                   }
               }, false),
            });
        }
    }
}

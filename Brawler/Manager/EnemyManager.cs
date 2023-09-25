using System;
using System.Linq;
using System.Collections.Generic;
using DragonEngineLibrary;

namespace Brawler
{
    internal static class EnemyManager
    {
        public static Dictionary<uint, EnemyAI> EnemyAIs = new Dictionary<uint, EnemyAI>();

        internal static Fighter _OverrideNextAttackerOnce = null;


        //CALLBACK STUFF
        private static bool m_playerGettingUpDoOnce;
        private static bool m_playerDownDoOnce;

        private static bool m_attackStartDoOnce = false;

        private static void UpdateCallback()
        {

            if(BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Action)
            {
                if(BattleTurnManager.CurrentActionStep != BattleTurnManager.ActionStep.Action)
                    m_attackStartDoOnce = false;
                else
                {
                    if(!m_attackStartDoOnce)
                    {
                        EnemyAI ai = GetAI(BrawlerBattleManager.CurrentAttacker);

                        if (ai == null)
                            return;

                        ai.OnStartAttack();
                        m_attackStartDoOnce = true;
                    }
                }
            }


            if (BrawlerPlayer.Info.IsGettingUp)
            {
                if (!m_playerGettingUpDoOnce)
                {
                    foreach (var kv in EnemyAIs)
                        kv.Value.OnPlayerGettingUp();

                    m_playerGettingUpDoOnce = true;
                }
            }
            else
                if (m_playerGettingUpDoOnce)
                    m_playerGettingUpDoOnce = false;


            if (BrawlerPlayer.Info.IsDown)
            {
                if (!m_playerDownDoOnce)
                {
                    foreach (var kv in EnemyAIs)
                        kv.Value.OnPlayerDown();

                    m_playerDownDoOnce = true;
                }
            }
            else
                if (m_playerDownDoOnce)
                m_playerDownDoOnce = false;
        }

        public static EnemyAI GetAI(Fighter enemy)
        {
            if (enemy == null || !enemy.IsValid() || !EnemyAIs.ContainsKey(enemy.Character.UID))
                return null;
            return EnemyAIs[enemy.Character.UID];
        }

        public static void Update()
        {
            if (BrawlerBattleManager.Enemies.Length <= 0 && BattleTurnManager.CurrentPhase >= BattleTurnManager.TurnPhase.Action)
                return;

            UpdateCallback();


            foreach (Fighter fighter in BrawlerBattleManager.Enemies)
                if (!EnemyAIs.ContainsKey(fighter.Character.UID) && fighter.GetStatus().GetArts() > 0)
                {
                    EnemyAI createdAI = null;

                    if (!fighter.IsBoss())
                        createdAI = CreateEnemyAI(fighter);
                    else
                        createdAI = CreateBossAI(fighter);

                    EnemyAIs.Add(fighter.Character.UID, createdAI);

                    createdAI.Start();
                    createdAI.InitResources();
                }

            EnemyAIs = EnemyAIs.Where(x => x.Value.Chara.IsValid() && !x.Value.Character.IsDead()).ToDictionary(x => x.Key, x => x.Value);

            uint attacker = BattleTurnManager.SelectedFighter.UID;

            foreach (var kv in EnemyAIs)
            {
                kv.Value.Update();

                if (kv.Value.Character == BrawlerBattleManager.CurrentAttacker)
                {
                    BattleTurnManager.ActionStep phase = BattleTurnManager.CurrentActionStep;

                    if (phase == BattleTurnManager.ActionStep.Action || phase == BattleTurnManager.ActionStep.Ready)
                        if (!kv.Value.Character.IsDead())
                        {
                            if (kv.Key == attacker)
                                kv.Value.TurnUpdate(phase);
                        }
                }
            }
        }

        public static Fighter OnAttackerSelectNormalBattle(bool readOnly, bool getNextFighter)
        {
            if (Mod.DisableAttacksFromAI)
                return FighterManager.GetFighter(0);

            if (!FighterManager.IsBrawlerMode() || Debug.DontEraseRPGUI)
                return null;

            if(_OverrideNextAttackerOnce != null)
            {
                Fighter attacker = _OverrideNextAttackerOnce;
                _OverrideNextAttackerOnce = null;

                return attacker;
            }

            Fighter chosenEnemy = null;
            Fighter[] allEnemies = FighterManager.GetAllEnemies().Where(x => !x.IsDead()).ToArray();

            if (allEnemies.Length <= 0)
                return new Fighter((IntPtr)0);

            if (allEnemies.Length == 1)
                chosenEnemy = allEnemies[0];

            Random rnd = new Random();

            if (allEnemies.Length > 1)
            {
                allEnemies = allEnemies.OrderBy(x => Vector3.Distance((Vector3)FighterManager.GetFighter(0).Character.Transform.Position, (Vector3)x.Character.Transform.Position)).ToArray();
                chosenEnemy = allEnemies[rnd.Next(0, 2)]; //focus on the nearest three enemies.
            }
            else
                chosenEnemy = allEnemies[rnd.Next(0, allEnemies.Length)];

            BrawlerBattleManager.CurrentAttacker = chosenEnemy;

            return chosenEnemy;
        }

        public static Fighter OnAttackerSelectMachineryBattle(bool readOnly, bool getNextFighter)
        {
            if (BrawlerBattleManager.Enemies.Length > 1)
                return BrawlerBattleManager.Enemies[1];
            else if (BrawlerBattleManager.Enemies.Length == 1)
                return BrawlerBattleManager.Enemies[0];

            return null;
        }

        private static EnemyAI CreateEnemyAI(Fighter enemy)
        {
            EnemyAI ai = null;

            switch(enemy.Character.Attributes.enemy_id)
            {
                case BattleRPGEnemyID.yazawa_boss_ushio_c01:
                    ai = new EnemyAIUshio();
                    break;
                case BattleRPGEnemyID.yazawa_boss_kantoku_c05:
                    ai = new EnemyAIBossGenericWPG();
                    break;
                case BattleRPGEnemyID.yazawa_boss_kantoku_c061:
                    ai = new EnemyAIBoss();
                    break;
                case BattleRPGEnemyID.yazawa_boss_kantoku_c062:
                    ai = new EnemyAIBoss();
                    break;
                case BattleRPGEnemyID.yazawa_boss_mabuchi_c08:
                    ai = new EnemyAIMabuchi();
                    break;
                case BattleRPGEnemyID.yazawa_boss_mabuchi_c10:
                    ai = new EnemyAIMabuchi();
                    break;
                case BattleRPGEnemyID.yazawa_boss_totsuka_c041:
                    ai = new EnemyAIBoss();
                    break;
                case BattleRPGEnemyID.yazawa_boss_totsuka_c042:
                    ai = new EnemyAIBoss();
                    break;
                case BattleRPGEnemyID.yazawa_boss_totsuka_c09:
                    ai = new EnemyAIBoss();
                    break;

                case BattleRPGEnemyID.yazawa_boss_tei_c10:
                    ai = new EnemyAIBoss();
                    break;

                case BattleRPGEnemyID.boss_mabuchi_test:
                    ai = new EnemyAIMabuchi();
                    break;
            }

            if (ai == null)
            {
                switch (enemy.Character.Attributes.soldier_data_id)
                {
                    case CharacterNPCSoldierPersonalDataID.yazawa_btl03_0010_000_2:
                        ai = new EnemyAIHu();
                        break;
                }
            }
            if (ai == null)
            {
                //If we are fighting a lone enemy better to treat them as a boss for improved behavior
                if (BrawlerBattleManager.Enemies.Length == 1)
                {
                    if (ai == null)
                        ai = CreateBossAI(enemy);
                }

                if(ai == null)
                    ai = new EnemyAI();
            }

            ai.Character = enemy;
            ai.Chara = enemy.Character;

            return ai;
        }

        private static EnemyAIBoss CreateBossAI(Fighter enemy)
        {
            EnemyAIBoss ai = null;


            if (ai == null)
            {
                switch (enemy.Character.Attributes.soldier_data_id)
                {
                    case CharacterNPCSoldierPersonalDataID.yazawa_btl15_0030_000_9:
                        ai = new EnemyAIAoki2();
                        break;
                }
            }

            if (ai == null)
            {
                switch (Asset.GetArmsCategory(enemy.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get().AssetID))
                {
                    case AssetArmsCategoryID.D:
                        ai = new EnemyAIBossGenericWPD();
                        break;
                    case AssetArmsCategoryID.G:
                        ai = new EnemyAIBossGenericWPG();
                        break;
                }
            }

            if (ai == null)
            {
                switch (enemy.Character.Attributes.ctrl_type)
                {
                    default:
                        ai = new EnemyAIBoss();
                        break;

                    case BattleControlType.hitman:
                        ai = new EnemyAIBossGenericHitman();
                        break;
                    case BattleControlType.boss_majima_b:
                        ai = new EnemyAIMajima();
                        break;
                    case BattleControlType.boss_saejima:
                        ai = new EnemyAISaejima();
                        break;
                    case BattleControlType.boss_sawashiro:
                        ai = new EnemyAISawashiro();
                        break;
                    case BattleControlType.boss_sawashiro_e:
                        ai = new EnemyAISawashiro2();
                        break;
                    case BattleControlType.boss_kiryu:
                        ai = new EnemyAIKiryu();
                        break;
                    case BattleControlType.boss_ishioda:
                        ai = new EnemyAIIshioda();
                        break;
                    case BattleControlType.boss_mabuchi_i:
                        ai = new EnemyAIMabuchi();
                        break;
                    case BattleControlType.crane_truck:
                        ai = new EnemyAIMachinery();
                        break;
                    case BattleControlType.boss_crane_truck:
                        ai = new EnemyAIMachinery();
                        break;
                    case BattleControlType.boss_power_shovel:
                        ai = new EnemyAIMachinery();
                        break;
                    case BattleControlType.boss_nanba:
                        ai = new EnemyAINanba();
                        break;

                    case BattleControlType.boss_tiger:
                        ai = new EnemyAITiger();
                        break;

                    case BattleControlType.boss_tendo:
                        ai = new EnemyAITendo();
                        break;
                }
            }

            ai.Character = enemy;
            ai.Chara = enemy.Character;

            return ai;
        }
    }
}

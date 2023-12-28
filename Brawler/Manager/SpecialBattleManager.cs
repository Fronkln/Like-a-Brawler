using System;
using System.Collections.Generic;
using System.Linq;
using DragonEngineLibrary;

namespace Brawler
{
    internal static class SpecialBattleManager
    {
        public static void OnCombatInit()
        {
            new DETask(delegate { return FighterManager.GetAllEnemies().Length > 0; }, delegate
            {
                Fighter[] enemies = FighterManager.GetAllEnemies();

                if (enemies.Length >= 0)
                {
                    if (BattleProperty.BattleConfigID == 6)
                        Tutorial01();
                    else if (BattleProperty.BattleConfigID == 12)
                        Tutorial02();
                    else if (BattleProperty.BattleConfigID == 139)
                        Tutorial03();
                    else
                    {
                        //Dont change to BattleManager.Enemies
                        IEnumerable<Fighter> machinery = enemies.Where(
                            x => x.Character.Attributes.ctrl_type == BattleControlType.boss_power_shovel ||
                            x.Character.Attributes.ctrl_type == BattleControlType.boss_crane_truck);

                        //Special fight against machinery. Disable locking on
                        //Else Ichiban will lock into their arms and not the body (which is annoying)
                        if (machinery.Count() > 0)
                            BrawlerBattleManager.DisableTargetingOnce = true;

                        if (enemies.FirstOrDefault(x => x.Character.Attributes.soldier_data_id == CharacterNPCSoldierPersonalDataID.yazawa_btl02_0010_000_1).IsValid())
                        {
                            BrawlerBattleManager.KasugaChara.GetRender().Reload((CharacterID)0x3DB7, 0x1);
                        }

                    }

                    DragonEngine.Log("Special battle check");
                }
            });
        }


        //Basic controls
        private static void Tutorial01()
        {
            int m_blockCount = 0;

            //Testing
            TutorialManager.InitializeTutorial(
                new TutorialSegment[]
                {
                                new TutorialSegment()
                                {
                                    Instructions = "Test!",
                                    TimeToComplete = 1f,
                                    OnStart = delegate{HActManager.RequestHAct(new HActRequestOptions(){id = (TalkParamID)12902, is_force_play = true}); },
                                    Silent = true
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "",
                                    TimeToComplete = 1f,
                                    OnStart = delegate{BattleTurnManager.RequestHActEvent(new HActRequestOptions(){id = (TalkParamID)12903, is_force_play = true});  },
                                    Silent = true
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "Testing",
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage | TutorialModifier.NoHAct,
                                    TimeoutIsSuccess = true,
                                    IsCompleteDelegate = delegate{return false; },
                                    TimeToComplete = 15,
                                    UpdateDelegate =
                                    delegate
                                    {
                                        string lightFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.LightAttack);
                                        TutorialManager.SetText($"{lightFmt}{lightFmt}{lightFmt}\nRush Combo");
                                    }
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "",
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage | TutorialModifier.NoHAct,
                                    TimeoutIsSuccess = true,
                                    IsCompleteDelegate = delegate{return false; },
                                    TimeToComplete = 15,
                                    UpdateDelegate =
                                    delegate
                                    {
                                        string heavyFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.HeavyAttack);
                                        TutorialManager.SetText(
                                            $"{heavyFmt} during a Rush Combo\n" +
                                            $"Finishing Blows");
                                    }
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "",
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage | TutorialModifier.NoHAct,
                                    TimeoutIsSuccess = false,
                                    IsCompleteDelegate = delegate{ return AttackSimulator.PlayerInstance.CurrentAttack != null && AttackSimulator.PlayerInstance.CurrentAttack.AttackType == YazawaCommand.AttackType.MoveSidestep; },
                                    TimeToComplete = 10,
                                    UpdateDelegate =
                                    delegate
                                    {
                                        string swayFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.Dodge);
                                        TutorialManager.SetText($"{swayFmt}\nDodging");
                                    }
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "",
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage | TutorialModifier.NoHAct,
                                    TimeoutIsSuccess = false,
                                    IsCompleteDelegate = delegate{return m_blockCount == 3; },
                                    TimeToComplete = 15,
                                    OnStart = delegate
                                    {
                                        BrawlerPlayer.OnPlayerGuard += delegate{ m_blockCount++; };
                                    },
                                    UpdateDelegate =
                                    delegate
                                    {
                                        string grdFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.Block);
                                        TutorialManager.SetText($"Guard 3 attacks with {grdFmt}\nGuarding");
                                    }
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "",
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage | TutorialModifier.NoHAct,
                                    TimeoutIsSuccess = true,
                                    IsCompleteDelegate = delegate{return AttackSimulator.PlayerInstance.CurrentAttack!= null && AttackSimulator.PlayerInstance.CurrentAttack.AttackType == YazawaCommand.AttackType.MoveSync;  },
                                    TimeToComplete = 25,
                                    UpdateDelegate =
                                    delegate
                                    {
                                        string grabFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.Grab);
                                        TutorialManager.SetText($"Grab enemies with {grabFmt}\nGrabbing ");
                                    }
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "Kick Ushio's ass!",
                                    IsCompleteDelegate = delegate{ return BrawlerBattleManager.Enemies.Length <= 0; },
                                    TimeToComplete = -1,
                                },
                });
        }

        //HAct and weapon
        private static void Tutorial02()
        {
            TutorialManager.InitializeTutorial(new TutorialSegment[]
            {

                new TutorialSegment()
                {
                    Instructions = "",
                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage | TutorialModifier.NoHAct,
                    TimeoutIsSuccess = true,
                    IsCompleteDelegate= delegate{return false ; },
                    TimeToComplete = 10,
                    UpdateDelegate =
                    delegate
                    {
                        string lockFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.LockOn);
                        TutorialManager.SetText($"Lock onto the nearest enemy with {lockFmt}\nBattle Stance");
                    }
                },
                new TutorialSegment()
                {
                    Instructions = "",
                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage | TutorialModifier.NoHAct,
                    TimeoutIsSuccess = true,
                    IsCompleteDelegate= delegate{return BrawlerPlayer.Info.RightWeapon.IsValid(); },
                    TimeToComplete = 15,
                    UpdateDelegate =
                    delegate
                    {
                        string grabFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.Grab);
                        TutorialManager.SetText($"Pick up nearby weapons with {grabFmt}\nPicking Weapons");
                    }
                },
                new TutorialSegment()
                {
                    Instructions = "",
                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage,
                    TimeoutIsSuccess = true,
                    IsCompleteDelegate= delegate{return BrawlerBattleManager.HActIsPlaying; },
                    TimeToComplete = 30,
                    UpdateDelegate =
                    delegate
                    {
                        Player.SetHeatNow(Player.ID.kasuga, Player.GetHeatMax(Player.ID.kasuga));

                        string heavyFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.HeavyAttack);
                        TutorialManager.SetText($"{heavyFmt} nearby a downed enemy or while holding a weapon\nHeat Actions");
                    }
                }
            }); ;
        }

        private static void Tutorial03()
        {
            //lets ensure kasuga has the ability to enter extreme heat mode here
            if (Player.GetLevel(Player.ID.kasuga) < 8)
                Player.SetLevel(8, Player.ID.kasuga);

            TutorialManager.InitializeTutorial(new TutorialSegment[]
            {
                new TutorialSegment()
                {
                    Instructions = "",
                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage | TutorialModifier.NoHAct,
                    TimeoutIsSuccess = true,
                    IsCompleteDelegate= delegate{return BrawlerPlayer.IsEXGamer; },
                    TimeToComplete = 10,
                    UpdateDelegate =
                    delegate
                    {
                        Player.SetHeatNow(Player.ID.kasuga, Player.GetHeatMax(Player.ID.kasuga));

                        string heatFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.ExHeat);
                        TutorialManager.SetText($"Enter extreme heat mode with {heatFmt}\nExtreme Heat Mode");
                    }
                },
            });
        }
    }
}

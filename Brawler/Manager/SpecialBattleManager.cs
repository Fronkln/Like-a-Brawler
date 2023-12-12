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
                    if (enemies[0].Character.Attributes.soldier_data_id == CharacterNPCSoldierPersonalDataID.yazawa_btl01_0010_000_1)
                        Tutorial01();
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


        private static void Tutorial01()
        {
            int m_blockCount = 0;

            //Testing
            TutorialManager.Initialize(
                new TutorialSegment[]
                {
                                new TutorialSegment()
                                {
                                    Instructions = "Test!",
                                    TimeToComplete = 1f,
                                    OnStart = delegate{HActManager.RequestHAct(new HActRequestOptions(){id = (TalkParamID)12902, is_force_play = true}); DragonEngine.Log("play hact"); },
                                    Silent = true
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "",
                                    TimeToComplete = 1f,
                                    OnStart = delegate{BattleTurnManager.RequestHActEvent(new HActRequestOptions(){id = (TalkParamID)12903, is_force_play = true}); DragonEngine.Log("play hact 2");  },
                                    Silent = true
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "Testing",
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage,
                                    TimeoutIsSuccess = true,
                                    IsCompleteDelegate = delegate{return false; },
                                    TimeToComplete = 20,
                                    UpdateDelegate =
                                    delegate
                                    {
                                        string lightFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.LightAttack);
                                        TutorialManager.SetText($"{lightFmt}{lightFmt}{lightFmt}\nRush Combo");
                                    }
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "Testing",
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage,
                                    TimeoutIsSuccess = true,
                                    IsCompleteDelegate = delegate{return false; },
                                    TimeToComplete = 20,
                                    UpdateDelegate =
                                    delegate
                                    {
                                        string lightFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.LightAttack);
                                        string heavyFmt = TutorialManager.GetFormattedButtonStr(TutorialButton.HeavyAttack);
                                        TutorialManager.SetText(
                                            $"{heavyFmt} during a Rush Combo\n" +
                                            $"Finishing Blows");
                                    }
                                },
                                new TutorialSegment()
                                {
                                    Instructions = "",
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage,
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
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage,
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
                                    Modifiers = TutorialModifier.PlayerDontTakeDamage | TutorialModifier.EnemyDontTakeDamage,
                                    TimeoutIsSuccess = true,
                                    IsCompleteDelegate = delegate{return false; },
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
    }
}

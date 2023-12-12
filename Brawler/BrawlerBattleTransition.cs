using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using DragonEngineLibrary;
using MinHook.NET;
using static Brawler.BrawlerHooks;

namespace Brawler
{
    //Controls the transition of turn phases

    internal static class BrawlerBattleTransition
    {
        public static bool DontAllowEnd = false;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate bool BattleTurnManagerExecPhaseCleanup(IntPtr mngr);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool ExecPhaseBattleResult(IntPtr mng);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void BattleTurnManagerChangePhase(IntPtr mng, BattleTurnManager.TurnPhase phase);


        public static void Init()
        {
            _execPhaseCleanupDeleg = new BattleTurnManagerExecPhaseCleanup(BattleTurnManager_ExecPhaseCleanup);
            _btlTurnManagerChangePhaseDeleg = new BattleTurnManagerChangePhase(BattleTurnManager_ChangePhase);

            MinHookHelper.createHook((IntPtr)DragonEngineLibrary.Unsafe.CPP.PatternSearch("40 55 56 57 41 56 41 57 48 8B EC 48 83 EC ? 48 C7 45 B0 ? ? ? ? 48 89 9C 24 B0 00 00 00 C5 F8 29 74 24 60"), _execPhaseCleanupDeleg, out _execPhaseCleanupTrampoline);
            MinHookHelper.createHook((IntPtr)DragonEngineLibrary.Unsafe.CPP.PatternSearch("40 55 41 54 41 55 41 56 41 57 48 8D AC 24 D0 EE FF FF"), _btlTurnManagerChangePhaseDeleg, out _btlTurnManagerChangePhaseTrampoline);
        }

        public static void TestExec(IntPtr rawPtr)
        {
            ExecPhaseBattleResult resFunc = (ExecPhaseBattleResult)Marshal.GetDelegateForFunctionPointer(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8B C4 57 48 81 EC ? ? ? ? 48 C7 40 B8 ? ? ? ?"), typeof(ExecPhaseBattleResult));
            resFunc.Invoke(rawPtr);
        }


        //Rewrite: It breaks hacts with reinforcements
        private static BattleTurnManagerExecPhaseCleanup _execPhaseCleanupDeleg;
        private static BattleTurnManagerExecPhaseCleanup _execPhaseCleanupTrampoline;
        private static bool m_cleanupProcedureDoOnce = false;
        private static bool m_forceExecuteOriginalCleanup = false;
        private static unsafe bool BattleTurnManager_ExecPhaseCleanup(IntPtr mng)
        {
            if (m_cleanupProcedureDoOnce)
                return true;

            if (m_forceExecuteOriginalCleanup || BrawlerBattleManager.Kasuga.IsDead())
                return _execPhaseCleanupTrampoline(mng);

            //Battle is not over yet, likely a second wave is inbound.
            //Run original cleanup code.

            //To fully determine if the battle is over, we should wait around 0.1 - 0.2 seconds

            // return _execPhaseCleanupTrampoline(mng);

            if (!m_cleanupProcedureDoOnce)
            {
                m_cleanupProcedureDoOnce = true;

                if(BrawlerBattleManager.IsEncounter)
                {
                    RunY7BCleanupProcedure(mng);
                }
                else
                {
                    new DETaskTime(0.1f,
                        delegate
                        {
                            DragonEngine.Log(BattleTurnManager.BattleConfigID + " " + BattleProperty.BattleConfigID);
                            bool doY7bTransition = BattleTurnManager.BattleConfigID == BattleProperty.BattleConfigID || BrawlerBattleManager.Kasuga.IsDead();

                            if (!doY7bTransition)
                            {
                                m_forceExecuteOriginalCleanup = true;
                                m_cleanupProcedureDoOnce = false;
                            }
                            else
                                RunY7BCleanupProcedure(mng);

                            DragonEngine.Log(doY7bTransition + " full over");
                        }, gameTime: false);
                }
            }

            return true;
        }

        private static void RunY7BCleanupProcedure(IntPtr mng)
        {
            BattleMusic.OnBattleEnd();

            float totalWaitTime = 0;

            new DETaskList
            (
                new DETask(delegate { return !HActManager.IsPlaying() && !BrawlerBattleManager.Kasuga.IsSync(); }, delegate { new DETaskTime(0.35f, delegate { BattleCamera.Phase = 1; }); }, false),
                new DETask(delegate { totalWaitTime += DragonEngine.deltaTime; return !BrawlerBattleManager.KasugaChara.HumanModeManager.IsAttack() && !BrawlerBattleManager.Kasuga.IsSync(); }, null, false),
                new DETaskTime(totalWaitTime < 1 ? 0.8f : 0.3f,
                                    delegate
                                    {
                                        BattleCamera.Phase = 2;
                                        BrawlerBattleManager.KasugaChara.GetMotion().RequestBehavior(MotionBehaviorType.Battle, BehaviorActionID.P_KRU_MOV_std_btl_cautnml_tfm);

                                        //Played through soundmanager so that its 2D and easier to hear
                                        if (!BrawlerBattleManager.Kasuga.IsBrawlerCriticalHP())
                                            SoundManager.PlayCue(SoundCuesheetID.gv_fighter_kasuga, 84, 0);
                                        else
                                            SoundManager.PlayCue(SoundCuesheetID.gv_fighter_kasuga, 86, 0);
                                    }, false),
                new DETaskTime(1.5f,
                delegate
                {
                    BattleTurnManager.ChangePhase(BattleTurnManager.TurnPhase.BattleResult);

                    if (!IniSettings.EnableBattleResults)
                    {
                        TestExec(mng);
                        BattleTurnManager.ChangePhase(BattleTurnManager.TurnPhase.End);
                    }

                    BattleCamera.Phase = 0;

                    m_cleanupProcedureDoOnce = false;
                }, false)
            );
        }

        private static BattleTurnManagerChangePhase _btlTurnManagerChangePhaseDeleg;
        private static BattleTurnManagerChangePhase _btlTurnManagerChangePhaseTrampoline;
        private static void BattleTurnManager_ChangePhase(IntPtr mng, BattleTurnManager.TurnPhase phase)
        {
            //  _btlTurnManagerChangePhaseTrampoline(mng, phase);
            // return;
            //dont forget this nerd
            //     if (phase > BattleTurnManager.TurnPhase.StartWait && phase != BattleTurnManager.TurnPhase.NumPhases)
            // return;

            /*
            if (phase == BattleTurnManager.TurnPhase.Event && HeatActionManager.BlockEventDoOnce)
            {
                DragonEngine.Log("Block Event request");
                HeatActionManager.BlockEventDoOnce = false;
                return;
            }

            */


            bool ichiDead = BrawlerBattleManager.KasugaChara.IsDead();

            if (ichiDead)
            {
                _btlTurnManagerChangePhaseTrampoline(mng, phase);
                return;
            }


            if (BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Cleanup && phase != BattleTurnManager.TurnPhase.Cleanup)
            {
                m_cleanupProcedureDoOnce = false;
                m_forceExecuteOriginalCleanup = false;
            }

            switch (phase)
            {

                case BattleTurnManager.TurnPhase.NumPhases:
                    if(BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.End)
                        BrawlerBattleManager.OnBattleEndCallback();
                    break;

                case BattleTurnManager.TurnPhase.Action:
                    if (BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Event)
                        if (BrawlerBattleManager.Enemies.Length <= 0 || BrawlerBattleManager.Enemies.FirstOrDefault(x => !x.IsDead()) == null)
                        {
                            phase = BattleTurnManager.TurnPhase.Cleanup;
                        }
                    break;
                    
                case BattleTurnManager.TurnPhase.Cleanup:
                    //   if (BrawlerBattleManager.Kasuga.IsDead() || DontAllowEnd || BrawlerBattleManager.HActIsPlaying)
                    if (BrawlerBattleManager.Kasuga.IsDead())
                        return;

                    
                    if (BrawlerBattleManager.CurrentHActIsY7B)
                    {
                        SoundManager.PlayCue(SoundCuesheetID.battle_common, 21, 0);
                        new DETaskTime(1.2f, null, true, delegate
                        {
                            if (!BrawlerBattleManager.HActIsPlaying)
                                return true;

                            DragonEngine.SetSpeed(DESpeedType.Unprocessed, 0.1f);
                            //If we dont also slow down these, things will look odd if they link out
                            //during finish slowmo. Example: y7b1250_buki_g
                            DragonEngine.SetSpeed(DESpeedType.General, 0.1f);
                            DragonEngine.SetSpeed(DESpeedType.Character, 0.1f);
                            DragonEngine.SetSpeed(DESpeedType.Player, 0.1f);

                            return false;
                        }, false);
                    }
                    
                    break;

                case BattleTurnManager.TurnPhase.Event:

                    if (BrawlerBattleManager.BattleTime < 1 || BrawlerBattleManager.CurrentHActIsY7B)
                        return;
                    
                    /*
                    if(BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Action)
                        if (BrawlerBattleManager.Enemies.Length <= 1)
                            return;
                    */
                    break;

                case BattleTurnManager.TurnPhase.End:
                    for (uint i = 0; i < BrawlerBattleManager.PartyMembersCache.Length; i++)
                        NakamaManager.Change(i + 1, BrawlerBattleManager.PartyMembersCache[i]);
                    break;
            }

            Console.WriteLine(BattleTurnManager.CurrentPhase + " -> " + phase);
            _btlTurnManagerChangePhaseTrampoline(mng, phase);
        }
    }
}

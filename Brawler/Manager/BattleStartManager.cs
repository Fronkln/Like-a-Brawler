using System;
using System.Runtime.InteropServices;
using MinHook.NET;
using DragonEngineLibrary;
using static Brawler.BrawlerHooks;

namespace Brawler
{
    internal static class BattleStartManager
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void BattleStartManagerDecideType(IntPtr mng);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return:MarshalAs(UnmanagedType.U1)]
        private unsafe delegate bool BattleStartManagerCanSeamless(void* mng);

        private static BattleStartManagerDecideType m_decideTypeDelegate;
        private static BattleStartManagerDecideType m_decideTypeTrampoline;

        public static void Hook()
        {
            m_decideTypeDelegate = new BattleStartManagerDecideType(DecideTypeHook);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 20 55 56 41 56 48 8D 6C 24 B9 48 81 EC ? ? ? ? C5 F8 29 B4 24 C0 00 00 00"), m_decideTypeDelegate, out m_decideTypeTrampoline);
        }

        public static void DecideTypeHook(IntPtr manager)
        {
            BrawlerBattleManager.CheckSpecialBattle();

            m_decideTypeTrampoline(manager);

            unsafe
            {
                uint startType = (*(uint*)(manager.ToInt64() + 0x50));
                uint* divPtr = (uint*)(manager.ToInt64() + 0x54);
                TalkParamID divId = (TalkParamID)(*divPtr);

                if (divId == TalkParamID.yazawa_btlst_eb_zako_no_trans ||
                    divId == TalkParamID.BTLst_eb_zako_test ||
                    divId == TalkParamID.test_yazawa_enc_st || divId.ToString().Contains("_enc"))
                    BrawlerBattleManager.OnStartEncounterBattle();
                else
                    BrawlerBattleManager.IsEncounter = false;

            }
        }
    }
}

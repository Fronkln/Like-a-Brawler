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
            MinHookHelper.createHook((IntPtr)0x1409A9BC0, m_decideTypeDelegate, out m_decideTypeTrampoline);
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


            DragonEngine.Log("we deciding");
        }

        public unsafe static bool CanSeamless()
        {
            void* battleStartManager = *((void**)0x142BC7200);


            BattleStartManagerCanSeamless resFunc = (BattleStartManagerCanSeamless)Marshal.GetDelegateForFunctionPointer((IntPtr)0x00000001409ABE10, typeof(BattleStartManagerCanSeamless));
            return resFunc.Invoke(battleStartManager);
        }
    }
}

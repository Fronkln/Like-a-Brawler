using DragonEngineLibrary;
using MinHook.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    public unsafe static class CameraHooks
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return:MarshalAs(UnmanagedType.U1)]
        private delegate bool CameraFreeAllowInput(IntPtr camera);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void CameraFreeUpdateTarget(IntPtr camera);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate uint* CameraFreeGetLookAt(IntPtr camera, uint* input);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void CameraFreeUpdate(IntPtr camera);

        public static void Hook()
        {
            _camFreeAllowInputDeleg = new CameraFreeAllowInput(CameraFree_AllowInput);
            _camFreeUpdateTargetDeleg = new CameraFreeUpdateTarget(CameraFree_UpdateTarget);
            _camFreeUpdateDeleg = new CameraFreeUpdate(CameraFree_Update);

            //Warning: peepeepoopoo pattern
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("B8 ? ? ? ? C3 CC CC CC CC CC CC CC CC CC CC 33 C0 C3 CC CC CC CC CC CC CC CC CC CC CC CC CC B8 ? ? ? ? C3 CC CC CC CC CC CC CC CC CC CC 33 C0 C3 CC CC CC CC CC CC CC CC CC CC CC CC CC 0F B6 81 2F 05 00 00"), _camFreeAllowInputDeleg, out _camFreeAllowInputTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("40 55 56 57 48 8D 6C 24 E0"), _camFreeUpdateTargetDeleg, out _camFreeUpdateTargetTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8B C4 56 57 41 56 48 83 EC ? 48 C7 40 A8 ? ? ? ? 48 89 58 10 48 89 68 18 C5 F8 29 70 D8 48 8B F1"), _camFreeUpdateDeleg, out _camFreeUpdateTrampoline);
        }

        //Dont allow movement when we are in battle finishing sequence.
        private static CameraFreeAllowInput _camFreeAllowInputDeleg;
        private static CameraFreeAllowInput _camFreeAllowInputTrampoline;
        private static bool CameraFree_AllowInput(IntPtr camera)
        {
            BattleTurnManager.TurnPhase phase = BattleTurnManager.CurrentPhase;

            if (phase == BattleTurnManager.TurnPhase.Cleanup || phase == BattleTurnManager.TurnPhase.End)
                return false;

            return true;
        }

        //Dont allow movement when we are in battle finishing sequence.
        private static CameraFreeUpdateTarget _camFreeUpdateTargetDeleg;
        private static CameraFreeUpdateTarget _camFreeUpdateTargetTrampoline;
        private static void CameraFree_UpdateTarget(IntPtr camera)
        {
            BattleTurnManager.TurnPhase phase = BattleTurnManager.CurrentPhase;

            if (phase == BattleTurnManager.TurnPhase.Cleanup || phase == BattleTurnManager.TurnPhase.End)
                if(BattleCamera.Phase != 0)
                    return;

            _camFreeUpdateTargetTrampoline(camera);
        }


        //Look at dead enemy when we are in battle finishing sequence.
        private static CameraFreeUpdate _camFreeUpdateDeleg;
        private static CameraFreeUpdate _camFreeUpdateTrampoline;
        private static void CameraFree_Update(IntPtr camera)
        {
            BattleTurnManager.TurnPhase phase = BattleTurnManager.CurrentPhase;
            uint* targetPtr = (uint*)((camera.ToInt64()) + 0x268);

            if (BrawlerBattleManager.Kasuga.IsValid())
            {
                if (phase == BattleTurnManager.TurnPhase.Cleanup || phase == BattleTurnManager.TurnPhase.End)
                {
                    Fighter[] enemies = null;

                    switch (BattleCamera.Phase)
                    {
                        case 0:
                            *targetPtr = BrawlerBattleManager.KasugaChara.UID;
                            break;
                        case 1:
                            enemies = FighterManager.GetAllEnemies();

                            if (enemies.Length > 0)
                                *targetPtr = enemies[0].Character.UID;
                            break;
                        case 2:
                            *targetPtr = BrawlerBattleManager.KasugaChara.UID;
                            break;
                    }
                }
                else
                    *targetPtr = BrawlerBattleManager.KasugaChara.UID;
            }
            else
            {
                *targetPtr = BrawlerBattleManager.KasugaChara.UID;
            }

             _camFreeUpdateTrampoline(camera);
        }
    }
}

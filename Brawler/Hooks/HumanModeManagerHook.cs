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
    public static class HumanModeManagerHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        private unsafe delegate bool HumanModeManagerTransitExecPickup(IntPtr humanMode);

        public static void Hook()
        {
            _humanModeTransitPickupDeleg = new HumanModeManagerTransitExecPickup(HumanModeManager_TransitExecPickup);

            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 08 48 89 6C 24 18 48 89 74 24 20 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 8B F9"), _humanModeTransitPickupDeleg, out _humanModeTransitDmgPickupTrampoline);
        }

        private static HumanModeManagerTransitExecPickup _humanModeTransitPickupDeleg;
        private static HumanModeManagerTransitExecPickup _humanModeTransitDmgPickupTrampoline;
        private static unsafe bool HumanModeManager_TransitExecPickup(IntPtr humanModeManager)
        {
            if (humanModeManager != BrawlerBattleManager.KasugaChara.HumanModeManager.Pointer)
                return false;

            if (BrawlerPlayer.CanPickupWeapon())
            {
                AssetUnit wep = AssetManager.FindNearestAssetFromAll(BrawlerBattleManager.KasugaChara.GetPosCenter(), 2);

                if (!wep.IsValid())
                    return false;

                BrawlerPlayer.PickupAsset(wep);
                return true;
            }
            else
                return false;
        }
    }
}

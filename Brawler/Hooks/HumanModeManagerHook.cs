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

            MinHookHelper.createHook((IntPtr)0x1406ED4A0, _humanModeTransitPickupDeleg, out _humanModeTransitDmgPickupTrampoline);
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

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
    public static class FighterModeHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate MotionID FighterModePickupGetMotionID(IntPtr humanMode);

        public static void Hook()
        {
            _pickupMotionIDDeleg = new FighterModePickupGetMotionID(FighterModePickup_GetMotionID);

            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 10 48 89 6C 24 18 56 57 41 56 48 83 EC ? 48 8B F1 48 83 C1 ?"), _pickupMotionIDDeleg, out _pickupMotionIDTrampoline);
        }


        private static FighterModePickupGetMotionID _pickupMotionIDDeleg;
        private static FighterModePickupGetMotionID _pickupMotionIDTrampoline;
        private static unsafe MotionID FighterModePickup_GetMotionID(IntPtr humanMode)
        {
            //CRASH!! epic failed
            //CRASH!! epic failed
            //CRASH!! epic failed
            IntPtr module = (IntPtr)(*(ulong*)(humanMode.ToInt64() + 0xA0));
            uint asset = (*(uint*)(module.ToInt64() + 0x38));
            EntityHandle<AssetUnit> wep = new EntityHandle<AssetUnit>(asset);

            switch(Asset.GetArmsCategory(wep.Get().AssetID))
            {
                default:
                    return (MotionID)17150;
                case AssetArmsCategoryID.A:
                    return (MotionID)17150;
                case AssetArmsCategoryID.B:
                    return (MotionID)17151;
                case AssetArmsCategoryID.C:
                    return (MotionID)17152;
                case AssetArmsCategoryID.D:
                    return (MotionID)17153;
                case AssetArmsCategoryID.H:
                    return (MotionID)17154;

            }
        }
    }
}

using DragonEngineLibrary;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using MinHook;
using MinHook.NET;

namespace Brawler
{
    public unsafe static class AuthHooks
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void AuthPlayCameraActive(IntPtr authPlay);

        /// <summary>
        /// Force next HAct to use a random camera.
        /// </summary>
        public static bool AlterNextHAct;

        public static void Hook()
        {
            _cameraActiveDeleg = new AuthPlayCameraActive(AuthPlay_CameraActive);

            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8B C4 55 41 54 41 55 41 56 41 57 48 8D 68 88 48 81 EC ? ? ? ? 48 C7 44 24 70 ? ? ? ? 48 89 58 10 48 89 70 18 48 89 78 20 48 8B 05 ? ? ? ? 48 33 C4 48 89 45 40 48 8B F1"), _cameraActiveDeleg, out _cameraActiveTrampoline);
        }


        private static AuthPlayCameraActive _cameraActiveDeleg;
        private static AuthPlayCameraActive _cameraActiveTrampoline;
        private static void AuthPlay_CameraActive(IntPtr authPlay)
        {
            if (AlterNextHAct)
            {
                int* flags = (int*)(authPlay.ToInt64() + 0x7f4);
                *flags |= 0x1000;
                *flags &= ~0x2000;

                AlterNextHAct = false;
            }

            _cameraActiveTrampoline(authPlay);
        }
    }
}

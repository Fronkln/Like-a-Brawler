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

            MinHookHelper.createHook((IntPtr)0x14079EA90, _cameraActiveDeleg, out _cameraActiveTrampoline);
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

                DragonEngine.Log("alter");

                AlterNextHAct = false;
            }

            _cameraActiveTrampoline(authPlay);
        }
    }
}

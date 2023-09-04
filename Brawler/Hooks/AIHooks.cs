using System;
using System.Runtime.InteropServices;
using DragonEngineLibrary;
using MinHook.NET;

namespace Brawler
{
    public unsafe static class AIHooks
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate IntPtr CreateRPGAI(IntPtr pack);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void CreateZakoAI(IntPtr ai, uint* owner, IntPtr pack);

        public static void Hook()
        {
            _createRpgAIDeleg = new CreateRPGAI(Create_Rpg_AI);
          //  MinHookHelper.createHook((IntPtr)0x1409CEDA0, _createRpgAIDeleg, out _createRpgAITrampoline);
        }

        private static CreateRPGAI _createRpgAIDeleg;
        private static CreateRPGAI _createRpgAITrampoline;
        private static IntPtr Create_Rpg_AI(IntPtr pack)
        {
            return _createRpgAITrampoline(pack);

        //    ulong* aiPtr = (ulong*)((pack.ToInt64() + 0x138));

            //now overwrite everything they did with zako AI 😈😈😈😈😈😈😈
        }
    }
}

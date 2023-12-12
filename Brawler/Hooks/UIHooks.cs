using System;
using System.Runtime.InteropServices;
using MinHook.NET;
using DragonEngineLibrary;

namespace Brawler
{
    public unsafe static class UIHooks
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate void PartySetMemberInfo(IntPtr ui, Player.ID id, uint slot, bool mainMember, uint* entity);

        public static void Hook()
        {
            _setMemberInfDeleg = new PartySetMemberInfo(Party_SetMemberInfo);

            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.ReadCall(DragonEngineLibrary.Unsafe.CPP.PatternSearch("E8 ? ? ? ? FF C3 41 3B DF 72 ? 44 8B E6")), _setMemberInfDeleg, out _setMemberInfTrampoline);
        }

        private static PartySetMemberInfo _setMemberInfDeleg;
        private static PartySetMemberInfo _setMemberInfTrampoline;
        private static void Party_SetMemberInfo(IntPtr ui, Player.ID id, uint slot, bool mainMember, uint* entit)
        {;
            if (NakamaManager.FindIndex(id) == Party.GetMainMemberCount() - 1)
                id = Player.ID.kasuga;
            else
                id = Player.ID.adachi;

            _setMemberInfTrampoline(ui, id, slot, mainMember, entit);
        }
    }
}

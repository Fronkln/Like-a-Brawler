using System;
using System.Runtime.InteropServices;

namespace Brawler
{
    public static class EncounterManager
    {
        [DllImport("Y7Internal.dll", EntryPoint = "LIB_CENCOUNT_MANAGER_IS_ENCOUNTER", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsEncounter();
    }
}

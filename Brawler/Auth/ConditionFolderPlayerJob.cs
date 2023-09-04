using System;
using System.Runtime.InteropServices;
using DragonEngineLibrary;

namespace Brawler.Auth
{
    internal static class ConditionFolderPlayerJob
    {
        public static bool CheckDisabled(IntPtr disableInfo, IntPtr node)
        {
            ConditionFolderDisableData dat = Marshal.PtrToStructure<ConditionFolderDisableData>(disableInfo);

            RPGJobID playerJob;

            if (!BrawlerPlayer.IsEXGamer)
                playerJob = RPGJobID.kasuga_freeter;
            else
                playerJob = Player.GetCurrentJob(Player.ID.kasuga);

            if ((uint)playerJob != dat.tag_value)
                return true;

            return false;
        }
    }
}

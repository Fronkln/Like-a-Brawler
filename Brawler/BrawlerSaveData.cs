using System;
using System.Collections.Generic;
using DragonEngineLibrary;

namespace Brawler
{
    public unsafe static class BrawlerSaveData
    {
        private static IntPtr GetDataPtr()
        {
            //verification chunk which is a JE leftover
            IntPtr addr = SaveData.GetItem(21);
            uint* brawlerFlags = (uint*)(addr.ToInt64() + 8);

            return (IntPtr)brawlerFlags;
        }


        //If higher than zero, means we are gonna show all revelations starting from
        //The lowest revelation we haven't seen yet
        //Example, player is level 4, jumps to level 99
        //Save byte is set to level 4, revelations starting from level 4 will be shown.
        public static void SetRevelationQueue(int playerLevel)
        {
            IntPtr data = GetDataPtr();

            if (playerLevel < 0)
                playerLevel = 0;
            if (playerLevel > 99)
                playerLevel = 99;

            byte* start = (byte*)data;
            *start = (byte)playerLevel;
        }
        
        public static uint GetRevelationQueue()
        {
            IntPtr data = GetDataPtr();

            byte* start = (byte*)data;
            return (uint)*start;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonEngineLibrary;

namespace Brawler.Auth
{
    //82: Weapon Throw
    internal unsafe class AuthNodeBrawlerThrow
    {
        public static void Play(IntPtr thisObj, uint tick, IntPtr mtx, IntPtr unk)
        {
            BrawlerBattleManager.Kasuga.ThrowEquipAsset(false, true);
        }
    }
}

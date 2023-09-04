using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DragonEngineLibrary;

namespace Brawler
{
    [StructLayout(LayoutKind.Sequential, Size = 0x850)]
    internal unsafe struct FighterModeDamage
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x90)]
        public byte[] Unk;

        public BattleDamageInfo* Info;

        public byte[] Unk2;
    }
}

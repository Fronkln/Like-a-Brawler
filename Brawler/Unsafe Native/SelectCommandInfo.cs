using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    [StructLayout(LayoutKind.Explicit, Size = 0x20)]
    internal unsafe struct SelectCommandInfo
    {
        [FieldOffset(0x0)]
        public CommandRef command;
        [FieldOffset(0x10)]
        public FighterID target_fighter;
        [FieldOffset(0x14)]
        public uint target_player_id;
        [FieldOffset(0x18)]
        public uint target_asset;
        [FieldOffset(0x1C)]
        public bool is_skip;
        [FieldOffset(0x1D)]
        public bool is_wait;
    }
}

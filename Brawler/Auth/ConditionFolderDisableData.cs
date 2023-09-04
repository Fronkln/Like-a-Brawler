using System.Runtime.InteropServices;
using DragonEngineLibrary;

namespace Brawler.Auth
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct ConditionFolderDisableData
    {
        //Handles
        [FieldOffset(0)]
        public uint character_base;
        [FieldOffset(4)]
        public uint node_tag_guid;
        [FieldOffset(8)]
        public uint auth_player_handle;
        [FieldOffset(12)]
        public uint scene;
        [FieldOffset(16)]
        public uint entity_a;
        [FieldOffset(20)]
        public uint entity_b;
        [FieldOffset(24)]
        public uint type;
        [FieldOffset(32)]
        public uint tag_value;
    }
}

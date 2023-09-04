using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class HeatActionCondition
    {
        public HeatActionConditionType Type;
        public LogicalOperator LogicalOperator = LogicalOperator.TRUE;

        //Parameters, all of them are not required
        //It depends on the condition type
        public uint Param1U32;
        public uint Param2U32;
        public uint Param3U32;
        public uint Param4U32;

        public int Param1I32;
        public int Param2I32;
        public int Param3I32;

        public float Param1F;
        public float Param2F;
        public float Param3F;
        public float Param4F;
        public float Param5F;
        public float Param6F;

        public bool Param1B;
        public bool Param2B;
        public bool Param3B;

        public ulong Param1U64;
        public ulong Param2U64;
        public ulong Param3U64;

        internal void Write(BinaryWriter writer)
        {
            writer.Write((uint)Type);
            writer.Write((uint)LogicalOperator);

            writer.Write(Param1U32);
            writer.Write(Param2U32);
            writer.Write(Param3U32);
            writer.Write(Param4U32);

            writer.Write(Param1I32);
            writer.Write(Param2I32);
            writer.Write(Param2I32);

            writer.Write(Param1F);
            writer.Write(Param2F);
            writer.Write(Param3F);
            writer.Write(Param4F);
            writer.Write(Param5F);
            writer.Write(Param6F);

            writer.Write(Param1B);
            writer.Write(Param2B);
            writer.Write(Param3B);

            writer.Write(new byte[5]);

            writer.Write(Param1U64);
            writer.Write(Param2U64);
            writer.Write(Param3U64);
        }

        internal void Read(BinaryReader reader, uint version)
        {
            Type = (HeatActionConditionType)reader.ReadUInt32();
            LogicalOperator = (LogicalOperator)reader.ReadUInt32();

            Param1U32 = reader.ReadUInt32();
            Param2U32 = reader.ReadUInt32();
            Param3U32 = reader.ReadUInt32();
            Param4U32 = reader.ReadUInt32();

            Param1I32 = reader.ReadInt32();
            Param2I32 = reader.ReadInt32();
            Param3I32 = reader.ReadInt32();

            Param1F = reader.ReadSingle();
            Param2F = reader.ReadSingle();
            Param3F = reader.ReadSingle();
            Param4F = reader.ReadSingle();
            Param5F = reader.ReadSingle();
            Param6F = reader.ReadSingle();

            Param1B = reader.ReadBoolean();
            Param2B = reader.ReadBoolean();
            Param3B = reader.ReadBoolean();

            reader.BaseStream.Position += 5;

            if(version > 5)
            {
                Param1U64 = reader.ReadUInt64();
                Param1U64 = reader.ReadUInt64();
                Param1U64 = reader.ReadUInt64();
            }
        }
    }
}

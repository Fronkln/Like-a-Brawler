using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    [System.Serializable]
    public class AttackCondition
    {
        internal Attack _parent;

        public AttackConditionType Type;
        public LogicalOperator LogicalOperator = LogicalOperator.TRUE;

        //Parameters, all of them are not required
        //It depends on the condition type
        public uint Param1U32;
        public uint Param2U32;

        public int Param1I32;
        public int Param2I32;

        public float Param1F;
        public float Param2F;

        public bool Param1B;
        public bool Param2B;

        internal void Write(BinaryWriter writer)
        {
            writer.Write((uint)Type);
            writer.Write((uint)LogicalOperator);

            writer.Write(Param1U32);
            writer.Write(Param2U32);

            writer.Write(Param1I32);
            writer.Write(Param2I32);

            writer.Write(Param1F);
            writer.Write(Param2F);

            writer.Write(Param1B);
            writer.Write(Param2B);

            writer.Write(new byte[6]);
        }

        internal void Read(BinaryReader reader)
        {
            Type = (AttackConditionType)reader.ReadUInt32();
            LogicalOperator = (LogicalOperator)reader.ReadUInt32();

            Param1U32 = reader.ReadUInt32();
            Param2U32 = reader.ReadUInt32();

            Param1I32 = reader.ReadInt32();
            Param2I32 = reader.ReadInt32();

            Param1F = reader.ReadSingle();
            Param2F = reader.ReadSingle();

            Param1B = reader.ReadBoolean();
            Param2B = reader.ReadBoolean();

            reader.BaseStream.Position += 6;
        }
    }
}

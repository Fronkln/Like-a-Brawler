using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class AttackRPG : Attack
    {
        public override AttackType AttackType => AttackType.MoveRPG;

        public uint ID;

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(ID);
        }

        internal override void Read(BinaryReader reader, uint version)
        {
            base.Read(reader, version);

            ID = reader.ReadUInt32();
        }
    }
}

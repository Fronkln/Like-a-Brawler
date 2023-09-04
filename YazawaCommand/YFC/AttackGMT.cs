using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class AttackGMT : Attack
    {
        public override AttackType AttackType => AttackType.MoveGMTOnly;

        public uint MotionID = 0;

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
                
            writer.Write(MotionID);
        }

        internal override void Read(BinaryReader reader, uint version)
        {
            base.Read(reader, version);

            MotionID = reader.ReadUInt32();
        }
    }
}

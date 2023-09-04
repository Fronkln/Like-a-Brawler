using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class AttackSync : AttackRPG
    {
        public override AttackType AttackType => AttackType.MoveSync;
        public AttackSyncType SyncType = 0; //0 = grab, 1 = other
        public AttackSyncDirection SyncDirection = 0; //0 = grab, 1 = other
        public AttackSyncCategory SyncCategory = 0;

        //Movement
        public bool MoveSync; //Moves character around, rotates with lever, scripted movement
        public float MoveSpeed = 0.08f;
        public bool InvertDirection = false;
        public bool Loop = false;

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((uint)SyncType);
            writer.Write((uint)SyncDirection);
            writer.Write((uint)SyncCategory);
            writer.Write(MoveSync);
            writer.Write(MoveSpeed);
            writer.Write(InvertDirection);
            writer.Write(Loop);
        }

        internal override void Read(BinaryReader reader, uint version)
        {
            base.Read(reader, version);

            if (version > 1)
                SyncType = (AttackSyncType)reader.ReadUInt32();
            if (version > 4)
                SyncDirection = (AttackSyncDirection)reader.ReadUInt32();
            if (version > 6)
                SyncCategory = (AttackSyncCategory)reader.ReadUInt32();
            if (version > 2)
            {
                MoveSync = reader.ReadBoolean();
                MoveSpeed = reader.ReadSingle();
                InvertDirection = reader.ReadBoolean();
            }
            if (version > 3)
                Loop = reader.ReadBoolean();
        }

        public override bool IsSyncMove()
        {
            return true;
        }

        public override bool AllowHActWhileExecuting()
        {
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class HeatActionAttack
    {
        public string Name = "HACT0000_DUMMY";
        public uint HactID;
        public Attack StartupAttack; //Version 3: Attack to play before hact starts
        public HeatActionAttackFlags Flags;
        public bool PreferHActPosition;
        public bool DontAllowMovementOnLinkOut;
        public bool IsFollowupOnly;

        public float[] Position = new float[3];
        public float RotationY; //Version 1: Do we really need an euler angle? I think not.

        public HeatActionSpecialType SpecialType; //Version 4
        public HeatActionRangeType Range = HeatActionRangeType.None; //Version 5

        public bool UseMatrix = false;
        public CoordinateMatrix Mtx = new CoordinateMatrix();

        public List<HeatActionActor> Actors = new List<HeatActionActor>();

        internal void Write(BinaryWriter writer)
        {
            writer.Write(Name.ToLength(32).ToCharArray());
            writer.Write(HactID);
            
            writer.Write(StartupAttack != null ? 1 : 0);
           
            if(StartupAttack != null)
                StartupAttack.Write(writer);

            writer.Write((uint)Flags);
            writer.Write(PreferHActPosition);
            writer.Write(DontAllowMovementOnLinkOut);
            writer.Write(IsFollowupOnly);
            
            foreach (float f in Position)
                writer.Write(f);

            writer.Write(RotationY);
            writer.Write((uint)SpecialType);
            writer.Write((uint)Range);

            writer.Write(UseMatrix);

            if(UseMatrix)
                writer.Write(Mtx);    

            writer.Write(Actors.Count);

            foreach (HeatActionActor actor in Actors)
                actor.Write(writer);

        }

        internal void Read(BinaryReader reader, uint version)
        {
            Name = Encoding.UTF8.GetString(reader.ReadBytes(32)).Split(new[] { '\0' }, 2)[0];
            HactID = reader.ReadUInt32();
            
            bool hasStartupAttack = reader.ReadUInt32() == 1;

            if (hasStartupAttack)
                StartupAttack = Attack.ReadFromBuffer(reader, YFC.VERSION);
            
            Flags = (HeatActionAttackFlags)reader.ReadUInt32();
            PreferHActPosition = reader.ReadBoolean();

            if (version > 7)
                DontAllowMovementOnLinkOut = reader.ReadBoolean();

            if (version > 9)
                IsFollowupOnly = reader.ReadBoolean();

            for (int i = 0; i < 3; i++)
                Position[i] = reader.ReadSingle();

            RotationY = reader.ReadSingle();
            SpecialType = (HeatActionSpecialType)reader.ReadUInt32();
            Range = (HeatActionRangeType)reader.ReadUInt32();

            if(version > 6)
            {
                UseMatrix = reader.ReadBoolean();

                if (UseMatrix)
                    Mtx = reader.ReadCoordinateMtx();
            }

            int actorCount = reader.ReadInt32();

            for(int i = 0; i < actorCount; i++)
            {
                HeatActionActor actor = new HeatActionActor();
                actor.Read(reader, version);
                Actors.Add(actor);
            }
        }
    }
}

using System.IO;

namespace YazawaCommand
{

    /// <summary>
    /// CFC Attack that positions you in a range. Optionally may lerp
    /// </summary>
    public class AttackCFCRange : Attack
    {
        public override AttackType AttackType => AttackType.MoveCFCRange;

        public HeatActionRangeType Range = HeatActionRangeType.Invalid;

        public float OffsetLeft = 0;
        public float OffsetUp= 0;
        public float OffsetForward = 0;

        public float LerpDuration = 0;
        public float LerpIntensity = 0;

        public ushort MovesetID = 0;
        public ushort Index = 0;

        internal override void Read(BinaryReader reader, uint version)
        {
            base.Read(reader, version);

            Range = (HeatActionRangeType)reader.ReadUInt32();

            OffsetLeft = reader.ReadSingle();
            OffsetUp = reader.ReadSingle();
            OffsetForward = reader.ReadSingle();

            LerpDuration = reader.ReadSingle();
            LerpIntensity = reader.ReadSingle();

            MovesetID = reader.ReadUInt16();
            Index = reader.ReadUInt16();
        }

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write((uint)Range);

            writer.Write(OffsetLeft);
            writer.Write(OffsetUp);
            writer.Write(OffsetForward);

            writer.Write(LerpDuration);
            writer.Write(LerpIntensity);

            writer.Write(MovesetID);
            writer.Write(Index);
        }
    }
}

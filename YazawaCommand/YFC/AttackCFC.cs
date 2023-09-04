using System.IO;

namespace YazawaCommand
{
    /// <summary>
    /// An attack from battle/fighter_command.cfc
    /// </summary>
    public class AttackCFC : Attack
    {
        public override AttackType AttackType => AttackType.MoveCFC;

        public ushort MovesetID = 0;
        public ushort Index = 0;

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(MovesetID);
            writer.Write(Index);
        }

        internal override void Read(BinaryReader reader, uint version)
        {
            base.Read(reader, version);

            MovesetID = reader.ReadUInt16();
            Index = reader.ReadUInt16();
        }
    }
}

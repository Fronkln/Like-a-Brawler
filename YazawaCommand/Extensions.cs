using System.IO;

namespace YazawaCommand
{
    internal static class Extensions
    {
        public static void Write(this BinaryWriter writer, CoordinateMatrix mtx)
        {
            writer.Write(mtx.LeftDirection.x);
            writer.Write(mtx.LeftDirection.y);
            writer.Write(mtx.LeftDirection.z);
            writer.Write(mtx.LeftDirection.w);

            writer.Write(mtx.UpDirection.x);
            writer.Write(mtx.UpDirection.y);
            writer.Write(mtx.UpDirection.z);
            writer.Write(mtx.UpDirection.w);

            writer.Write(mtx.ForwardDirection.x);
            writer.Write(mtx.ForwardDirection.y);
            writer.Write(mtx.ForwardDirection.z);
            writer.Write(mtx.ForwardDirection.w);

            writer.Write(mtx.Coordinates.x);
            writer.Write(mtx.Coordinates.y);
            writer.Write(mtx.Coordinates.z);
            writer.Write(mtx.Coordinates.w);
        }

        public static CoordinateMatrix ReadCoordinateMtx(this BinaryReader reader)
        {
            CoordinateMatrix mtx = new CoordinateMatrix();
            mtx.LeftDirection = new CoordinateMatrix.Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            mtx.UpDirection = new CoordinateMatrix.Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            mtx.ForwardDirection = new CoordinateMatrix.Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            mtx.Coordinates = new CoordinateMatrix.Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            return mtx;
        }
    }
}

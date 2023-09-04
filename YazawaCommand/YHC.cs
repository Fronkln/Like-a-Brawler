using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class YHC
    {
        public const uint VERSION = 10;

        public List<HeatActionAttack> Attacks = new List<HeatActionAttack>();

        public static void Write(YHC yhc, string filePath)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write("JHRINO_YHC".ToCharArray());
            writer.Write(new byte[2]);
            writer.Write(VERSION);

            writer.Write(yhc.Attacks.Count);
            writer.Write(new byte[12]);

            foreach (HeatActionAttack atk in yhc.Attacks)
            {
                atk.Write(writer);
            }

            File.WriteAllBytes(filePath, ms.ToArray());
        }

        public static YHC Read(string path)
        {
            if (!File.Exists(path))
                return null;

            YHC yhc = new YHC();

            BinaryReader reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));

            string magic = Encoding.UTF8.GetString(reader.ReadBytes(10));

            if (magic != "JHRINO_YHC")
                return null;

            reader.BaseStream.Position += 2;
            int version = reader.ReadInt32();

            if (version > VERSION)
                return null;

            int attacksCount = reader.ReadInt32();
            reader.BaseStream.Position += 12;

            for (int i = 0; i < attacksCount; i++)
            {
                HeatActionAttack attack = new HeatActionAttack();
                attack.Read(reader, (uint)version);
                yhc.Attacks.Add(attack);
            }

            return yhc;
        }
    }
}

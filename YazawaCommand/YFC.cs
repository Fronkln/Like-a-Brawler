using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YazawaCommand
{
    public class YFC
    {
        public const uint VERSION = 7;

        public string Name = "PLAYER_MOVESET";

        public List<AttackGroup> Groups = new List<AttackGroup>();


        public static void Write(YFC file, string path)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write("JHRINO_YFC".ToCharArray());
            writer.Write(new byte[2]);
            writer.Write(VERSION);

            writer.Write(file.Groups.Count);
            writer.Write(new byte[12]);

            foreach(AttackGroup group in file.Groups)
            {
                writer.Write(group.Name.ToLength(32).ToCharArray());
                writer.Write(group.Attacks.Count);

                foreach(Attack attack in group.Attacks)
                    attack.Write(writer);
            }


            File.WriteAllBytes(path, ms.ToArray());
        }

        public static YFC Read(string path)
        {
            if (!File.Exists(path))
                return null;

            BinaryReader reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));
            YFC yfc = new YFC();

            string magic = Encoding.UTF8.GetString(reader.ReadBytes(10));

            if (magic != "JHRINO_YFC")
                return null;

            reader.BaseStream.Position += 2;
            uint version = reader.ReadUInt32();

            if (version > VERSION)
                return null;

            int groupsCount = reader.ReadInt32();
            reader.BaseStream.Position += 12;

            for(int i = 0; i < groupsCount; i++)
            {
                AttackGroup group = new AttackGroup();
                group.Read(reader, version);
                yfc.Groups.Add(group);
            }

            return yfc;
        }
    }
}
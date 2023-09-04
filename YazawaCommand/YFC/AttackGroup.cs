using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class AttackGroup
    {
        public string Name = "Group";
        public List<Attack> Attacks = new List<Attack>();

        internal void Read(BinaryReader reader, uint version)
        {
            Name = Encoding.UTF8.GetString(reader.ReadBytes(32)).Split(new[] { '\0' }, 2)[0];
            int attacksCount = reader.ReadInt32();

            for (int i = 0; i < attacksCount; i++)
                Attacks.Add(Attack.ReadFromBuffer(reader, version));
        }

        public Attack[] GetAllCounterAttacks()
        {
            return Attacks.Where(x => x.IsCounterAttack()).ToArray();
        }
    }
}

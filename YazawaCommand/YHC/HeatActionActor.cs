using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class HeatActionActor
    {
        public HeatActionActorType Type = HeatActionActorType.Fighter;
        /// <summary>
        /// Optional? HAct can still play without their presence.
        /// </summary>
        public bool Optional = false;
        public List<HeatActionCondition> Conditions = new List<HeatActionCondition>();

        internal void Write(BinaryWriter writer)
        {
            writer.Write((uint)Type);
            writer.Write(Optional);
            writer.Write(Conditions.Count);

            foreach (HeatActionCondition cond in Conditions)
                cond.Write(writer);
        }

        internal void Read(BinaryReader reader, uint version)
        {
            Type = (HeatActionActorType)reader.ReadUInt32();

            if (version > 8)
                Optional = reader.ReadBoolean();

            int condCount = reader.ReadInt32();

            for(int i = 0; i < condCount; i++)
            {
                HeatActionCondition cond = new HeatActionCondition();
                cond.Read(reader, version);
                Conditions.Add(cond);
            }
        }
    }
}

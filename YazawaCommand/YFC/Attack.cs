using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    /// <summary>
    /// Base class for all attacks.
    /// </summary>
    public abstract class Attack
    {
        public string Name = "ATTACK";

        public virtual AttackType AttackType => AttackType.MoveBase;

        /// <summary>
        /// Length in seconds before the move can be utilized again
        /// </summary>
        public float Cooldown;
        public bool DisableControlLock = false;
        public List<AttackCondition> Conditions = new List<AttackCondition>();

        public int TransitionGroup = -1;

        public virtual bool AllowHActWhileExecuting()
        {
            return false;
        }

        public virtual bool IsSyncMove()
        {
            return false;
        }

        public bool HasConditionOfType(AttackConditionType cond)
        {
            return Conditions.FirstOrDefault(x => x.Type == cond) != null;
        }

        public bool IsCounterAttack()
        {
            return Conditions.FirstOrDefault(x => x.Type == AttackConditionType.InputKey && x.Param1I32 == 1) != null;
        }

        public AttackCondition[] GetAllInputConditions()
        {
            return Conditions.Where(x => x.Type == AttackConditionType.InputKey).ToArray();
        }

        internal virtual void Write(BinaryWriter writer)
        {
            writer.Write(Name.ToLength(32).ToCharArray());
            writer.Write((uint)AttackType);
            writer.Write(Cooldown);
            writer.Write(TransitionGroup);

            writer.Write(Conditions.Count);

            foreach (AttackCondition cond in Conditions)
                cond.Write(writer);
        }

        internal virtual void Read(BinaryReader reader, uint version)
        {
        }

        internal static Attack ReadFromBuffer(BinaryReader reader, uint version)
        {
            string name = Encoding.UTF8.GetString(reader.ReadBytes(32)).Split(new[] { '\0' }, 2)[0];
            AttackType attackType = (AttackType)reader.ReadUInt32();

            Attack attack = null;

            switch(attackType)
            {
                default:
                    throw new Exception("Yazawa Command: Unknown Attack Type " + (uint)attackType);

                case AttackType.MoveCFC:
                    attack = new AttackCFC();
                    break;
                case AttackType.MoveRPG:
                    attack = new AttackRPG();
                    break;
                case AttackType.MoveGMTOnly:
                    attack = new AttackGMT();
                    break;
                case AttackType.MoveSidestep:
                    attack = new AttackQuickstep();
                    break;
                case AttackType.MoveSync:
                    attack = new AttackSync();
                    break;
                case AttackType.MoveEmpty:
                    attack = new AttackEmpty();
                    break;
                case AttackType.MoveCFCRange:
                    attack = new AttackCFCRange();
                    break;
            }

            attack.Name = name;
            attack.Cooldown = reader.ReadSingle();
            attack.TransitionGroup = reader.ReadInt32();
            int conditionCount = reader.ReadInt32();

            for (int i = 0; i < conditionCount; i++)
            {
                AttackCondition cond = new AttackCondition();
                cond._parent = attack;
                cond.Read(reader);
                attack.Conditions.Add(cond);
            }

            attack.Read(reader, version);

            return attack;
        }
    }
}

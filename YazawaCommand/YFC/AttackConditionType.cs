using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    [Flags]
    public enum AttackConditionType : uint
    {
        None = 0,
        AttackHit = 1,
        Down = 2,
        GettingUp = 4,
        LowHealth = 8,
        LockedEnemyDown = 16,
        CanAttackOverall = 32,
        InputKey = 64,
        CharacterLevel = 128,
        CharacterJobLevel = 256,
        Running = 512,
        AnimationOver = 1024,
        IsFlinching = 2048,
        LockedToEnemy = 4096,
        IsExtremeHeat = 8192,
        JobID = 16384,
        Sync = 32768,
        MoveInput = 65536,
        EnemyResponse = 131072,
        SyncType = 262144,
        SyncDirection = 524288,
        DistanceToRange = 1048576,
        FacingRange = 2097152,
        NearestEnemyFlag = 4194304
    }
}

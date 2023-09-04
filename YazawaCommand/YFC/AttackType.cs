using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public enum AttackType : uint
    {
        MoveBase,
        MoveRPG,
        MoveGMTOnly,
        MoveHeatAction,
        MoveCFC,
        MoveSidestep,
        MoveSync,
        MoveEmpty,
        MoveCFCRange
    }
}

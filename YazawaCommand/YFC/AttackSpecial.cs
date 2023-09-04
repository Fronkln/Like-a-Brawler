using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    //An attack that literally does nothing, could be useful for setting up free transitions
    public class AttackEmpty : Attack
    {
        public override AttackType AttackType => AttackType.MoveEmpty;
    }
}

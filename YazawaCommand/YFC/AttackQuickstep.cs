using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class AttackQuickstep : Attack
    {
        public override AttackType AttackType => AttackType.MoveSidestep;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    public static class BattleCamera
    {
        //Phase 0 = Look at and move with Ichiban
        //Phase 1 = Look at nearest enemy and dont move
        //Phase 2 = Look at Ichiban
        public static int Phase = 0;
    }
}

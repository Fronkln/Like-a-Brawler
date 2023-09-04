using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal class ParticleHook
    {
        //if effect_hit_wound_param.bin didnt store its data in some cursed format
        //i wouldnt have to do this, but it is what it is!
        public static uint DetermineID(uint particleID)
        {
            switch(particleID)
            {
                default:
                    return particleID;

                //HYa0001 (original) -> BHya0001 (Y7B pib)
                case 12399:
                    return 15154;
            }
        }
    }
}

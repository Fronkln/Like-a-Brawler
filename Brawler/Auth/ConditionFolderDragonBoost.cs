using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler.Auth
{
    public static class ConditionFolderDragonBoost
    {
        public static bool Check(IntPtr disableInfo, IntPtr node)
        {
            return !BrawlerPlayer.IsEXGamer;
        }
    }
}

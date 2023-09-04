using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler.Utils
{
    internal class DETaskWaitAnimStart : DETask
    {
        public DETaskWaitAnimStart(ECMotion motion, Action onFinish, bool autoStart = true) : base(null, onFinish, autoStart)
        {
            MotionID curGmt = motion.GmtID;

            m_Func = delegate { return motion.GmtID != 0 && motion.PlayInfo.tick_gmt_now_ < 1000 && motion.PlayInfo.tick_old_ > 1000; };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonEngineLibrary;

namespace Brawler
{
    internal class DETaskWaitRequestedAnim : DETask
    {
        public DETaskWaitRequestedAnim(ECMotion motion, Action onFinish, bool autoStart = true) : base(null, onFinish, autoStart)
        {
            MotionID curGmt = motion.GmtID;
            uint tick = motion.PlayInfo.tick_gmt_now_;

            m_Func = delegate { return motion.GmtID == 0 || curGmt != motion.GmtID || motion.PlayInfo.tick_gmt_now_ < tick; };
        }
    }
}

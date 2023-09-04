using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal class DETaskNextFrame : DETask
    {
        public DETaskNextFrame() : base(null, null, false)
        {
            m_Func = delegate { return true; };
        }

        public DETaskNextFrame(Action onFinish) : base(null, onFinish, false)
        {
           m_Func = delegate { return true; };
        }

        public DETaskNextFrame(Action onFinish, bool autoStart = true) : base(null, onFinish, autoStart)
        {
            m_Func = delegate { return true; };
        }
    }
}

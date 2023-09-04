using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonEngineLibrary;

namespace Brawler
{
    internal class RepeatingTask : DETask
    {
        private float m_tickRate = 0;
        private float m_curTick = 0;

        public bool Paused = false;

        private Action m_Action;

        public RepeatingTask(Action task, float interval) : base(null, null)
        {
            m_Action = task;
            m_tickRate = interval;
        }


        public void StopFully()
        {
            DETaskManager.Tasks.Remove(this);
        }

        public override void Run()
        {
            if (Paused)
                return;

           if(m_tickRate > 0)
            {
                m_curTick += DragonEngine.deltaTime;

                if (m_curTick >= m_tickRate)
                {
                    m_curTick = 0;
                    m_Action?.Invoke();
                }
            }
        }
    }
}

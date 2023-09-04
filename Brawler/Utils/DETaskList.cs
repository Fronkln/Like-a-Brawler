using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal class DETaskList : DETask
    {
        private DETask[] m_Tasks;

        private bool m_Done = true;
        int m_curTask = 0;


        public DETaskList(params DETask[] tasks) : base(null, null, true)
        {
            m_Tasks = tasks;
            m_Func = delegate { return m_Done; };

            DETaskManager.Tasks.Add(m_Tasks[m_curTask]);
        }

        public override void Run()
        {
            if (m_curTask >= m_Tasks.Length)
            {
                m_Done = true;
                base.Run();
                return;
            }


            if (m_Tasks[m_curTask].m_Func())
            {
                //m_Tasks[m_curTask].m_FinishFunc?.Invoke();
                m_curTask++;

                if(m_curTask < m_Tasks.Length)
                    DETaskManager.Tasks.Add(m_Tasks[m_curTask]);
            }
        }
    }
}

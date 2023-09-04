using System;
using System.Threading;
using DragonEngineLibrary;

namespace Brawler
{
    public class DETask
    {
        public Func<bool> m_Func = null;
        public Action m_FinishFunc = null;

        public bool Fail = false;
        public bool Success = false;

        public DETask(Func<bool> condition, Action onFinish, bool autoStart = true)
        {
            m_Func = condition;
            m_FinishFunc = onFinish;

            if (autoStart)
                StartTask();
        }

        public void StartTask()
        {
            DETaskManager.Tasks.Add(this);
        }

        public virtual void Run()
        {
            if (m_Func?.Invoke() == true)
            {
                DETaskManager.Tasks.Remove(this);
                m_FinishFunc?.Invoke();
                Success = true;
            }
        }

    }
}

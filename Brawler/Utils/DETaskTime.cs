using System;
using System.Timers;
using DragonEngineLibrary;

namespace Brawler
{
    internal class DETaskTime : DETask
    {
        public float Time = 0;

        private float m_targetTime = 0;
        private Func<bool> m_proc = null;

        private bool m_gameTime = true;

        private Timer m_timer;

        public DETaskTime(float time, Action onFinish, bool autoStart = true, Func<bool> proc = null, bool gameTime = true) : base(null, onFinish, autoStart)
        {
            if (gameTime)
            {
                m_targetTime = time;
            }
            else
            {
                if (time <= 0)
                    time = 0.01f;
                else
                    m_targetTime = time;

                Timer timer = new Timer();
                timer.Interval = 10;
                timer.AutoReset = true;
                timer.Elapsed += delegate { if(!Mod.IsGamePaused) Time += 0.01f; };
                m_timer = timer;

                if (autoStart)
                    m_timer.Enabled = true;

            }

            m_Func = delegate { return Time >= m_targetTime; };

            m_proc = proc;
            m_gameTime = gameTime;
        }

        public override void Run()
        {

            if(m_gameTime)
                Time += DragonEngine.deltaTime;
            else
                if (!m_timer.Enabled)
                    m_timer.Enabled = true;

            if (m_proc?.Invoke() == true)
            {
                m_FinishFunc?.Invoke();
                Success = true;
                return;
            }
                

            base.Run();
        }
    }
}

using System;
using System.Threading.Tasks;
using DragonEngineLibrary;

namespace Brawler
{
    public class DETaskAsync : DETask
    {
        private Func<bool> m_Func = null;
        private Action m_FinishFunc = null;

        public DETaskAsync(Func<bool> condition) : base(condition, null)
        {
            m_Func = condition;

            DETaskManager.Tasks.Add(this);
        }

        public async Task Await()
        {
            await Task.Run(
                delegate 
                { 
                    while(!m_Func())
                        continue;
                });

            DETaskManager.Tasks.Remove(this);
        }

        public override void Run()
        {

        }
    }
}

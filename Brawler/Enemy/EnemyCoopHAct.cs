using System;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyCoopHAct
    {
        public TalkParamID HAct;
        public Func<Fighter[], bool> Condition;


        public bool CanPerform(Fighter[] performers)
        {
            return Condition.Invoke(performers);
        }
    }
}

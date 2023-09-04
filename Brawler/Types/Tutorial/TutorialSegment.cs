using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal class TutorialSegment
    {
        public string Instructions;
        public float TimeToComplete = 45f; //45 seconds
        public bool TimeoutIsSuccess = false;
        public TutorialModifier Modifiers = TutorialModifier.None;
        public Func<bool> IsCompleteDelegate;

        //No UI, immediate completion
        public bool Silent;

        public Action OnStart;
        public Action OnEnd;
        public Action UpdateDelegate;

        public float var1;
        public bool var2;

        public bool HasTime()
        {
            return TimeToComplete > 0;
        }
    }
}

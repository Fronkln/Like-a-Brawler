using System;
using System.Runtime.InteropServices;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAIAoki : EnemyAIBoss
    {
        public override bool CanDieOnHAct()
        {
            return false;
        }
    }
}

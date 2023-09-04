using System;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyMoveSync
    {
        /// <summary>
        /// Sync.
        /// </summary>
        public RPGSkillID Sync;
        /// <summary>
        /// Animation to check if it hit
        /// </summary>
        public MotionID GrabbedAnim;

        public Action<Fighter[]> OnHitSuccess = null;
    }
}

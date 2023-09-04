using System;
using DragonEngineLibrary;

namespace Brawler
{
    //Grab kasuga > lead into a hact
    //Is it ideal? No
    //Do we have not much choice? Also no
    internal class EnemySyncHActModule : EnemyModule
    {

        private EnemyMoveSync m_currentSync = null;
        private bool m_waitDoOnce = false;

        public void StartProcedure(Fighter initiator, EnemyMoveSync sync)
        {
            m_currentSync = sync;
            BattleTurnManager.ForceCounterCommand(initiator, BrawlerBattleManager.Kasuga, sync.Sync);

            new DETaskList(new DETask[]
            {
                new DETaskNextFrame(),
                new DETask(delegate{return AI.Character.IsSync(); }, delegate{m_waitDoOnce = true; }, false),
            });
        }

        public override void Update()
        {
            base.Update();

            if (m_currentSync != null && m_waitDoOnce)
            {
                 
                if (!AI.Character.IsSync())
                {
                 //   m_currentSync = null;
                    m_waitDoOnce = false;
                    return;
                }
                

                //TODO: Use followup nodes to determine when to start the hact

                MotionID gmt = AI.Chara.Get().GetMotion().GmtID;

                if (gmt == m_currentSync.GrabbedAnim)
                {
                    //  AI.Chara.Get().HumanModeManager.ToEndReady();
                    //  BrawlerBattleManager.KasugaChara.HumanModeManager.ToEndReady();

                    DETaskList list = new DETaskList(new DETask[]
                    {
                        new DETaskNextFrame(),
                        new DETaskTime(1, delegate
                        {

                            m_currentSync.OnHitSuccess?.Invoke(null);
                         //   m_currentSync = null;
                            m_waitDoOnce = false;
                        }, false),

                    });

                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using DragonEngineLibrary;
using DragonEngineLibrary.Service;

namespace Brawler
{
    public static class EXFollowups
    {
        private static bool m_Processing;
        private static MotionID m_processingGMT;
        private static int m_processingNearestFollowup;

        private static float m_processDelay = 0;

        private static Dictionary<MotionID, HeatAction> m_exFollowups = new Dictionary<MotionID, HeatAction>()
        {
            [MotionID.P_ICH_BTL_SUD_sy0_ztk_lp] = new HeatAction((TalkParamID)12888, HeatActionCondition.None, 5, 0, 1, 5f), //EX-Grab
            [(MotionID)17066] = new HeatAction((TalkParamID)12889, HeatActionCondition.None, 5, 0, 1, 5f), //EX-Parry
            [MotionID.P_ICH_BTL_SUD_sy0_german] = new HeatAction((TalkParamID)12882, HeatActionCondition.None, 5, 0, 1, 5f), //EX-Grab
            [(MotionID)17128] = new HeatAction((TalkParamID)888, HeatActionCondition.None, 5, 0, 1, 5f), //EX-Grab
            [(MotionID)17147] = new HeatAction((TalkParamID)12919, HeatActionCondition.None, 5, 0, 1, 5f), //EX-Grab
        };

        public static void Update()
        {
            return;

            if (Mod.IsGamePaused)
                return;

            if (!BrawlerBattleManager.BattleStartDoOnce)
                return;

            MotionID gmt = BrawlerBattleManager.KasugaChara.GetMotion().PlayInfo.gmt_id_;
            int gmtTick = (int)DragonEngine.GetHumanPlayer().GetMotion().PlayInfo.tick_gmt_now_;

            if (!m_Processing)
            {
                if (m_processDelay > 0)
                    m_processDelay -= DragonEngine.deltaTime;
                else
                {
                    if (m_exFollowups.ContainsKey(gmt) && BrawlerPlayer.IsEXGamer)
                        StartEXFollowup(gmt);
                }
            }

            if (m_Processing)
            {
                ECMotion ecMotion = BrawlerBattleManager.KasugaChara.GetMotion();
                MotionPlayInfo inf = DragonEngine.GetHumanPlayer().GetMotion().PlayInfo;

                if (inf.gmt_id_ != m_processingGMT)
                    ExecEXFollowup();
                else
                    if (m_processingNearestFollowup != -1)
                    if (gmtTick >= m_processingNearestFollowup)
                        ExecEXFollowup();
            }
        }

        private static void StartEXFollowup(MotionID motion)
        {
            m_Processing = true;
            m_processDelay = 1;

            ECMotion ecMotion = BrawlerBattleManager.KasugaChara.GetMotion();
            m_processingNearestFollowup = 0;

            uint bepID = ecMotion.BepID;

            if (bepID > 0)
                m_processingNearestFollowup = (int)MotionService.SearchTimingDetail(0, bepID, 71).Start;

            if (m_processingNearestFollowup == 0)
                m_processingNearestFollowup = -1;

            m_processingGMT = motion;
        }

        private static void ExecEXFollowup()
        {
            BrawlerPlayer.ExecuteMove(m_exFollowups[m_processingGMT], BrawlerBattleManager.Kasuga, BrawlerBattleManager.Enemies, true);
            m_Processing = false;
        }

    }
}

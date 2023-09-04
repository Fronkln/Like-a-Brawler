using System;
using System.Collections.Generic;
using System.Linq;
using DragonEngineLibrary;

namespace Brawler
{
    public class DETaskChainHAct : DETask
    {
        private bool m_awaitingPlay = false;
        private bool m_done = false;

        List<TalkParamID> ids = new List<TalkParamID>();

        public DETaskChainHAct(Action onFinish, bool autoStart, params TalkParamID[] ids) : base(null, onFinish, autoStart)
        {
            m_Func = delegate { return m_done; };
            this.ids = ids.ToList();
        }

        public override void Run()
        {
            if (!HActManager.IsPlaying())
            {
                if (m_awaitingPlay)
                    return;
                else
                {
                    if (ids.Count > 0)
                    {
                        m_awaitingPlay = true;
                        HActRequestOptions opts = new HActRequestOptions();
                        opts.can_skip = false;
                        opts.id = ids[0];
                        opts.is_force_play = true;
                        opts.base_mtx.matrix = BrawlerBattleManager.KasugaChara.GetPosture().GetRootMatrix();

                        opts.Register(HActReplaceID.hu_player, BrawlerBattleManager.KasugaChara);
                        DragonEngine.Log(HActManager.RequestHActProc(opts));

                        DragonEngine.Log("hact");

                        ids.RemoveAt(0);
                    }
                    else
                    {
                        m_done = true;
                        m_FinishFunc?.Invoke();
                        Success = true;
                        return;
                    }
                }
            }
            else
                m_awaitingPlay = false;

            base.Run();
        }
    }
}

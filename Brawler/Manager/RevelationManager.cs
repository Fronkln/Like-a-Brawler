using System;
using System.IO;
using System.Collections.Generic;
using DragonEngineLibrary;
using System.Linq;

namespace Brawler
{
    internal static  class RevelationManager
    {
        private static float m_validTime;
        private static bool m_revelationProcedure = false;
        private static uint m_lastPlayerLevel = 0;

        private static Dictionary<uint, List<TalkParamID>> m_revelationMap = new Dictionary<uint, List<TalkParamID>>();
        private static List<TalkParamID> m_revelationQueue = new List<TalkParamID>();
        
        public static void Init()
        {
            ReadRevelationsFile();

            BrawlerBattleManager.OnBattleStart += OnBattleStart;
            BrawlerBattleManager.OnBattleEnd += OnBattleEnd;

            DragonEngine.RegisterJob(Update, DEJob.Update);
        }


        private static void ReadRevelationsFile()
        {
            m_revelationMap.Clear();

            string revelationsFilePath = Path.Combine(Mod.ModPath, "mod_config", "revelations.txt");

            if (!File.Exists(revelationsFilePath))
                return;

            string[] revelationsTxt = File.ReadAllLines(revelationsFilePath);

            foreach(string revelationEntry in revelationsTxt)
            {
                string line = revelationEntry;

                if(line.Contains("//"))
                    line = line.Remove(line.IndexOf("//"));

                if (string.IsNullOrEmpty(line))
                    continue;

                try
                {
                    string[] revelationDat = line.Split(' ');
                    
                    if (revelationDat.Length < 2)
                        continue;

                    uint level = uint.Parse(revelationDat[0]);
                    TalkParamID hact = (TalkParamID)uint.Parse(revelationDat[1]);

                    if (!m_revelationMap.ContainsKey(level))
                        m_revelationMap.Add(level, new List<TalkParamID>());

                    m_revelationMap[level].Add(hact);
                }
                catch { }
            }
        }

        public static void Update()
        {
            if (!ShouldDoRevelationProcedure())
                m_validTime = 0;
            else
                m_validTime += DragonEngine.deltaTime;

            if (m_revelationQueue.Count <= 0)
                return;

            if (!m_revelationProcedure)
            {
                if (m_validTime > 0.2f)
                {
                    m_revelationProcedure = true;
                    DoRevelationProcedure();
                }

            }
        }

        private static void DoRevelationProcedure()
        {
            TalkParamID hactID = (TalkParamID)12939;

            HActRequestOptions opts = new HActRequestOptions();
            opts.can_skip = false;
            opts.id = hactID;
            opts.is_force_play = true;
            opts.base_mtx.matrix = BrawlerBattleManager.KasugaChara.GetPosture().GetRootMatrix();

            opts.Register(HActReplaceID.hu_player, BrawlerBattleManager.KasugaChara);
            HActManager.RequestHActProc(opts);

            DragonEngine.Log("OH SHIT!");


            new DETask(delegate { return !HActManager.IsPlaying(); }, 
                delegate 
                {
                    new DETaskChainHAct(delegate { m_revelationProcedure = false; m_revelationQueue.Clear(); }, true, m_revelationQueue.ToArray());
                    /*
                    new DETask(
                        delegate 
                        {
                            if (!HActManager.IsPlaying() && !AuthManager.PlayingScene.IsValid())
                            {
                                PlayRevelation();
                                DragonEngine.Log("HIRAMETA!");
                            }

                            return m_revelationQueue.Count <= 0;
                        }, delegate { m_revelationProcedure = false; });
                    */
                }, true);
        }

        private static void PlayRevelation()
        {
            TalkParamID hactID = m_revelationQueue[0];

            HActRequestOptions opts = new HActRequestOptions();
            opts.can_skip = false;
            opts.id = hactID;
            opts.is_force_play = true;

            opts.Register(HActReplaceID.hu_player, BrawlerBattleManager.KasugaChara);
            HActManager.RequestHActProc(opts);


            m_revelationQueue.RemoveAt(0);
        }

        private static bool ShouldDoRevelationProcedure()
        {
            if (!BrawlerBattleManager.Kasuga.IsValid())
                return false;

            if (BattleTurnManager.CurrentPhase != BattleTurnManager.TurnPhase.Action)
                return false;

            if (HActManager.IsPlaying() || AuthManager.PlayingScene.IsValid())
                return false;
            
            if (GameVarManager.GetValueBool(GameVarID.is_scene_fading))
                return false;
            
            if (GameVarManager.GetValueBool(GameVarID.is_hact))
                return false;

            if (GameVarManager.GetValueBool(GameVarID.is_talk))
                return false;

            if (GameVarManager.GetValueBool(GameVarID.is_pause))
                return false;


            if (GameVarManager.GetValueBool(GameVarID.is_ui_loading))
                return false;

            return true;
        }

        //Adds revelations to the queue if we just learnt any moves/hacts
        //defined in revelations.txt
        private static void CheckRevelationEligibility()
        {
            for(uint i = m_lastPlayerLevel + 1; i < Player.GetLevel(Player.ID.kasuga) + 1; i++)
            {
                if (!m_revelationMap.ContainsKey(i))
                    continue;

                m_revelationQueue.AddRange(m_revelationMap[i]);
            }
        }

        private static void OnBattleStart()
        {
            m_lastPlayerLevel = Player.GetLevel(Player.ID.kasuga);
        }

        private static void OnBattleEnd()
        {
            if(Player.GetLevel(Player.ID.kasuga) > m_lastPlayerLevel)
                CheckRevelationEligibility();
        }
    }
}

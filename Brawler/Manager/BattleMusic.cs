using System;
using System.Runtime.InteropServices;
using DragonEngineLibrary;
using DragonEngineLibrary.Service;
using MinHook.NET;

namespace Brawler
{
    internal static class BattleMusic
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void SoundManagerPlayBGM(IntPtr mng, uint port_kind, uint se_id, uint priority, int start_msec, float fade_in_sec, float fade_out_sec);

        private static SoundManagerPlayBGM m_playBgmDelegate;
        private static SoundManagerPlayBGM m_playBgmTrampoline;


        public static void Init()
        {
            BrawlerBattleManager.OnBattleEnd += OnBattleEnd;
        }


       public static void OnBattleEnd()
        {
            if (BrawlerBattleManager.IsEncounter)
                return;

            //Outro
            switch(SoundManager.GetBGMSeID())
            {
                //Asphodelos
                case 0x11B80002:
                    SoundManager.PlayBGM(5595, 5);
                    break;
                //Pellagra
                case 0x11AD0004:
                    SoundManager.PlayBGM(5603, 1);
                    break;
                //Ascension Point
                case 0x11B80001:
                    SoundManager.PlayBGM(5595, 3);
                    break;
                //Yokohama Crackhouse
                case 0x11AD0001:
                    SoundManager.PlayBGM(5595, 2);
                    break;
                //Theory of Beauty
                case 297271308:
                    SoundManager.PlayBGM(5595, 1);
                    break;
            }
        }

        public static void Hook()
        {
            m_playBgmDelegate = new SoundManagerPlayBGM(PlayBGMHook);
            MinHookHelper.createHook((IntPtr)0x1411C7DB0, m_playBgmDelegate, out m_playBgmTrampoline);
        }

        private static uint m_lastCombatMusic;
        private static bool m_waitingInitialization;
        private static bool m_allowDefaultMusicOnce;
        private static void PlayBGMHook(IntPtr mng, uint port_kind, uint se_id, uint priority, int start_msec, float fade_in_sec, float fade_out_sec)
        {

            if (m_waitingInitialization)
            {
                se_id = 0;
                m_playBgmTrampoline(mng, port_kind, se_id, priority, start_msec, fade_in_sec, fade_out_sec);
                return;
            }

            if(!m_allowDefaultMusicOnce && ShouldInterceptMusic() && IsCombatMusic(se_id))
            {
                m_waitingInitialization = true;
                m_lastCombatMusic = se_id;

                se_id = 0;

                new DETask(delegate { return BrawlerBattleManager.Enemies.Length > 0; }, 
                    delegate 
                    {
                        m_waitingInitialization = false;
                        se_id = 0;
                        uint decidedTheme = DecideTheme();

                        m_allowDefaultMusicOnce = decidedTheme == m_lastCombatMusic;

                        SoundManager.PlayBGM(decidedTheme);
                        return;
                    });
            }

            m_playBgmTrampoline(mng, port_kind, se_id, priority, start_msec, fade_in_sec, fade_out_sec);


            if (m_allowDefaultMusicOnce)
                m_allowDefaultMusicOnce = false;
        }
        private static bool ShouldInterceptMusic()
        {
            if (!BrawlerBattleManager.KasugaChara.IsValid())
                return false;

            return true;
        }

        private static bool IsCombatMusic(uint id)
        {
            if (id == 0x11B80001 || id == 0x11AD0001 || id == 0x11B80007)
                return true;

            return false;
        }


        private static uint DecideTheme()
        {
            //Tough random encounter: The Myth
            if (BattleProperty.BattleConfigID == 2 && BrawlerBattleManager.Enemies[0].GetStatus().Level > Player.GetLevel(Player.ID.kasuga))
            {
                DragonEngine.Log("The Myth!");
                return 0x15DA0003;
            }

            switch (m_lastCombatMusic)
            {
                case 0x11B80001:
                    return DecideKamurochoEncounterTheme();

                case 0x11B80007:
                    return DecideOmiEncounterTheme();

                //Yokohama
                case 0x11AD0001:
                    return DecideYokohamaEncounterTheme();
            }

            return 0;
        }

        private static uint DecideKamurochoEncounterTheme()
        {

            if (BrawlerBattleManager.IsEncounter)
            {
                //Kamurocho 1999: Push Me Under Water
                if (SceneService.GetSceneInfo().ScenePlay.Get().StageID == StageID.st_kamuro_yazawa_past)
                    return 0x15DA0002;
            }

            return 0x11B80001;
        }

        private static uint DecideYokohamaEncounterTheme()
        {
            return 0x11AD0001;
        }

        private static uint DecideOmiEncounterTheme()
        {
            return 0x15DC0001;
        }
    }
}

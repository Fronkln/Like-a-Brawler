using System;
using System.Linq;
using DragonEngineLibrary;
using System.Collections.Generic;
using DragonEngineLibrary.Service;
using System.Configuration;

namespace Brawler
{
    [Serializable]
    public struct JobTheme
    {
        public ushort Cuesheet;
        public ushort ID;

        public JobTheme(ushort cue, ushort id)
        {
            Cuesheet = cue;
            ID = id;
        }
    }

    [Serializable]
    public class AuraDefinition
    {
        public EffectEventCharaID Loop;
        public EffectEventCharaID End;

        public AuraDefinition(EffectEventCharaID loop, EffectEventCharaID end)
        {
            Loop = loop;
            End = end;
        }
    }

    public static class BrawlerBattleManager
    {
        public const float BattleStartTime = 2.8f;
        public static bool BattleStartDoOnce = false;
        public static float BattleTime = 0;

        public static float TimeSinceBattle = 0;

        public static bool IsEncounter = false;

        public static bool HActIsPlaying = false;
        public static bool CurrentHActIsY7B = false;

        /// <summary>
        /// Used for special fights like crane/excavator
        /// </summary>
        public static bool DisableTargetingOnce = false;
        public static bool AllowPlayerTransformationDoOnce = false;

        public static Dictionary<RPGJobID, AuraDefinition> AuraDefs = new Dictionary<RPGJobID, AuraDefinition>();
        public static Dictionary<RPGJobID, JobTheme> JobThemes = new Dictionary<RPGJobID, JobTheme>();

        static AuraDefinition LastAura = new AuraDefinition(EffectEventCharaID.invalid, EffectEventCharaID.invalid);

        private static uint m_bgmID;
        private static float m_bgmTime;

        public static Fighter Kasuga = new Fighter(IntPtr.Zero);
        public static Character KasugaChara = new Character() { Pointer = IntPtr.Zero };
        public static ECMotion KasugaMotion = new ECMotion();

        public static Fighter[] Enemies = new Fighter[0];
        public static Fighter[] EnemiesNearest = new Fighter[0];

        //Needs to be checked properly to ensure they are actually alive and kicking
        public static Fighter CurrentAttacker = new Fighter((IntPtr)0);

        private static bool m_ResourcesLoaded = false;

        private static bool m_actionDoOnce = false;

        public static Player.ID[] PartyMembersCache = new Player.ID[3];


        public static event Action OnBattleStart = null;
        public static event Action OnBattleEnd = null;

        static BrawlerBattleManager()
        {
            JobThemes = new Dictionary<RPGJobID, JobTheme>()
            {
                [RPGJobID.invalid] = new JobTheme(4536, 8),
                [RPGJobID.man_01] = new JobTheme(5593, 5), //bodyguard
                [RPGJobID.man_05] = new JobTheme(5593, 1), //breaker 
                [RPGJobID.man_06] = new JobTheme(5628, 1), //enforcer
                [RPGJobID.man_07] = new JobTheme(5628, 3), //chef
                [RPGJobID.kasuga_freeter] = new JobTheme(5593, 11), //freelancer / unarmed
                [RPGJobID.kasuga_braver] = new JobTheme(5593, 2), //slugger / bat style
                [RPGJobID.man_kaitaiya] = new JobTheme(5593, 3) //foreman
            };

            AuraDefs = new Dictionary<RPGJobID, AuraDefinition>()
            {
                [RPGJobID.invalid] = new AuraDefinition(EffectEventCharaID.OgrefHeatAuraKr01, EffectEventCharaID.invalid),
                [RPGJobID.kasuga_freeter] = new AuraDefinition(EffectEventCharaID.OgrefHeatAuraKr01, EffectEventCharaID.invalid),
                [RPGJobID.kasuga_braver] = new AuraDefinition(EffectEventCharaID.JudgeSideAuraHentai_lp, EffectEventCharaID.invalid),
                [RPGJobID.man_01] = new AuraDefinition(EffectEventCharaID.JudgeBossAuraHam_lp, EffectEventCharaID.invalid),
                [RPGJobID.man_05] = new AuraDefinition(EffectEventCharaID.JudgeBossAuraSak_lp, EffectEventCharaID.invalid)
            };
        }

        private static void OnAuraChanged()
        {
            StopAura();
        }

        private static void StopAura()
        {
            KasugaChara.Components.EffectEvent.Get().StopEvent(LastAura.Loop, false);
        }

        private static void StartAura()
        {
            KasugaChara.Components.EffectEvent.Get().PlayEvent(LastAura.Loop);
        }

        public static bool IsThereAnyBoss()
        {
            return Enemies.FirstOrDefault(x => x.IsBoss()) != null;
        }

        private static bool ShouldShowHeatAura()
        {
            if (!BrawlerPlayer.IsEXGamer)
                return false;
            else
                return true;

            // if (HActManager.IsPlaying())
            //  return false;

            if (!Kasuga.IsValid() || !BattleStartDoOnce)
                return false;
            else
                return Player.GetHeatNow(Player.ID.kasuga) == Player.GetHeatMax(Player.ID.kasuga);
        }

        public static int GetPartyMemberCount()
        {
            int count = 0;

            for (int i = 1; i < 4; i++)
            {
                if (!NakamaManager.GetCharacterHandle((uint)i).Get().IsValid())
                    return count;

                count++;
            }

            return count;
        }
        
        private static bool EXHeatShouldTransform()
        {
            if (BrawlerBattleManager.IsEncounter)
                return true;

            CharacterID kasugaModel = BrawlerBattleManager.KasugaChara.Attributes.chara_id;

            if ((uint)kasugaModel != 0xCE4 && (uint)kasugaModel != 0x3BB7)
                return false;

            return true;
        }

        public static void OnEXGamerON(bool immediate)
        {
            DragonEngine.Log("is ex cancel: " + immediate);

            m_bgmID = SoundManager.GetBGMSeID();
            m_bgmTime = SoundManager.GetBGMPlaytimeSec();

            RPGJobID playerJob = Player.GetCurrentJob(Player.ID.kasuga);

            AllowPlayerTransformationDoOnce = true;
            SoundManager.PlayCue(SoundCuesheetID.battle_common, 5, 11);

            if (playerJob != RPGJobID.kasuga_freeter)
            {
                new DETaskTime(immediate ? 0.0f : 0.5f, 
                    delegate 
                    {
                        BattleCommandSetID commandSet = RPG.GetJobCommandSetID(Player.ID.kasuga, Player.GetCurrentJob(Player.ID.kasuga));

                        if(EXHeatShouldTransform())
                            Kasuga.Character.GetRender().BattleTransformationOn();

                        BrawlerPlayer.WantSwapJobWeapon = true;
                        Kasuga.Character.GetBattleStatus().ActionCommand = commandSet;
                        Kasuga.Character.SetCommandSet(commandSet);
                        Kasuga.GetStatus().SetSuperArmor(true);
                        BrawlerPlayer.FreezeInput = false;
                        Kasuga.PlayVoice(0x10B);

                    });
            }
            else
            {
                Kasuga.GetStatus().SetSuperArmor(true);
                BrawlerPlayer.FreezeInput = false;
                AttackSimulator.PlayerInstance.ExecuteSingleCFCAttack(new FighterCommandID(1346, 21));
                Kasuga.PlayVoice(0x10B);

                RPGJobID job = Player.GetCurrentJob(Player.ID.kasuga);
            }

            JobTheme theme;

            if (!JobThemes.ContainsKey(playerJob))
                theme = JobThemes[RPGJobID.invalid];
            else
                theme = JobThemes[playerJob];


            if (IsEncounter)
                SoundManager.PlayBGM(theme.Cuesheet, theme.ID);

            //EX Cancel
            if (immediate)
            {
                AttackSimulator.PlayerInstance.LastEnemyHitCurrentCombo = null;
                AttackSimulator.PlayerInstance.m_attacking = false;          
                AttackSimulator.PlayerInstance.CurrentAttack = null;
                AttackSimulator.PlayerInstance.Stop();

                if (immediate)
                    BrawlerPlayer.OnEXCancel();
            }
        }

        public static void OnEXGamerOFF(bool immediate)
        {

            if (!IsThereAnyBoss())
            {
                if (m_bgmID == 0)
                    Console.WriteLine("!!!!!!!!!!!!!!!!!EX GAMER OFF: BGM ID IS 0!!!!!!!!!!!!!!!!");

                new DETaskList
                    (
                        new DETaskNextFrame(),
                        new DETaskNextFrame(delegate { SoundManager.PlayBGM(m_bgmID, (uint)TimeSpan.FromSeconds(m_bgmTime).TotalMilliseconds); })
                    );

                // SoundManager.PlayBGM(m_bgmID, (uint)TimeSpan.FromSeconds(m_bgmTime).TotalMilliseconds);
            }

            SoundManager.PlayCue(SoundCuesheetID.battle_common, 5, 4);

            Fighter kasugaFighter = FighterManager.GetFighter(0);
            BrawlerPlayer.FreezeInput = true;

            SimpleTimer timer = new SimpleTimer(0.5f,
                delegate
                {
                    BattleCommandSetID commandSet = BattleCommandSetID.p_kasuga_job_01;

                    if(EXHeatShouldTransform())
                        kasugaFighter.Character.GetRender().BattleTransformationOff();

                    BrawlerPlayer.WantSwapJobWeapon = true;
                    kasugaFighter.Character.GetBattleStatus().ActionCommand = commandSet;
                    kasugaFighter.Character.SetCommandSet(commandSet);
                    kasugaFighter.GetStatus().SetSuperArmor(false);
                    BrawlerPlayer.FreezeInput = false;
                    BrawlerPlayer.IsEXGamer = false;
                }
                );

            //EX Cancel
            if (immediate)
                BrawlerPlayer.OnEXCancel();
    
        }


        public static void CheckSpecialBattle()
        {
            //ccamera_rpg_battle -> ccamera_free
            //DragonEngineLibrary.Unsafe.CPP.PatchMemory(BattleTurnManager.RPGCamera.Get().Pointer, new byte[] { 0xD8, 0x96, 0x19, 0x42, 0x0, 0x0, 0x0 });
            SpecialBattleManager.OnCombatInit();
        }

        public static void OnStartEncounterBattle()
        {
            IsEncounter = true;
        }

        public static void LoadBattleResources()
        {
            SoundManager.LoadCuesheet((SoundCuesheetID)5593); //bbg_x (formerly bbg_brawler)
            SoundManager.LoadCuesheet((SoundCuesheetID)5628); //bbg_f (bbg_brawler part 2)
            SoundManager.LoadCuesheet((SoundCuesheetID)5594); //bbg_y (formerly bbg_brawler_enc)
            SoundManager.LoadCuesheet((SoundCuesheetID)5595); //bbg_z (formerly bbg_brawler_btled)
            SoundManager.LoadCuesheet((SoundCuesheetID)5603); //bbg_z (bbg_brawler_btled part 2)
            SoundManager.LoadCuesheet((SoundCuesheetID)5596); //bbg_k (battle themes that replace repetitive ones)
            SoundManager.LoadCuesheet((SoundCuesheetID)2546); //act_player

            EffectEventManager.LoadScreen(3); //ogref_bact


            //DragonEngine.Log("Loaded battle resources");
            m_ResourcesLoaded = true;
        }
        public static void Update()
        {
            Kasuga = FighterManager.GetFighter(0);
            KasugaChara = DragonEngine.GetHumanPlayer();
            KasugaMotion = KasugaChara.GetMotion();

            Enemies = FighterManager.GetAllEnemies().Where(x => !x.IsDead()).ToArray();
            EnemiesNearest = Enemies.OrderBy(x => Vector3.Distance((Vector3)Kasuga.Character.Transform.Position, (Vector3)x.Character.Transform.Position)).ToArray();

            AttackSimulator.PlayerInstance.Attacker = Kasuga;
            AttackSimulator.PlayerInstance.AttackerMotion = KasugaMotion;

            DETaskManager.Update();

#if DEBUG
            if (!FighterManager.IsBrawlerMode())
                return;
#endif

            TimeSinceBattle += DragonEngine.deltaTime;

            

            if(KasugaChara.IsValid())
            {
                if(!SoundManager.IsCuesheetLoaded((SoundCuesheetID)5594))
                    LoadBattleResources();
            }

            HActIsPlaying = AuthManager.PlayingScene.IsValid() || GameVarManager.GetValueBool(GameVarID.is_hact);  //HActManager.IsPlaying();

            //Core updates outside of combat
            BrawlerGaugeRestore.Update();
            TutorialManager.Update();

            if (!ShouldShowHeatAura())
                StopAura();
            else
                StartAura();

            TalkParamID curHactID = AuthManager.PlayingScene.Get().TalkParamID;

            if (BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Start)
            {
                /*
                if (curHactID == TalkParamID.yazawa_btlst_eb_zako_no_trans ||
                    curHactID == TalkParamID.BTLst_eb_zako_test || curHactID == TalkParamID.test_yazawa_enc_st)
                    IsEncounter = true;
                */

                /*
                if (IsEncounter)
                {
                    new DETask(delegate { return !HActManager.IsPlaying(); },
                    (
                        delegate
                        {
                            Fighter starter = FighterManager.GetAllEnemies()[0];

                            HActRequestOptions opts = new HActRequestOptions();
                            opts.base_mtx.matrix = starter.Character.GetPosture().GetRootMatrix();

                            opts.id = (TalkParamID)12882;
                            opts.is_force_play = true;

                            opts.Register(HActReplaceID.hu_enemy_00, starter.Character.UID);
                            opts.Register(HActReplaceID.hu_player1, new EntityHandle<CharacterBase>(KasugaChara.UID));
                            opts.RegisterWeapon(AuthAssetReplaceID.we_player_r, Kasuga.GetWeapon(AttachmentCombinationID.right_weapon));
                            BattleTurnManager.RequestHActEvent(opts);
                        }
                    ));
                }
                */
            }

            //Updates that concern combat start from here
            if (!Kasuga.IsValid())
            {
                IsEncounter = false;

                if (BattleStartDoOnce)
                {
                    KasugaChara.SetCommandSet(BattleCommandSetID.p_kasuga_job_01);

                    AllowPlayerTransformationDoOnce = false;
                    DisableTargetingOnce = false;
                    m_ResourcesLoaded = false;
                }

                BattleStartDoOnce = false;
                BattleTime = 0;
                BrawlerPlayer.IsEXGamer = false;

                AttackSimulator.PlayerInstance.Stop();

                return;
            }

            BattleTurnManager.UIRoot.SetVisible(false);

            if (!Kasuga.IsDead())
            {
                BrawlerPlayer.GameUpdate();
                AttackSimulator.PlayerInstance.GameUpdate();

                RPGJobID playerJob = Player.GetCurrentJob(Player.ID.kasuga);
                AuraDefinition aura;

                if (!AuraDefs.ContainsKey(playerJob))
                    aura = AuraDefs[RPGJobID.invalid];
                else
                    aura = AuraDefs[playerJob];

                if (LastAura != aura)
                    OnAuraChanged();

                LastAura = aura;

                if (!m_ResourcesLoaded)
                    if (BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Action && BattleTurnManager.CurrentActionStep != BattleTurnManager.ActionStep.Init)
                    {
                        CheckSpecialBattle();
                        m_ResourcesLoaded = true;
                    }

                if (!BattleStartDoOnce)
                {
                    Kasuga.Character.SetCommandSet(RPG.GetJobCommandSetID(Player.ID.kasuga, RPGJobID.kasuga_freeter));

                    for (uint i = 1; i < 4; i++)
                        PartyMembersCache[i - 1] = NakamaManager.GetCharacterHandle(i).Get().Attributes.player_id;

                    NakamaManager.RemoveAllPartyMembers();
                    BattleTurnManager.OverrideAttackerSelection(EnemyManager.OnAttackerSelectNormalBattle);
                    OnBattleStart?.Invoke();
                    OnBattleStartEvent();

                    HActManager.RequestPreload(TalkParamID.yazawa_btled_dummy).Get().Restart();

                    BattleStartDoOnce = true;
                }


                if (!Mod.DebugNoUpdate)
                    EnemyManager.Update();


                if (!Mod.IsGamePaused)
                {
                    HeatActionManager.Update();
                    WeaponManager.Update(Kasuga.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get());
                    HeatModule.Update();
                    EXModule.Update();
                    EXFollowups.Update();

                    //TODO CRITICAL: Use this more cleverly  as improper use can cause crash
                    if (!Debug.DontEraseRPGUI)
                        if (BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Action)
                            BattleTurnManager.ReleaseMenu();
                }
            }

            BattleTurnManager.RPGCamera.Get().Sleep();

            BattleTime += DragonEngine.deltaTime;
            TimeSinceBattle = 0;
        }



        public static void OnBattleStartEvent()
        {
            DragonEngine.Log("Battle Config ID:" + BattleProperty.BattleConfigID);
        }

        public static void OnBattleEndCallback()
        {
            OnBattleEnd?.Invoke();
        }

        public static void DecideTurnNormalBattle()
        {

        }
        
        public static void DecideTurnMachineryBattle()
        {

        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using DragonEngineLibrary;
using YazawaCommand;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using DragonEngineLibrary.Service;

namespace Brawler
{
    public static class HeatActionManager
    {
        /// <summary>Startup animation before it leads to HAct</summary>
        public static bool AnimProcedure = false;
        public static bool AwaitingHAct = false;
        /// <summary>Calculated based on level on battle start</summary>
        public static float DamageScale = 1;

        private static float m_heatCd = 0;

        private static Dictionary<AssetArmsCategoryID, YHC> m_yhcList = new Dictionary<AssetArmsCategoryID, YHC>();
        private static Dictionary<StageID, YHC> m_stageYhcList = new Dictionary<StageID, YHC>();
        private static HeatActionInformation m_currentPerformableHact = null;

        //Temp means ideally we might want to leave hacts to do actual heat reduction later down the line.
        public const int TEMP_HACT_COST = 150;
        private const float HACT_COOLDOWN = 3.5f;

        /// <summary>
        /// Prevent switching to "Event" phase when doing heat actions, else we wont get battle rewards if the enemy dies during heat action.
        /// </summary>
        public static bool BlockEventDoOnce;

        private static bool m_hactPlayingDoOnce = false;
        private static bool m_dontAllowLinkoutMovement = false;

        public static event Action OnHActStart;
        public static event Action OnHActEnd;

        private static long GetHActCost()
        {
            long heatMax = Player.GetHeatMax(Player.ID.kasuga);
            long heatNow = Player.GetHeatNow(Player.ID.kasuga);

            if (heatMax < TEMP_HACT_COST)
            {
                if(!BrawlerPlayer.IsEXGamer)
                    return (long)(heatMax * 0.85f); //85%
                else
                    return (long)(heatMax * 0.35f); //35%
            }
            else
            {
                if (!BrawlerPlayer.IsEXGamer)
                    return TEMP_HACT_COST;
                else
                    return (long)((TEMP_HACT_COST - (TEMP_HACT_COST * 0.2f))); //60% of base cost
            }

        }

        public static void Init()
        {
            m_stageYhcList[StageID.st_kamuro_yazawa_past] = Mod.ReadYHC("stage/st_kamuro_yazawa_past.yhc");
            m_stageYhcList[StageID.st_kamuro_yazawa_now] = Mod.ReadYHC("stage/st_kamuro_yazawa_now.yhc");
            m_stageYhcList[StageID.st_oumi] = Mod.ReadYHC("stage/st_oumi.yhc");
            m_stageYhcList[StageID.st_yokohama] = Mod.ReadYHC("stage/st_yokohama.yhc");

            m_yhcList[AssetArmsCategoryID.invalid] = Mod.ReadYHC("kasuga/kasuga_sud.yhc");
            m_yhcList[AssetArmsCategoryID.A] = Mod.ReadYHC("kasuga/kasuga_wpa.yhc");
            m_yhcList[AssetArmsCategoryID.B] = Mod.ReadYHC("kasuga/kasuga_wpb.yhc");
            m_yhcList[AssetArmsCategoryID.C] = Mod.ReadYHC("kasuga/kasuga_wpc.yhc");
            m_yhcList[AssetArmsCategoryID.D] = Mod.ReadYHC("kasuga/kasuga_wpd.yhc");
            m_yhcList[AssetArmsCategoryID.E] = Mod.ReadYHC("kasuga/kasuga_wpe.yhc");
            m_yhcList[AssetArmsCategoryID.F] = Mod.ReadYHC("kasuga/kasuga_wpf.yhc");
            m_yhcList[AssetArmsCategoryID.G] = Mod.ReadYHC("kasuga/kasuga_wpg.yhc");
            m_yhcList[AssetArmsCategoryID.H] = Mod.ReadYHC("kasuga/kasuga_wph.yhc");
            m_yhcList[AssetArmsCategoryID.N] = Mod.ReadYHC("kasuga/kasuga_wpn.yhc");
            m_yhcList[AssetArmsCategoryID.X] = Mod.ReadYHC("kasuga/kasuga_wpx.yhc");

            BrawlerBattleManager.OnBattleStart += CalcHActDamageMultiplier;
            OnHActEnd += 
                delegate 
                {
                    BrawlerBattleManager.CurrentHActIsY7B = false; 
                    m_dontAllowLinkoutMovement = false; 
                };

            /*
            HeatAction[] unarmedAttacks = new HeatAction[]
            {
                new HeatAction((TalkParamID)12885, HeatActionCondition.FighterDown, 5, 0, 1, 5f), //y0 brawler getup attack
                new HeatAction((TalkParamID)12892, HeatActionCondition.EnemyStandingUp, 5, 0, 1, 3.5f), //y5 enemy gets up heat attack
                new HeatAction((TalkParamID)12905, HeatActionCondition.EnemyGrabbed | HeatActionCondition.FighterHealthNotCritical, 5, 0, 1, 3.5f), //y3 shimano throw
                new HeatAction((TalkParamID)12906, HeatActionCondition.EnemyGrabbed | HeatActionCondition.FighterCriticalHealth, 5, 0, 1, 3.5f), //Custom HAct: Essence of Last Stand
                new HeatAction((TalkParamID)12895, HeatActionCondition.EnemyDown | HeatActionCondition.FighterCriticalHealth, 2, 0, 1, 2f), //kaito tower bridge
                new HeatAction((TalkParamID)12914, HeatActionCondition.HeatFull | HeatActionCondition.EnemyNotDown , 2, 0, 1, 5f), //Kurohyou kick balls
                new HeatAction((TalkParamID)12904, HeatActionCondition.EnemyDown | HeatActionCondition.EnemyHealthNotCritical, 2, 0, 1, 2f) //y5 downed combo
            };

            HeatAction[] wepAAttacks = new HeatAction[]
            {
                new HeatAction(TalkParamID.jh23760_buki_n, HeatActionCondition.EnemyNotDown, 1, 0, 1, 2f) //slam weapon on face
            };

            HeatAction[] wepABatonAttacks = new HeatAction[]
            {
                new HeatAction((TalkParamID)12891, HeatActionCondition.EnemyNotDown, 1, 0, 1, 2f) //bonk head with baton
            };

            HeatAction[] wepCAttacks = new HeatAction[]
{
                new HeatAction((TalkParamID)12893, HeatActionCondition.EnemyNotDown, 1, 0, 1, 2f) //slam weapon on face
};

            HeatAction[] wepDAttacks = new HeatAction[]
            {
                new HeatAction(TalkParamID.YH1440_ich_bat_atk, HeatActionCondition.EnemyNotDown | HeatActionCondition.FighterCriticalHealth, 3, 0, 1, 3.5f), //repurposed hero kiwami bat
                new HeatAction((TalkParamID)2530, HeatActionCondition.EnemyNotDown | HeatActionCondition.FighterHealthNotCritical, 1, 0, 1, 3.5f), //bat heat action
            };

            HeatAction[] wepGAttacks = new HeatAction[]
            {
                new HeatAction(TalkParamID.jh27320_buki_g_oi, HeatActionCondition.EnemyNotDown, 1, 0, 1, 2f) //knocked down hammer attack
            };

            HeatAction[] wepYAttacks = new HeatAction[]
            {
                new HeatAction((TalkParamID)12887, HeatActionCondition.EnemyNotDown, 1, 0, 1, 2f) //unload clip on enemy
            };

            //0 = default
            HeatActionsList.Add(AssetArmsCategoryID.invalid, new Dictionary<uint, HeatAction[]>() { [0] = unarmedAttacks });
            HeatActionsList.Add(AssetArmsCategoryID.A, new Dictionary<uint, HeatAction[]>() { [0] = wepAAttacks, [3] = wepABatonAttacks });
            HeatActionsList.Add(AssetArmsCategoryID.C, new Dictionary<uint, HeatAction[]>() { [0] = wepCAttacks });
            HeatActionsList.Add(AssetArmsCategoryID.D, new Dictionary<uint, HeatAction[]>() { [0] = wepDAttacks });
            HeatActionsList.Add(AssetArmsCategoryID.G, new Dictionary<uint, HeatAction[]>() { [0] = wepGAttacks });
            HeatActionsList.Add(AssetArmsCategoryID.Y, new Dictionary<uint, HeatAction[]>() { [0] = wepYAttacks });
            */
        }

        public static void CalcHActDamageMultiplier()
        {
            uint playerLevel = Player.GetLevel(Player.ID.kasuga);
            float mult = 1;
            int numIncrease = 0;

            for (int i = 8; i < 61 && i < playerLevel; i += 4)
                numIncrease++;

            mult = (float)Math.Pow(1.25f, numIncrease);
            DamageScale = mult;

            DragonEngine.Log("Multiplier: " + mult + "\n20 damage with multiplier: " + 20 * mult);
        }

        //Return true = heat action executed
        public static bool InputUpdate(AssetArmsCategoryID currentWep, uint subCategory)
        {
            if (m_currentPerformableHact == null)
                return false;

            bool rightClickPressed = ModInput.JustPressed(AttackInputID.HeavyAttack);

            if (!rightClickPressed || !CanHAct())
                return false;

            m_heatCd = HACT_COOLDOWN;

            ExecHeatAction(m_currentPerformableHact);
            m_currentPerformableHact = null;
            SoundManager.PlayCue(SoundCuesheetID.battle_common, 10, 0);

            int newHeat = Player.GetHeatNow(Player.ID.kasuga) - (int)GetHActCost();

            if (newHeat < 0)
                newHeat = 0;

            Player.SetHeatNow(Player.ID.kasuga, newHeat);

            return true;
        }

        public static void Update()
        {
            if (!BrawlerBattleManager.HActIsPlaying && !AnimProcedure)
            {
                if (!AwaitingHAct)
                    BrawlerBattleManager.CurrentHActIsY7B = false;

                if (m_heatCd > 0)
                    m_heatCd -= DragonEngine.deltaTime;
            }
            else
            {
                if (m_dontAllowLinkoutMovement)
                    BrawlerBattleManager.KasugaChara.Status.SetNoInputTemporary();
            }

            //DragonEngine.Log(BrawlerBattleManager.HActIsPlaying + " " + BrawlerBattleManager.CurrentHActIsY7B);



            if(!m_hactPlayingDoOnce)
            {
                if(BrawlerBattleManager.HActIsPlaying)
                {
                    m_hactPlayingDoOnce = true;
                    AwaitingHAct = false;
                    OnHActStart?.Invoke();
                    DragonEngine.Log("HAct Start");
                }
            }
            else
            {
                if(!BrawlerBattleManager.HActIsPlaying)
                {
                    m_hactPlayingDoOnce = false;
                    OnHActEnd?.Invoke();
                    DragonEngine.Log("HAct End");
                }
            }


            AssetArmsCategoryID kasugaWepCategory = Asset.GetArmsCategory(BrawlerBattleManager.Kasuga.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get().AssetID);
            StageID stageID = SceneService.GetSceneInfo().ScenePlay.Get().StageID;


            HeatActionInformation performableHact = null;

            if (m_stageYhcList.ContainsKey(stageID))
            {
                HeatActionInformation stageHact = HeatActionSimulator.Check(BrawlerBattleManager.Kasuga, m_stageYhcList[stageID]);
                performableHact = stageHact;
            }

            if (performableHact == null)
            {
                if (m_yhcList.ContainsKey(kasugaWepCategory))
                    performableHact = HeatActionSimulator.Check(BrawlerBattleManager.Kasuga, m_yhcList[kasugaWepCategory]);
                else
                    performableHact = null;
            }

            m_currentPerformableHact = performableHact;
        }

        public static bool CanHAct()
        {

            if (m_heatCd > 0)
                return false;

            if (!BrawlerBattleManager.Kasuga.IsValid())
                return false;

            if (BrawlerBattleManager.Kasuga.IsDead())
                return false;

            if (BrawlerBattleManager.HActIsPlaying)
                return false;

            if (m_currentPerformableHact == null)
                return false;

            if (BattleTurnManager.CurrentPhase != BattleTurnManager.TurnPhase.Action)
                return false;

            int heatNow = Player.GetHeatNow(Player.ID.kasuga);

            if (heatNow < GetHActCost())
                return false;


            if (InputInterface.IsKeyboardActive())
            {
                if (DragonEngine.IsKeyHeld(VirtualKey.R))
                    return false;
            }
            else
            {
                if (ModInput.Holding(BattleButtonID.npc))
                    return false;
            }

            return true;
        }

        private static HActReplaceID GetReplaceIDForActor(HeatActionActorType type)
        {
            switch (type)
            {
                case HeatActionActorType.Player:
                    return HActReplaceID.hu_player;
                case HeatActionActorType.Fighter:
                    return HActReplaceID.hu_player;
                case HeatActionActorType.Enemy1:
                    return HActReplaceID.hu_enemy_00;
                case HeatActionActorType.Enemy2:
                    return HActReplaceID.hu_enemy_01;
                case HeatActionActorType.Enemy3:
                    return HActReplaceID.hu_enemy_02;
                case HeatActionActorType.Enemy4:
                    return HActReplaceID.hu_enemy_03;
                case HeatActionActorType.Enemy5:
                    return HActReplaceID.hu_enemy_04;
                case HeatActionActorType.Enemy6:
                    return HActReplaceID.hu_enemy_05;
            }

            return HActReplaceID.invalid;
        }

        private static AuthAssetReplaceID GetAssetReplaceIDForCharacter(HActReplaceID ID, bool right)
        {
            switch (ID)
            {
                case HActReplaceID.hu_player:
                    if (right)
                        return AuthAssetReplaceID.we_player_r;
                    else
                        return AuthAssetReplaceID.we_player_l;
                case HActReplaceID.hu_player1:
                    if (right)
                        return AuthAssetReplaceID.we_player1_r;
                    else
                        return AuthAssetReplaceID.we_player1_l;
                case HActReplaceID.hu_enemy_00:
                    if (right)
                        return AuthAssetReplaceID.we_enemy_00_r;
                    else
                        return AuthAssetReplaceID.we_enemy_00_l;

                case HActReplaceID.hu_enemy_01:
                    if (right)
                        return AuthAssetReplaceID.we_enemy_01_r;
                    else
                        return AuthAssetReplaceID.we_enemy_01_l;

                case HActReplaceID.hu_enemy_02:
                    if (right)
                        return AuthAssetReplaceID.we_enemy_02_r;
                    else
                        return AuthAssetReplaceID.we_enemy_02_l;

                case HActReplaceID.hu_enemy_03:
                    if (right)
                        return AuthAssetReplaceID.we_enemy_03_r;
                    else
                        return AuthAssetReplaceID.we_enemy_03_l;
                case HActReplaceID.hu_enemy_04:
                    if (right)
                        return AuthAssetReplaceID.we_enemy_04_r;
                    else
                        return AuthAssetReplaceID.we_enemy_04_l;
            }

            return AuthAssetReplaceID.invalid;
        }



        private static void ForceExecHeatAction(HeatActionAttack hact)
        {

        }

        public static void ExecHeatAction(HeatActionInformation info)
        {
            DragonEngine.Log("Execute hact: " + info.Hact.Name);

            Vector3 hactPos = new Vector3(info.Hact.Position[0], info.Hact.Position[1], info.Hact.Position[2]);
            bool usePerformerPosition = !info.Hact.PreferHActPosition;

            HActRequestOptions opts = new HActRequestOptions();

            switch (info.Hact.SpecialType)
            {
                case HeatActionSpecialType.Normal:
                    if (info.Hact.Range == HeatActionRangeType.None)
                    {

                        if (usePerformerPosition)
                            opts.base_mtx.matrix = info.Performer.Character.GetPosture().GetRootMatrix();
                        else
                        {
                            opts.base_mtx.matrix.Position = hactPos;
                            info.Performer.Character.SetAngleY(info.Hact.RotationY);
                        }
                    }
                    else
                    {
                        opts.base_mtx.matrix = info.RangeInfo.GetMatrix();
                    }
                    break;
                case HeatActionSpecialType.Asset:
                    AssetUnit asset = AssetManager.FindNearestAssetFromAll(BrawlerBattleManager.KasugaChara.GetPosCenter(), 0).Get();
                    Vector3 assetPos = asset.GetPosCenter();

                    opts.base_mtx.matrix.Position = assetPos;
                    opts.base_mtx.matrix.ForwardDirection = asset.Transform.forwardDirection;
                    opts.base_mtx.matrix.LeftDirection = -asset.Transform.rightDirection;
                    opts.base_mtx.matrix.UpDirection = asset.Transform.upDirection;
                    break;

            }



            if (info.Hact.UseMatrix)
            {
                opts.base_mtx.matrix.ForwardDirection = new Vector4(info.Hact.Mtx.ForwardDirection.x,
                                                                    info.Hact.Mtx.ForwardDirection.y,
                                                                    info.Hact.Mtx.ForwardDirection.z,
                                                                    info.Hact.Mtx.ForwardDirection.w);

                opts.base_mtx.matrix.UpDirection = new Vector4(info.Hact.Mtx.UpDirection.x,
                                                               info.Hact.Mtx.UpDirection.y,
                                                               info.Hact.Mtx.UpDirection.z,
                                                               info.Hact.Mtx.UpDirection.w);

                opts.base_mtx.matrix.LeftDirection = new Vector4(info.Hact.Mtx.LeftDirection.x,
                                                     info.Hact.Mtx.LeftDirection.y,
                                                     info.Hact.Mtx.LeftDirection.z,
                                                     info.Hact.Mtx.LeftDirection.w);

                //overrides hact position
                if(!usePerformerPosition)
                    opts.base_mtx.matrix.Position = new Vector4(info.Hact.Mtx.Coordinates.x,
                                                     info.Hact.Mtx.Coordinates.y,
                                                     info.Hact.Mtx.Coordinates.z,
                                                     info.Hact.Mtx.Coordinates.w);
            }

            opts.id = (TalkParamID)info.Hact.HactID;
            opts.is_force_play = true;

            foreach (var kv in info.Map)
            {
                //TODO IMPORTANT: MAKE THIS LINEAR TIME
                if (info.Hact.Actors.FirstOrDefault(x => x.Type == kv.Key) == null)
                    continue;

                HActReplaceID replaceID = GetReplaceIDForActor(kv.Key);

                opts.Register(replaceID, kv.Value.Character);
                opts.RegisterWeapon(GetAssetReplaceIDForCharacter(replaceID, true), kv.Value.GetWeapon(AttachmentCombinationID.right_weapon));
                opts.RegisterWeapon(GetAssetReplaceIDForCharacter(replaceID, false), kv.Value.GetWeapon(AttachmentCombinationID.left_weapon));
            }

            AttackSimulator.PlayerInstance.Stop();

            AuthHooks.AlterNextHAct = true;
            AwaitingHAct = true;

            BrawlerBattleManager.CurrentHActIsY7B = true;

            new DETask(delegate { return m_hactPlayingDoOnce == true; }, 
                delegate 
                {
                    BrawlerBattleManager.CurrentHActIsY7B = true;
                    m_dontAllowLinkoutMovement = info.Hact.DontAllowMovementOnLinkOut;
                });


            if (info.Hact.StartupAttack != null)
            {
                AttackSimulator.PlayerInstance.ExecuteAttack(null, info.Hact.StartupAttack);
                //motion.RequestGMT((MotionID)info.Hact.MotionID);
                AnimProcedure = true;

                EffectEventManager.PlayScreen(3, false, false);

                new DETaskList
                (
                    new DETask(delegate { return !AttackSimulator.PlayerInstance.Attacking() && !BrawlerBattleManager.Kasuga.IsSync(); }, delegate { BattleTurnManager.RequestHActEvent(opts); AnimProcedure = false; }, false)

                );
            }
            else
                BattleTurnManager.RequestHActEvent(opts);

            BlockEventDoOnce = true;

            //Player.SetHeatNow(Player.ID.kasuga, 0);
            // attacker.Character.GetBattleStatus().Heat = 0;
            //BrawlerPlayer.ExecuteMove(act, attacker, enemies);
            // m_heatCd = 3.5f;
        }
    }
}

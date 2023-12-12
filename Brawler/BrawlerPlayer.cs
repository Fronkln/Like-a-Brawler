using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using DragonEngineLibrary;
using YazawaCommand;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace Brawler
{
    public static class BrawlerPlayer
    {
        public static BrawlerFighterInfo Info;
        public static MotionPlayInfo MotionInfo;

        public static Style[] Styles;
        public static Style CurrentStyle;

        public static YFC CurrentMoveset = null;
        private static YFC CommonAbilities = null;
        public static Dictionary<RPGJobID, YFC> EXMovesets = new Dictionary<RPGJobID, YFC>();

        public static float GuardTime = 0;
        public static int GuardedAttacks = 0;

        private static ECAssetArms m_lastArms = new ECAssetArms();

        public static bool ThrowingWeapon = false;

        public static bool IsEXGamer = false;
        public static bool WantSwapJobWeapon = false;
        public static bool WantTransform = false;

        public static bool FreezeInput = false;

        public static event Action OnPlayerStartGettingUp;
        public static event Action OnPlayerGuard;

        public const float WEAPON_PICKUP_DISTANCE = 1.5f;

        private static bool m_getupHyperArmorDoOnce = false;
        private static float m_styleChangeCd = 0;

        public static void Init()
        {
            Styles = GetStyles();
            CurrentStyle = Styles[0];

            CommonAbilities = Mod.ReadYFC("kasuga_base.yfc");

            //EXMovesets[RPGJobID.kasuga_freeter] = Mod.ReadYFC("kasuga_unarmed.yfc");
            EXMovesets[RPGJobID.kasuga_braver] = Mod.ReadYFC("extreme_heat/kasuga_hero.yfc");
            EXMovesets[RPGJobID.man_01] = Mod.ReadYFC("extreme_heat/kasuga_bodyguard.yfc");
            EXMovesets[RPGJobID.man_kaitaiya] = Mod.ReadYFC("extreme_heat/kasuga_foreman.yfc");
            EXMovesets[RPGJobID.man_05] = Mod.ReadYFC("extreme_heat/kasuga_breaker.yfc");
            EXMovesets[RPGJobID.man_06] = Mod.ReadYFC("extreme_heat/kasuga_enforcer.yfc");
            EXMovesets[RPGJobID.man_07] = Mod.ReadYFC("extreme_heat/kasuga_chef.yfc");
            EXMovesets[RPGJobID.dlc_01] = Mod.ReadYFC("extreme_heat/kasuga_devil.yfc");
        }


        //Should execute brawler input
        //Battle start done once
        //Input not frozen
        //Not downed
        //Not dead
        //Not ragdolled
        //Not sync
        //Not attacking
        //Not getting up
        public static bool GenericShouldExecuteAttack()
        {
            if (!Mod.ShouldExecBrawlerInput() || !BrawlerBattleManager.BattleStartDoOnce || FreezeInput)
                return false;

            if (Info.IsDead)
                return false;

            if (Info.IsFlinching & (AttackSimulator.PlayerInstance.m_currentGroup != null && AttackSimulator.PlayerInstance.m_currentGroup.Attacks.FirstOrDefault(x => x.HasConditionOfType(AttackConditionType.IsFlinching)) == null))
                return false;

            return !Info.IsSync && !Info.IsDown && !Info.IsGettingUp && !Info.IsRagdoll;
        }

        public static bool ThrowWeapon()
        {
            Weapon wep = BrawlerBattleManager.Kasuga.GetWeapon(AttachmentCombinationID.right_weapon);
            AssetUnit unit = wep.Unit;

            DragonEngine.Log("throw request");

            if (unit.AssetID == AssetID.invalid)
                return false;

            AssetArmsCategoryID wepCategory = Asset.GetArmsCategory(unit.AssetID);

            switch (wepCategory)
            {
                default:
                    BrawlerBattleManager.KasugaChara.GetMotion().RequestGMT(17075);
                    break;
            }

            //   ThrowingWeapon = true;
            return true;
        }

        public static Fighter GetLockOnTarget(Fighter kasugaFighter)
        {
            try
            {
                if (BrawlerBattleManager.DisableTargetingOnce)
                    return new Fighter(IntPtr.Zero);

                if (BrawlerBattleManager.EnemiesNearest.Length <= 0)
                    return new Fighter(IntPtr.Zero);

                if (Debug.ForceTargetingNearest)
                    return BrawlerBattleManager.EnemiesNearest[0];

                bool isLockingIn = BrawlerHooks.HumanModeManager_IsInputKamae(kasugaFighter.Character.HumanModeManager.Pointer);

                //always prioritize locking in to the nearest target.
                if (isLockingIn)
                    return BrawlerBattleManager.EnemiesNearest[0];


                //always prioritize last hit enemy during a combo to make landing combos easier.
                if (AttackSimulator.PlayerInstance.Attacking() && AttackSimulator.PlayerInstance.LastEnemyHitCurrentCombo.IsValid())
                {
                    EnemyAI ai = EnemyManager.GetAI(AttackSimulator.PlayerInstance.LastEnemyHitCurrentCombo);

                    if (ai != null && !ai.Info.IsDead)
                        return AttackSimulator.PlayerInstance.LastEnemyHitCurrentCombo;
                }

                Fighter[] nearestEnemies = BrawlerBattleManager.EnemiesNearest;


                float dot = -0.6f;

                foreach (Fighter enemy in nearestEnemies)
                {
                    if (!kasugaFighter.Character.IsFacingEntity(enemy.Character, dot))
                        continue;

                    float dist = Vector3.Distance(kasugaFighter.Character.GetPosCenter(), enemy.Character.GetPosCenter());

                    if (dist >= 4.5)
                        continue;

                    return enemy;
                }

                return new Fighter(IntPtr.Zero);
            }
            catch
            {
                return new Fighter(IntPtr.Zero);
            }
        }

        public static void UpdateTargeting(Fighter kasugaFighter, Fighter[] allEnemies)
        {
            ECBattleTargetDecide targetDecide = kasugaFighter.Character.TargetDecide;
            targetDecide.SetTarget(GetLockOnTarget(kasugaFighter).GetID());
        }

        public static bool AllowDamage(BattleDamageInfo inf)
        {
            return Mod.GodMode || TutorialManager.AllowPlayerDamage();
        }

        public static bool CanPickupWeapon()
        {
            if (BattleTurnManager.CurrentPhase != BattleTurnManager.TurnPhase.Action)
                return false;

            if(BrawlerBattleManager.Kasuga.IsValid() && !BrawlerBattleManager.Kasuga.GetWeapon(AttachmentCombinationID.right_weapon).Unit.IsValid())
            {
                bool pressed = ModInput.JustPressed(AttackInputID.Grab);

                if (pressed)
                {
                    AssetUnit wep = AssetManager.FindNearestAssetFromAll(BrawlerBattleManager.KasugaChara.GetPosCenter(), 2);

                    if (Vector3.Distance(wep.GetPosCenter(), BrawlerBattleManager.KasugaChara.GetPosCenter()) > WEAPON_PICKUP_DISTANCE)
                        return false;
                    else
                        return true;
                }
            }
            return false;
        }

        public static void PickupAsset(AssetUnit unit)
        {
            BrawlerBattleManager.KasugaChara.HumanModeManager.ToPickup(unit);
            unit.DestroyEntity();
            WeaponManager.WeaponTime = 0;

            DragonEngine.Log("pickup");
        }

        public static void OnGuard()
        {
            OnPlayerGuard?.Invoke();
        }

        /// <summary>
        /// Used in damage execution functions, don't use individually on normal game loop
        /// </summary>
        public unsafe static bool CanCounterAttack(BattleDamageInfoSafe info)
        {
            Character attacker = info.Attacker.Get();

            if (Vector3.Distance((Vector3)BrawlerBattleManager.KasugaChara.Transform.Position, (Vector3)attacker.Transform.Position) > 1.8f)
                return false;

            //Are we locked into the enemy that tried to attack us.
            if (GetLockOnTarget(BrawlerBattleManager.Kasuga).Character.UID != attacker.UID)
                return false;

            if (Info.IsDead || Info.IsSync || Info.IsDown || Info.IsGettingUp)
                return false;

            /*
            if (Mod.Input[AttackInputID.LeftShift].TimeHeld > 0 && Mod.Input[AttackInputID.LeftShift].TimeHeld <= 0.7f)
                return true;
            */

            return true;
        }

        /// <summary>
        /// Would this attack kill the player (takes revive items/RNG into account)
        /// </summary>
        //TODO: Check inv for sacrifice stone
        public unsafe static bool WouldDie(BattleDamageInfoSafe dmg)
        {
            ECBattleStatus status = BrawlerBattleManager.Kasuga.GetStatus();

            if (status.CurrentHP - dmg.Damage <= 0)
                return true;
            else
                return false;
        }

        public static bool IsInputKamae()
        {
            if (DragonEngine.IsKeyHeld(VirtualKey.MiddleButton))
                return true;

            return false;
        }

        public static void InputUpdate()
        {
            if (!Mod.AllowInputUpdate())
                return;

            Fighter kasugaFighter = FighterManager.GetFighter(0);

            UpdateTargeting(kasugaFighter, BrawlerBattleManager.Enemies);

            CurrentMoveset = DecideMoveset();

            AssetUnit unit = kasugaFighter.GetWeapon(AttachmentCombinationID.right_weapon).Unit;
            bool heatActionUpdate = false;

          //  if (AttackSimulator.PlayerInstance.CurrentAttack == null || AttackSimulator.PlayerInstance.CurrentAttack.AllowHActWhileExecuting())
            heatActionUpdate = HeatActionManager.InputUpdate(Asset.GetArmsCategory(unit.AssetID), Asset.GetArmsCategorySub(unit.AssetID));

            //we executed a heat action.
            if (heatActionUpdate || HeatActionManager.AwaitingHAct ||  BrawlerBattleManager.CurrentHActIsY7B)
                return;

            AttackSimulator.PlayerInstance.InputUpdateAttack(CurrentMoveset);

            bool attacking = AttackSimulator.PlayerInstance.Attacking();
            bool wepUpdate = WeaponManager.InputUpdate(unit) && !attacking;

            if (DragonEngine.IsKeyDown(VirtualKey.N1))
                ChangeStyle(Styles[0]);

            if (DragonEngine.IsKeyDown(VirtualKey.N2))
                ChangeStyle(Styles[1]);

            RPGJobID job = Player.GetCurrentJob(Player.ID.kasuga);
            bool isExCancel = AttackSimulator.PlayerInstance.IsInFollowupWindow() && AttackSimulator.PlayerInstance.Attacking();
            //DragonEngine.Log(BrawlerBattleManager.KasugaMotion.InTimingRange(71, 0) + " " + AttackSimulator.PlayerInstance.Attacking());

            if (BrawlerBattleManager.Kasuga.GetStatus().Level >= 8)
            {
                if ((m_styleChangeCd <= 0 && (ModInput.JustPressed(AttackInputID.ExtremeHeat) && !WantTransform)) && (!attacking || isExCancel))
                {
                    if (!IsEXGamer)
                    {
                        if (Player.GetHeatNow(Player.ID.kasuga) > 0)
                        {
                            IsEXGamer = true;
                            WantTransform = true;
                        }
                    }
                    else
                    {
                        IsEXGamer = false;
                        WantTransform = true;
                    }
                }

            }

            if (WantTransform)
            {
                bool sameJob = kasugaFighter.GetStatus().ActionCommand == RPG.GetJobCommandSetID(Player.ID.kasuga, job);

                if (!sameJob)
                {
                    kasugaFighter.Character.Components.EffectEvent.Get().StopEvent((EffectEventCharaID)0x104, false);
                    kasugaFighter.Character.Components.EffectEvent.Get().PlayEventOverride((EffectEventCharaID)0x104);
                }

                if (!isExCancel)
                    FreezeInput = true;
                else
                    if (!Info.IsSync)
                    AttackSimulator.PlayerInstance.Stop(); //attack cancel

                DragonEngine.Log(isExCancel);

                if (IsEXGamer)
                    BrawlerBattleManager.OnEXGamerON(isExCancel);
                else
                    BrawlerBattleManager.OnEXGamerOFF(isExCancel);

                m_styleChangeCd = 0.45f;

                WantTransform = false;
            }


            if (!attacking && !wepUpdate && !heatActionUpdate)
            {
                AttackSimulator.PlayerInstance.InputUpdatePreAttack(CommonAbilities);
                AttackSimulator.PlayerInstance.InputUpdatePreAttack(CurrentMoveset);
            }
        }

        public static void GameUpdate()
        {
            Fighter kasugaFighter = FighterManager.GetFighter(0);
            Info.Update(kasugaFighter);

            if (!m_lastArms.IsValid() && Info.RightWeapon.IsValid() || (Info.RightWeapon.IsValid() && m_lastArms.UID != Info.RightWeapon.UID))
            {
                OnEquipWeapon();
            }

            m_lastArms = Info.RightWeapon;

            MotionInfo = kasugaFighter.Character.GetMotion().PlayInfo;
            UpdateCheats();


            if (IsEXGamer)
                EXUpdate();

            InputUpdate();

            //Second condition is for player movement freezing on talk
            if (FreezeInput || (BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Event && BattleTurnManager.CurrentActionStep == BattleTurnManager.ActionStep.Init))
                DragonEngine.GetHumanPlayer().Status.SetNoInputTemporary();


             bool guardInput = DragonEngine.GetHumanPlayer().HumanModeManager.IsGuarding();
            float timeDelta = DragonEngine.deltaTime;


            if (m_styleChangeCd > 0)
                m_styleChangeCd -= timeDelta;

            if (!guardInput)
            {
                GuardedAttacks = 0;
                GuardTime = 0;
            }
            else
                GuardTime += timeDelta;

            if (WantSwapJobWeapon)
            {
                RPGJobID job = Player.GetCurrentJob(Player.ID.kasuga);

                //drop it
                if (!FighterManager.GetFighter(0).IsValid() || kasugaFighter.GetStatus().ActionCommand != RPG.GetJobCommandSetID(Player.ID.kasuga, job))
                {
                    kasugaFighter.DropWeapon(new DropWeaponOption(AttachmentCombinationID.right_weapon, true));
                    kasugaFighter.DropWeapon(new DropWeaponOption(AttachmentCombinationID.left_weapon, true));
                }
                else
                {
                    //Enforcer and Chef: Dual Weapons
                    switch(job)
                    {
                        default:
                            kasugaFighter.Equip(Party.GetEquipItemID(Player.ID.kasuga, PartyEquipSlotID.weapon), AttachmentCombinationID.right_weapon);
                            break;
                        case RPGJobID.man_06:
                            kasugaFighter.Equip(Party.GetEquipItemID(Player.ID.kasuga, PartyEquipSlotID.weapon), AttachmentCombinationID.left_weapon);
                            kasugaFighter.Equip(ItemID.yazawa_pocket_weapon_adachi_004, AttachmentCombinationID.right_weapon);
                            break;
                        case RPGJobID.man_07:
                            kasugaFighter.Equip(Party.GetEquipItemID(Player.ID.kasuga, PartyEquipSlotID.weapon), AttachmentCombinationID.left_weapon);
                            kasugaFighter.Equip(Party.GetEquipItemID(Player.ID.kasuga, PartyEquipSlotID.weapon), AttachmentCombinationID.right_weapon);
                            break;
                    };
                }

                WantSwapJobWeapon = false;


                AssetUnit leftWep = kasugaFighter.GetWeapon(AttachmentCombinationID.left_weapon).Unit.Get();
                AssetUnit rightWep = kasugaFighter.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get();

                DragonEngine.Log("Job Equip: Left Hand - " + leftWep.AssetID + $"({Asset.GetArmsCategory(leftWep.AssetID)}) ({Asset.GetArmsCategorySub(leftWep.AssetID)})");
                DragonEngine.Log("Job Equip: Right Hand - " + rightWep.AssetID + $"({Asset.GetArmsCategory(rightWep.AssetID)}) ({Asset.GetArmsCategorySub(rightWep.AssetID)})");
            }


            if(!m_getupHyperArmorDoOnce)
            {
                if(Info.IsGettingUp)
                {
                    if (!IsEXGamer)
                        kasugaFighter.GetStatus().SetSuperArmor(true);

                    OnPlayerStartGettingUp?.Invoke();

                    m_getupHyperArmorDoOnce = true;
                }
            }
            else
            {
                if (!Info.IsGettingUp)
                {
                    if (!IsEXGamer)
                        kasugaFighter.GetStatus().SetSuperArmor(false);

                    m_getupHyperArmorDoOnce = false;
                }

            }
        }

        public static void EXUpdate()
        {
            if(!WantTransform && !WantSwapJobWeapon)
            {
                ItemID weapon = Party.GetEquipItemID(Player.ID.kasuga, PartyEquipSlotID.weapon);

                //Restore our weapon if we lost it for some weird reason.
                if (weapon != 0 && !Info.RightWeapon.IsValid())
                {
                    ItemID weaponToEquip = ItemID.dummy0;

                    switch(Player.GetCurrentJob(Player.ID.kasuga))
                    {
                        default:
                            weaponToEquip = weapon;
                            break;
                        case RPGJobID.man_06:
                            weaponToEquip = ItemID.yazawa_pocket_weapon_adachi_004;
                            break;
                        case RPGJobID.man_07:
                            BrawlerBattleManager.Kasuga.Equip(weapon, AttachmentCombinationID.left_weapon);
                            break;
                    }
                    BrawlerBattleManager.Kasuga.Equip(weaponToEquip, AttachmentCombinationID.right_weapon);
                }
            }
        }


        public static void OnEXCancel()
        {
            AttackSimulator.PlayerInstance.LastEnemyHitCurrentCombo = new Fighter();
            AttackSimulator.PlayerInstance.m_attacking = false;
            AttackSimulator.PlayerInstance.CurrentAttack = null;
            AttackSimulator.PlayerInstance.Stop();

            BrawlerBattleManager.Kasuga.DropWeapon(new DropWeaponOption(AttachmentCombinationID.right_weapon, true));
            BrawlerBattleManager.Kasuga.DropWeapon(new DropWeaponOption(AttachmentCombinationID.left_weapon, true));

            BrawlerPlayer.CurrentMoveset = BrawlerPlayer.DecideMoveset();
        }

        private static void UpdateCheats()
        {
            if (Mod.GodMode)
            {
                if (BrawlerBattleManager.Kasuga.IsValid())
                {
                    ECBattleStatus status = BrawlerBattleManager.Kasuga.GetStatus();
                    status.CurrentHP = status.MaxHP;
                    Player.SetHeatNow(Player.ID.kasuga, Player.GetHeatMax(Player.ID.kasuga));

                    if(Info.RightWeapon.ArmsType == ArmsType.single)
                        Info.RightWeapon.AsSingle().UseCount = 5;
                }
            }
        }

        public static YFC DecideMoveset()
        {
            if (IsEXGamer)
            {
                RPGJobID job = Player.GetCurrentJob(Player.ID.kasuga);

                if (EXMovesets.ContainsKey(job))
                    return EXMovesets[job];
                else
                    return CurrentStyle.CommandSet;
            }
            else
            {
                if (Info.RightWeapon.IsValid())
                {
                    AssetUnit wepUnit = Info.RightWeapon.Unit;
                    AssetArmsCategoryID wepCategory = Asset.GetArmsCategory(wepUnit.AssetID);

                    if (WeaponManager.WeaponMovesets.ContainsKey(wepCategory))
                        return WeaponManager.WeaponMovesets[wepCategory];
                    else
                        return CurrentStyle.CommandSet;
                }
                else
                    return CurrentStyle.CommandSet;
            }
        }

        public static ECAssetArms GetWeapon(bool right)
        {
            AssetUnit armsUnit = BrawlerBattleManager.Kasuga.GetWeapon((right ? AttachmentCombinationID.right_weapon : AttachmentCombinationID.left_weapon)).Unit.Get();
            return armsUnit.Arms;
        }

        public static bool DamageTransit(BattleDamageInfoSafe dmg)
        {
            if (HeatActionManager.AnimProcedure)
                return true;

            Fighter attacker = dmg.Attacker.Get().GetFighter();

            if (!CanCounterAttack(dmg))
            {
                if (WouldDie(dmg))
                {
                    /*
                    HActRequestOptions opts = new HActRequestOptions()
                    {
                        id = TalkParamID.yh1630_sae_sosei,
                        is_force_play = true,
                    };

                    //test revive
                    opts.base_mtx.matrix = DragonEngine.GetHumanPlayer().GetPosture().GetRootMatrix();
                    opts.Register(HActReplaceID.hu_player, BrawlerBattleManager.KasugaChara.UID);
                    BattleTurnManager.RequestHActEvent(opts);
                    BrawlerBattleManager.Kasuga.GetStatus().CurrentHP = BrawlerBattleManager.Kasuga.GetStatus().MaxHP;


                    return true;
                    */

                    bool specialFinish = EnemyManager.GetAI(attacker).DoFinisher(dmg);

                    if (specialFinish)
                        return true;
                    else
                        return false;
                }

                return false;
            }


            bool counterTransit = AttackSimulator.PlayerInstance.TransitCounter(attacker, dmg);
            return counterTransit;
        }

        //OnAttackHit/OnAttackLand
        public static void OnHitEnemy(Fighter enemy, BattleDamageInfoSafe dmg)
        {
            if (!IsEXGamer)
            {
                int curHeat = Player.GetHeatNow(Player.ID.kasuga);
                int maxHeat = Player.GetHeatMax(Player.ID.kasuga);

                //player will recover 8% heat for each hit
                //TODO: make this fair for both early and late game by adding heat gain upgrade?
                if (curHeat < maxHeat)
                    Player.SetHeatNow(Player.ID.kasuga, curHeat + (int)(maxHeat * 0.08f));

                if (Player.GetHeatNow(Player.ID.kasuga) > maxHeat)
                    Player.SetHeatNow(Player.ID.kasuga, maxHeat);
            }

            if (Info.RightWeapon.IsValid())
            {
                ECAssetArmsSingle singleWep = Info.RightWeapon.AsSingle();

                if (singleWep != null)
                    OnHitEnemyWithWeapon(singleWep, enemy, dmg);
            }

            EnemyAI ai = EnemyManager.GetAI(enemy);

            if (ai == null)
                return;

            /*
            int count = BrawlerBattleManager.Enemies.Length;
            DragonEngine.Log(count + " " + dmg.Damage  + " " + enemy.GetStatus().CurrentHP);
            

            if (count <= 2)
            if (ai.WouldDieToDamage(dmg))
            {

                 Character fighter = FighterManager.GenerateEnemyFighter(new PoseInfo(BrawlerBattleManager.KasugaChara.GetPosCenter() + -BrawlerBattleManager.KasugaChara.Transform.forwardDirection, 0), 15603, (CharacterID)0x67);



                HActRequestOptions opts = new HActRequestOptions()
                {
                    id = (TalkParamID)12882,
                    is_force_play = true,
                };

                //test revive
                opts.base_mtx.matrix = DragonEngine.GetHumanPlayer().GetPosture().GetRootMatrix();
                opts.Register(HActReplaceID.hu_player, BrawlerBattleManager.KasugaChara.UID);
                opts.Register(HActReplaceID.hu_enemy_00, fighter);
                opts.RegisterWeapon(AuthAssetReplaceID.we_enemy_00_r, fighter.GetFighter().GetWeapon(AttachmentCombinationID.right_weapon));
                BattleTurnManager.RequestHActEvent(opts);

               BrawlerBattleTransition.DontAllowEnd = true;

                    new DETaskList
                        (
                            new DETask(delegate { return BrawlerBattleManager.HActIsPlaying; }, null, false),
                            new DETask(delegate { return !BrawlerBattleManager.HActIsPlaying; }, delegate { BrawlerBattleTransition.DontAllowEnd = false; }, false)
                        );
            }
            */
        }

        private static void OnHitEnemyWithWeapon(ECAssetArmsSingle arms, Fighter enemy, BattleDamageInfoSafe dmg)
        {
            if (IsEXGamer)
                return;

            //TODO: Alter this to check for job weapon instead.
            if (!BrawlerPlayer.IsEXGamer)
            {
                if (arms.UseCount >= 10)
                    arms.UseCount = 3;
                else
                {

                    arms.UseCount--;
                    DragonEngine.Log(arms.UseCount);

                    if (arms.UseCount <= 0)
                        arms.Unit.Break();
                    // BrawlerBattleManager.Kasuga.DropWeapon(new DropWeaponOption(AttachmentCombinationID.right_weapon, true));
                }
            }
        }

        public static void OnEquipWeapon()
        {
            if (Info.RightWeapon == null)
                return;

            ECAssetArmsSingle single = Info.RightWeapon.AsSingle();

            if (single == null)
                return;

            AssetArmsCategoryID category = Asset.GetArmsCategory(single.Unit.AssetID);
            single.UseCount = BrawlerConfig.GetUseCountForAssetCategory(category);
#if DEBUG
            DragonEngine.Log($"Equipped asset {single.Unit.AssetID} with type " + category + ". Use Count: " + single.UseCount);
#endif
            
        }

        public static void OnGetHit(Fighter enemy, BattleDamageInfoSafe dmg)
        {
            bool wasGuarded = dmg.IsGuard || dmg.IsJustGuard;

            if (!wasGuarded)
            {
                ECBattleStatus kasugaStatus = BrawlerBattleManager.Kasuga.GetStatus();
                int maxHeat = Player.GetHeatMax(Player.ID.kasuga);

                float reductionAmount = ((!dmg.IsGuard && !dmg.IsJustGuard) ? 0.025f * 2 : 0.025f);

                int reducedHeat = Player.GetHeatNow(Player.ID.kasuga) - (int)(maxHeat * reductionAmount);

                if (reducedHeat < 0)
                    reducedHeat = 0;

                //Player will lose 9.5% heat for each hit they take
                Player.SetHeatNow(Player.ID.kasuga, reducedHeat);
            }
            else
            {
                OnGuard();

                if (IsEXGamer)
                    dmg.Damage = 0;
            }
        }

        //Standard attacking, does not include heat actions
        public static void ExecuteMove(MoveBase move, Fighter attacker, Fighter[] enemy, bool force = false)
        {
            move.Execute(attacker, enemy);
        }

        public static void ChangeStyle(Style style, bool force = false)
        {
            if (CurrentStyle == style && !force)
                return;

            if (style == null)
            {
                if (CurrentStyle != null)
                    style = CurrentStyle;
                else
                    style = Styles[0];
            }

            CurrentStyle = style;
            AttackSimulator.PlayerInstance.ExecuteSingleGMTAttack(style.SwapAnimation);
        }

        public static Style[] GetStyles()
        {
            /*

            CommonMoves = new Moveset
            (
                RPGSkillID.invalid,
                new MoveSidestep(0.25f, new AttackInput[]
                {
                    new AttackInput(AttackInputID.Space, false)
                }, AttackConditionType.NotDown)
            );

            */

            /*
            //Legend
            Moveset legendMoveSet = new Moveset
            (
                (RPGSkillID)1750,
                new MoveSidestep(0.25f, new AttackInput[]
                {
                    new AttackInput(AttackInputID.Space, false)
                }, AttackConditionType.NotDown),

                //Stomp
                new MoveRPG((RPGSkillID)1752, 0.8f, new AttackInput[]
                {
                    new AttackInput(AttackInputID.RightMouse, false)
                }, AttackConditionType.LockedEnemyDown),

                new MoveCFC(new MoveCFC.AttackFrame[]
                {
                    new MoveCFC.AttackFrame(new FighterCommandID(1337, 1), new FighterCommandID(1337, 5), false, 1f),
                    new MoveCFC.AttackFrame(new FighterCommandID(1337, 2), new FighterCommandID(1337, 13), true, 0.4f, 0.8f),
                    new MoveCFC.AttackFrame(new FighterCommandID(1337, 3), new FighterCommandID(1337, 7), true, 0.7f, 0.8f),
                    new MoveCFC.AttackFrame(new FighterCommandID(1337, 4), new FighterCommandID(1337, 8), false, 0.4f, 0.8f),

                }, new AttackInput[]
                {
                    new AttackInput(AttackInputID.LeftMouse, false)
                }, AttackConditionType.NotDown),

                new MoveGrab(new AttackInput[] { new AttackInput(AttackInputID.E, false) }, AttackConditionType.NotDown)
                {
                    Grab = (RPGSkillID)194,
                    GrabAnim = (MotionID)17096, //p_yag_btl_mai_sy0_gsp_lp
                    GrabSync = (RPGSkillID)1759,
                    ShakeOff = (RPGSkillID)1754,
                    HitThrow = (RPGSkillID)1755,
                    HitLight = new RPGSkillID[] { (RPGSkillID)1756, (RPGSkillID)1757, (RPGSkillID)1758 },
                    HitHeavy = (RPGSkillID)1760
                },

                /*
                new MoveRPG((RPGSkillID)246, 0.001f, new AttackInput[]
                {
                    new AttackInput(AttackInputID.E, false)
                }, AttackConditionType.NotDown, 3.5f),
                */

            /*
            new MoveRPG((RPGSkillID)243, 2.5f, new AttackInput[]
            {
                new AttackInput(AttackInputID.E, true),
            }, AttackConditionType.NotDown),


#if DEBUG
            new MoveRPG(RPGSkillID.boss_kiryu_legend_atk_c, 1f, new AttackInput[]
            {
                new AttackInput(AttackInputID.LeftShift, true),
                new AttackInput(AttackInputID.RightMouse, false)
            }, AttackConditionType.NotDown)
#endif

        );

        EXMovesets = new Dictionary<AssetArmsCategoryID, Moveset>()
        {
            [AssetArmsCategoryID.invalid] = new Moveset
        (
            (RPGSkillID)1751,
            new MoveSidestep(0.25f, new AttackInput[]
            {
                new AttackInput(AttackInputID.Space, false)
            }, AttackConditionType.NotDown),

            new MoveRPG(RPGSkillID.kasuga_job01_nml_atk_down, 1f, new AttackInput[]
            {
                new AttackInput(AttackInputID.RightMouse, false)
            }, AttackConditionType.LockedEnemyDown, 3f),

            new MoveCFC(new MoveCFC.AttackFrame[]
            {
                new MoveCFC.AttackFrame(new FighterCommandID(1337, 1), new FighterCommandID(1337, 5), false, 1f),
                new MoveCFC.AttackFrame(new FighterCommandID(1337, 2), new FighterCommandID(1337, 6), true, 0.4f, 0.8f),
                new MoveCFC.AttackFrame(new FighterCommandID(1337, 3), new FighterCommandID(1337, 7), true, 0.7f, 0.8f),
                new MoveCFC.AttackFrame(new FighterCommandID(1337, 4), new FighterCommandID(1337, 8), false, 0.4f, 0.8f),

            }, new AttackInput[]
            {
                new AttackInput(AttackInputID.LeftMouse, false)
            }, AttackConditionType.NotDown),

            new MoveRPG((RPGSkillID)245, 3.5f, new AttackInput[]
            {
                new AttackInput(AttackInputID.E, false)
            }, AttackConditionType.NotDown),

            /*
            new MoveRPG((RPGSkillID)243, 2.5f, new AttackInput[]
            {
                new AttackInput(AttackInputID.E, true),
            }, AttackConditionType.NotDown),


            new MoveRPG(RPGSkillID.boss_kiryu_legend_atk_c, 1f, new AttackInput[]
            {
                new AttackInput(AttackInputID.LeftShift, true),
                new AttackInput(AttackInputID.RightMouse, false)
            }, AttackConditionType.NotDown)

        )
        };
         */
            Style legendStyle = new Style((MotionID)17033, Mod.ReadYFC("kasuga_unarmed.yfc"));
            Style debugStyle = new Style((MotionID)17033, Mod.ReadYFC("kasuga_debug.yfc"));

            return new Style[]
            {
                legendStyle,
                debugStyle
            };
        }
    }
}

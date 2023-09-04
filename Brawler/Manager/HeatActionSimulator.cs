using DragonEngineLibrary;
using System.Collections.Generic;
using YazawaCommand;
using HeatActionCondition = YazawaCommand.HeatActionCondition;
using FighterMap = System.Collections.Generic.Dictionary<YazawaCommand.HeatActionActorType, DragonEngineLibrary.Fighter>;
using System;
using DragonEngineLibrary.Service;
using System.Linq;

namespace Brawler
{
    internal static class HeatActionSimulator
    {
        public static HeatActionInformation Check(Fighter performer, YHC hactList)
        {
            if (HeatActionManager.AnimProcedure || !performer.IsValid() || hactList == null || hactList.Attacks.Count <= 0)
                return null;

            FighterMap map = new FighterMap();

            map[HeatActionActorType.Fighter] = performer;
            map[HeatActionActorType.Player] = BrawlerBattleManager.Kasuga;

            if (performer.IsPlayer())
            {
                Fighter[] enemies = BrawlerBattleManager.EnemiesNearest;
                int curEnemyIdx = 0;

                if (enemies != null && enemies.Length > 0)
                {
                    //Register enemies for player
                    for (int i = (int)HeatActionActorType.Enemy1; ; i++)
                    {
                        if (curEnemyIdx >= enemies.Length || curEnemyIdx == 5)
                            break;

                        map[(HeatActionActorType)i] = enemies[curEnemyIdx];

                        curEnemyIdx++;
                    }
                }
            }
            else
            {
                map[HeatActionActorType.Enemy1] = BrawlerBattleManager.Kasuga;
            }

            foreach (HeatActionAttack attack in hactList.Attacks)
            {
                if (attack.IsFollowupOnly)
                    continue;

                HActRangeInfo info = new HActRangeInfo();

                if (attack.Range != HeatActionRangeType.None)
                    if (!performer.GetStatus().HAct.GetPlayInfo(ref info, (HActRangeType)attack.Range))
                        goto cont;

                HeatActionInformation inf = new HeatActionInformation();
                inf.Performer = performer;
                inf.Map = map;
                inf.Hact = attack;
                inf.RangeInfo = info;

                FighterMap adjustedMap = new FighterMap(map);

                List<HeatActionActor> invalidActors = new List<HeatActionActor>();


                foreach (HeatActionActor actor in attack.Actors)
                {
                    bool exists = map.ContainsKey(actor.Type);

                    if (!exists && !actor.Optional)
                        goto cont;

                    if (exists)
                    {
                        foreach (YazawaCommand.HeatActionCondition cond in actor.Conditions)
                            if (!CheckFlag(attack, map, map[actor.Type], cond, inf.RangeInfo))
                            {
                                if (!actor.Optional)
                                    goto cont;
                                else
                                    invalidActors.Add(actor);
                            }
                    }
                }

                foreach (var invActor in invalidActors)
                    adjustedMap.Remove(invActor.Type);

                inf.Map = adjustedMap;



                return inf;

            cont:
                continue;
            }

            return null;
        }

        public static HeatActionInformation CheckSingle(Fighter performer, HeatActionAttack attack)
        {
            //NOT GOOD: COPY PASTED CODE FROM CHECK
            if (HeatActionManager.AnimProcedure || !performer.IsValid() || attack == null)
                return null;

            FighterMap map = new FighterMap();

            map[HeatActionActorType.Fighter] = performer;
            map[HeatActionActorType.Player] = BrawlerBattleManager.Kasuga;

            if (performer.IsPlayer())
            {
                Fighter[] enemies = BrawlerBattleManager.EnemiesNearest;
                int curEnemyIdx = 0;

                if (enemies != null && enemies.Length > 0)
                {
                    //Register enemies for player
                    for (int i = (int)HeatActionActorType.Enemy1; ; i++)
                    {
                        if (curEnemyIdx >= enemies.Length || curEnemyIdx == 5)
                            break;

                        map[(HeatActionActorType)i] = enemies[curEnemyIdx];

                        curEnemyIdx++;
                    }
                }
            }
            else
            {
                map[HeatActionActorType.Enemy1] = BrawlerBattleManager.Kasuga;
            }

            HActRangeInfo info = new HActRangeInfo();

            if (attack.Range != HeatActionRangeType.None)
                if (!performer.GetStatus().HAct.GetPlayInfo(ref info, (HActRangeType)attack.Range))
                    return null;

            HeatActionInformation inf = new HeatActionInformation();
            inf.Performer = performer;
            inf.Map = map;
            inf.Hact = attack;
            inf.RangeInfo = info;

            FighterMap adjustedMap = new FighterMap(map);

            List<HeatActionActor> invalidActors = new List<HeatActionActor>();

            foreach (HeatActionActor actor in attack.Actors)
            {
                bool exists = map.ContainsKey(actor.Type);

                if (!exists && !actor.Optional)
                    return null;

                if (exists)
                {
                    foreach (YazawaCommand.HeatActionCondition cond in actor.Conditions)
                        if (!CheckFlag(attack, map, map[actor.Type], cond, inf.RangeInfo))
                        {
                            if (!actor.Optional)
                                return null;
                            else
                                invalidActors.Add(actor);
                        }
                }
            }

            foreach (var invActor in invalidActors)
                adjustedMap.Remove(invActor.Type);

            inf.Map = adjustedMap;

            return inf;
        }

        private static bool CheckFlag(HeatActionAttack attack, FighterMap fightersList, Fighter actor, YazawaCommand.HeatActionCondition cond, HActRangeInfo rangeInf)
        {
            if (!actor.IsValid())
                return false;

            bool flag = false;
            bool performerIsPlayer = actor.IsPlayer();

            BrawlerFighterInfo inf;
            CharacterAttributes actorAttributes = actor.Character.Attributes;

            EnemyAI ai = null;

            if (performerIsPlayer)
                inf = BrawlerPlayer.Info;
            else
            {
                ai = EnemyManager.GetAI(actor);

                if (ai == null)
                    return false;

                if (!ai.IsHuman)
                    return false;

                inf = EnemyManager.GetAI(actor).Info;
            }


            switch (cond.Type)
            {
                case HeatActionConditionType.Invalid:
                    flag = true;
                    break;

                case HeatActionConditionType.Down:

                    if (cond.Param2B)
                        flag = inf.IsFaceDown || inf.IsDown;
                    else
                    {
                        if (cond.Param1B)
                            flag = inf.IsFaceDown;
                        else
                            flag = inf.IsDown;
                    }
                    break;

                case HeatActionConditionType.DownOrGettingUp:
                    flag = inf.IsDown || inf.IsGettingUp;
                    break;
                case HeatActionConditionType.CriticalHP:
                    flag = actor.IsBrawlerCriticalHP();
                    break;
                case HeatActionConditionType.CharacterLevel:
                    flag = Logic.CheckNumberLogicalOperator(actor.GetStatus().Level, cond.Param1U32, cond.LogicalOperator);
                    break;
                case HeatActionConditionType.Job:
                    if (actor.IsPlayer())
                        flag = Logic.CheckNumberLogicalOperator((uint)Player.GetCurrentJob(Player.ID.kasuga), cond.Param1U32, cond.LogicalOperator);
                    break;
                case HeatActionConditionType.JobLevel:
                    if (actor.IsPlayer())
                        flag = Logic.CheckNumberLogicalOperator((uint)Player.GetJobLevel(Player.ID.kasuga), cond.Param1U32, cond.LogicalOperator);
                    break;
                case HeatActionConditionType.GettingUp:
                    flag = inf.IsGettingUp;
                    break;
                case HeatActionConditionType.Moving:
                    flag = inf.IsMove;
                    break;
                case HeatActionConditionType.Flinching:
                    flag = actor.Character.IsAnimDamage();
                    break;
                case HeatActionConditionType.CanAttackGeneric:
                    flag = !inf.CantAttackOverall();


                    bool dontAllowMidAttack = cond.Param1B == true;

                    if (dontAllowMidAttack)
                        if (performerIsPlayer)
                            if (AttackSimulator.PlayerInstance.Attacking())
                                flag = false;

                    break;
                case HeatActionConditionType.Grabbing:
                    if (performerIsPlayer)
                    {
                        if (!AttackSimulator.PlayerInstance.Attacking() || AttackSimulator.PlayerInstance.CurrentAttack.AttackType != AttackType.MoveSync)
                        {
                            flag = false;
                            break;
                        }

                        AttackSync grabSync = (AttackSimulator.PlayerInstance.CurrentAttack as AttackSync);
                        AttackSyncType syncType = grabSync.SyncType;
                        flag = syncType == AttackSyncType.GrabMovement || syncType == AttackSyncType.GrabIdle;

                        if (flag)
                        {
                            if (cond.Param1I32 != 0)
                            {
                                if (cond.Param1I32 == 1)
                                {
                                    if (grabSync.SyncDirection != AttackSyncDirection.Back)
                                        flag = false;
                                }
                                else
                                {
                                    if (grabSync.SyncDirection != AttackSyncDirection.Front)
                                        flag = false;
                                }
                            }

                            if(cond.Param2I32 != 0)
                            {
                                if (grabSync.SyncCategory != (AttackSyncCategory)cond.Param2I32)
                                    flag = false;
                            }
                        }

                    }
                    break;
                case HeatActionConditionType.DistanceToHactPosition:
                    Vector3 hactPos = new Vector3(attack.Position[0], attack.Position[1], attack.Position[2]);
                    float distToHact = Vector3.Distance(hactPos, (Vector3)actor.Character.Transform.Position);
                    flag = Logic.CheckNumberLogicalOperator(distToHact, cond.Param1F, cond.LogicalOperator);
                    break;
                case HeatActionConditionType.Distance:
                    bool range = cond.Param1B;

                    if (!fightersList.ContainsKey((HeatActionActorType)cond.Param1U32))
                        break;

                    Fighter distanceTarget = fightersList[(HeatActionActorType)cond.Param1U32];
                    float dist = Vector3.Distance(actor.Character.Transform.Position, distanceTarget.Character.Transform.Position);

                    if (range)
                    {
                        float minRange = cond.Param1F;
                        float maxRange = cond.Param2F;

                        flag = dist >= minRange && dist <= maxRange;
                    }
                    else
                        flag = Logic.CheckNumberLogicalOperator(dist, cond.Param1F, cond.LogicalOperator);

                    break;
                case HeatActionConditionType.AssetID:
                    flag = Logic.CheckNumberLogicalOperator((uint)inf.RightWeapon.Unit.AssetID, cond.Param1U32, cond.LogicalOperator);
                    break;
                case HeatActionConditionType.WeaponType:
                    flag = (uint)Asset.GetArmsCategory(inf.RightWeapon.Unit.AssetID) == cond.Param1U32;
                    break;
                case HeatActionConditionType.WeaponSubtype:
                    flag = (uint)Asset.GetArmsCategorySub(inf.RightWeapon.Unit.AssetID) == cond.Param1U32;
                    break;
                case HeatActionConditionType.StageID:
                    flag = (uint)SceneService.CurrentScene.Get().StageID == cond.Param1U32;
                    break;
                case HeatActionConditionType.EXHeat:
                    if (actor.IsPlayer())
                        flag = BrawlerPlayer.IsEXGamer;
                    break;
                case HeatActionConditionType.FacingTarget:
                    HeatActionActorType faceTarget = (HeatActionActorType)cond.Param1U32;

                    if (!fightersList.ContainsKey(faceTarget))
                        return false;

                    Character faceTargetActor = fightersList[faceTarget].Character;

                    float dot = Vector3.Dot(actor.Character.Transform.forwardDirection, (faceTargetActor.Transform.Position - actor.Character.Transform.Position).normalized);

                    flag = dot >= 0.2;
                    break;
                case HeatActionConditionType.Running:
                    if (performerIsPlayer)
                        flag = inf.IsMove && inf.MoveTime > 0.35f && !BrawlerHooks.HumanModeManager_IsInputKamae(actor.Character.HumanModeManager.Pointer);
                    break;
                case HeatActionConditionType.BattleStance:
                    if (performerIsPlayer)
                        flag = BrawlerHooks.HumanModeManager_IsInputKamae(actor.Character.HumanModeManager.Pointer);
                    break;
                case HeatActionConditionType.DistanceToNearestAsset:
                    flag = Logic.CheckNumberLogicalOperator(Vector3.Distance(actor.Character.GetPosCenter(), AssetManager.FindNearestAssetFromAll(actor.Character.GetPosCenter(), 0).Get().GetPosCenter()), cond.Param1F, cond.LogicalOperator);
                    break;
                case HeatActionConditionType.NearestAssetSpecialType:

                    AssetID specialAssetID = AssetManager.FindNearestAssetFromAll(actor.Character.GetPosCenter(), 0).Get().AssetID;
                    switch (cond.Param1U32)
                    {
                        default:
                            flag = false;
                            break;
                        case 1:
                            flag = specialAssetID.ToString().StartsWith("car");
                            break;
                        case 2:
                            flag = specialAssetID == AssetID.stgy162 || specialAssetID == AssetID.stgy131;
                            break;
                    }
                    break;
                case HeatActionConditionType.FacingRange:
                    HActRangeInfo facingInf = new HActRangeInfo();

                    if (!actor.GetStatus().HAct.GetPlayInfo(ref facingInf, (HActRangeType)cond.Param1U32))
                        return false;
                    else
                    {
                        float faceDot = Vector3.Dot(actor.Character.Transform.forwardDirection, ((Vector3)facingInf.Pos - actor.Character.Transform.Position).normalized);
                        flag = faceDot >= 0.2;
                    }
                    break;
                case HeatActionConditionType.DistanceToRange:
                    flag = Logic.CheckNumberLogicalOperator(Vector3.Distance(actor.Character.GetPosCenter(), (Vector3)rangeInf.Pos), cond.Param1F, cond.LogicalOperator);
                    break;
                case HeatActionConditionType.Attacking:
                    flag = actor.Character.HumanModeManager.IsAttack();
                    break;
                case HeatActionConditionType.NearestAssetFlag:
                    switch ((NearestAssetFlagType)cond.Param1U32)
                    {
                        case NearestAssetFlagType.Invalid:
                            flag = false;
                            break;
                        case NearestAssetFlagType.Distance:
                            flag = Logic.CheckNumberLogicalOperator(Vector3.Distance(actor.Character.GetPosCenter(), AssetManager.FindNearestAssetFromAll(actor.Character.GetPosCenter(), 0).Get().GetPosCenter()), cond.Param1F, cond.LogicalOperator);
                            break;
                        case NearestAssetFlagType.Type:
                            flag = cond.Param2U32 == (uint)Asset.GetArmsCategory(AssetManager.FindNearestAssetFromAll(actor.Character.GetPosCenter(), 0).Get().AssetID);
                            break;
                        case NearestAssetFlagType.Subtype:
                            flag = cond.Param2U32 == Asset.GetArmsCategorySub(AssetManager.FindNearestAssetFromAll(actor.Character.GetPosCenter(), 0).Get().AssetID);
                            break;
                        case NearestAssetFlagType.CanBreak:
                            flag = AssetManager.FindNearestAssetFromAll(actor.Character.GetPosCenter(), 0).Get().IsCanBreak();
                            break;
                        case NearestAssetFlagType.EntityUID:
                            flag = cond.Param2U64 == AssetManager.FindNearestAssetFromAll(actor.Character.GetPosCenter(), 0).Get().EntityUID.UID;
                            break;
                        case NearestAssetFlagType.AssetID:
                            flag = cond.Param2U32 == (uint)AssetManager.FindNearestAssetFromAll(actor.Character.GetPosCenter(), 0).Get().AssetID;
                            break;
                    }
                    break;
                case HeatActionConditionType.InRange:

                    HActRangeInfo inRangeInf = new HActRangeInfo();
                    flag = actor.GetStatus().HAct.GetPlayInfo(ref inRangeInf, (HActRangeType)cond.Param1U32);
                    break;

                case HeatActionConditionType.IsBoss:
                    flag = actor.IsBoss();
                    break;
                case HeatActionConditionType.Health:
                    ECBattleStatus battleStat = actor.GetStatus();

                    if (cond.Param1B)
                        flag = Logic.CheckNumberLogicalOperator(battleStat.CurrentHP, battleStat.MaxHP * (cond.Param1F / 100f), cond.LogicalOperator);
                    else
                        flag = Logic.CheckNumberLogicalOperator(battleStat.CurrentHP, cond.Param1U64, cond.LogicalOperator);
                    break;

                case HeatActionConditionType.CtrlType:
                    flag = Logic.CheckNumberLogicalOperator((uint)actorAttributes.ctrl_type, cond.Param1U32, cond.LogicalOperator);
                    break;

                case HeatActionConditionType.SoldierID:
                    flag = Logic.CheckNumberLogicalOperator((uint)actorAttributes.soldier_data_id, cond.Param1U32, cond.LogicalOperator);
                    break;

                case HeatActionConditionType.BattleConfigID:
                    flag = Logic.CheckNumberLogicalOperator(BattleProperty.BattleConfigID, cond.Param1U32, cond.LogicalOperator);
                    break;

                case HeatActionConditionType.WouldDieInHAct:
                    bool isCalcPlayer = cond.Param1B;

                    Fighter calcVictim;
                    Fighter calcAttacker;

                    if (isCalcPlayer)
                    {
                        calcAttacker = BrawlerBattleManager.Kasuga;
                        calcVictim = actor;
                    }
                    else
                    {
                        calcAttacker = actor;
                        calcVictim = BrawlerBattleManager.Kasuga;
                    }

                    flag = HActDamage.CalculateDamage(calcAttacker, calcVictim, cond.Param1U32, 0, 0, false, false, false).Dies;
                    break;

                case HeatActionConditionType.MotionID:
                    flag = (uint)actor.Character.GetMotion().GmtID == cond.Param1U32;
                    break;

                case HeatActionConditionType.InBepElementRange:
                    MotionService.TimingResult elementWindow = MotionService.SearchTimingDetail(0, actor.Character.GetMotion().BepID, cond.Param1U32);
                    flag = elementWindow.IsValid() && actor.Character.GetMotion().InTimingRange(cond.Param1U32);
                    break;

                case HeatActionConditionType.BeingJuggled:
                    if (ai == null)
                        return false;

                    flag = ai.IsBeingJuggled();
                    break;
            }

            switch (cond.LogicalOperator)
            {
                case LogicalOperator.TRUE:
                    if (flag)
                        return true;
                    else
                        return false;
                case LogicalOperator.FALSE:
                    if (!flag)
                        return true;
                    else
                        return false;
            }

            return flag;
        }
    }
}

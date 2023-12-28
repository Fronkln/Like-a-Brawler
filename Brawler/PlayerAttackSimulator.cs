using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DragonEngineLibrary;
using DragonEngineLibrary.Service;
using YazawaCommand;

namespace Brawler
{
    public class AttackSimulator
    {
        public static AttackSimulator PlayerInstance = new AttackSimulator();

        public Fighter Attacker;
        public ECMotion AttackerMotion;

        public bool m_attacking = false;
        private bool m_attackGmtReady = false;
        private bool m_hadWeaponThisAttack = false;

        private YFC m_currentSet;
        public AttackGroup m_currentGroup;
        public Attack CurrentAttack;

        private MotionID m_attackGmt = MotionID.invalid;
        private AttackSyncType m_currentSyncType = AttackSyncType.Invalid;
        private MotionService.TimingResult m_followupWindow;

        private float m_attackTime = 0f;

        //LIMITATION: 1 TAME PER ATTACK
        private bool m_specialTame = false;
        private bool m_tameProcedure = false;
        private bool m_tameMidDuration = false;
        private float m_tameTime = 0;
        private MotionService.TimingResult m_tamePeriod;
        private AttackInputID m_tameInput;


        //Buffered attack inputs only.
        private Dictionary<AttackInputID, bool> m_pressedInputsThisAttack = new Dictionary<AttackInputID, bool>();
        private bool m_animationEndedThisAttack = false;
        private bool m_attackHitThisAttack = false;
        private bool m_counterAttackThisFrame = false;
        public Fighter LastEnemyHitCurrentCombo = new Fighter((IntPtr)0);


        private Dictionary<Attack, float> m_cooldowns = new Dictionary<Attack, float>();

        private float m_lastLeverAngle;

        public AttackSimulator()
        {
            foreach (AttackInputID val in (AttackInputID[])Enum.GetValues(typeof(AttackInputID)))
                m_pressedInputsThisAttack[val] = false;

            Stop();
        }

        public void Stop()
        {
            m_attacking = false;
            LastEnemyHitCurrentCombo = new Fighter();
            m_currentGroup = null;
            m_currentSet = null;
            CurrentAttack = null;
            m_attackGmtReady = false;
            m_currentSyncType = AttackSyncType.Invalid;

            ClearInputs();
        }

        public bool Attacking()
        {
            return m_attacking && CurrentAttack != null;
        }


        public void InputUpdatePreAttack(YFC attackSet)
        {
            if (BattleTurnManager.CurrentPhase != BattleTurnManager.TurnPhase.Action)
                return;

            // if (/*Attacker.Character.HumanModeManager.IsAttack() ||*/ m_attackGmtReady || m_attacking || attackSet == null)
            // return;

            if (attackSet == null)
                return;

            if (Attacker.IsPlayer() && BrawlerPlayer.CanPickupWeapon())
                return;

            if (attackSet.Groups.Count <= 0)
                return;

            m_currentSet = attackSet;
            m_currentGroup = attackSet.Groups[0];

            foreach (Attack attack in m_currentGroup.Attacks)
            {
                if (CheckConditions(attack))
                {
                    ExecuteAttack(attackSet, attack);
                    break;
                }
            }
        }

        public void InputUpdateAttack(YFC attackSet)
        {
            try
            {
                if (BrawlerBattleManager.HActIsPlaying)
                {
                    Stop();
                    return;
                }

                if (!m_attacking || !m_attackGmtReady)
                    return;

                MotionService.TimingResult controlLock = AttackerMotion.GetTiming(72);

                if (controlLock.IsValid())
                {
                    uint curTick = AttackerMotion.PlayInfo.tick_gmt_now_;

                    if (AttackerMotion.PlayInfo.tick_gmt_now_ >= controlLock.Start)
                    {
                       // if (Attacker.Character.HumanModeManager.IsInputMove())
                        //{
                        if(Attacker.Character.HumanModeManager.IsInputMove())
                            Attacker.Character.HumanModeManager.ToEndReady();

                            LastEnemyHitCurrentCombo = new Fighter();
                            m_attacking = false;
                            m_attackGmtReady = false;
                            CurrentAttack = null;
                            ClearInputs();
                            return;
                        
                    }
                }


                MotionService.TimingResult followupWindow = MotionService.SearchTimingDetail(0, AttackerMotion.BepID, 71);

                // if (followupWindow.IsValid())
                // {
                //Current group null means the attack doesnt transition to anything. Wait for animation end
                if (m_currentGroup != null && !(m_hadWeaponThisAttack && !Attacker.GetWeapon(AttachmentCombinationID.right_weapon).Unit.IsValid()))
                {
                    foreach (Attack attack in m_currentGroup.Attacks)
                    {
                        bool canExec = CheckConditions(attack);

                        if (canExec)
                        {
                            ExecuteAttack(attackSet, attack);
                            break;
                        }
                    }
                    // }
                }
            }
            catch
            {
                DragonEngine.Log("Error during AttackSimulator.PlayerInstance.InputUpdateAttack, stopping the sequence.");
                Stop();
            }
        }

        public bool IsInFollowupWindow()
        {
            MotionService.TimingResult timing = AttackerMotion.GetTiming(71, 0);
            return timing.IsValid() && AttackerMotion.InTimingRange(71, 0);
        }

        public void GameUpdate()
        {
            ProcessCooldowns();

            if (!m_attacking || !m_attackGmtReady)
                return;

            Character attackerChara = Attacker.Character;

            if (Attacking() && CurrentAttack != null)
                switch (CurrentAttack.AttackType)
                {
                    case AttackType.MoveSync:
                        AttackSync sync = CurrentAttack as AttackSync;


                        // AttackerMotion.NormalizedTime > 0.99f

                        if (sync.Loop && AttackerMotion.Frame >= AttackerMotion.Frames - 2) 
                        {
                            unsafe
                            {
                                IntPtr dat = SyncManager.GetPlayingData(AttackerMotion.SyncSerial);

                                if (dat != IntPtr.Zero)
                                {
                                    uint* max = (uint*)(dat.ToInt64() + 0x3C);
                                    *max = 0;

                                    AttackerMotion.SetFrame(0);
                                    Attacker.GetSyncPair().Get().GetMotion().SetFrame(0);
                                }
                            }
                        }

                        if (Attacker.IsSync() && Attacker.IsPlayer() && sync.MoveSync)
                        {
                            float movement = sync.MoveSpeed * DragonEngine.deltaTime;

                            if (sync.InvertDirection)
                                movement = movement * -1f;

                            attackerChara.RequestMovePose(new PoseInfo(attackerChara.GetPosCenter() + (attackerChara.Transform.forwardDirection * movement), attackerChara.Pad.LeverWorldAng));
                        }

                        break;
                }

            if (!m_tameProcedure || (!m_specialTame && !m_tamePeriod.IsValid()))
                return;

            m_tameTime += DragonEngine.deltaTime;

            if (Attacker.IsPlayer())
            {
                if (AttackerMotion.PlayInfo.tick_gmt_now_ >= m_tamePeriod.Start && AttackerMotion.PlayInfo.tick_gmt_now_ < m_tamePeriod.End)
                {
                    if (!ModInput.Holding(m_tameInput))
                    {
                        m_tameProcedure = false;
                        return;
                    }

                    if(!m_tameMidDuration)
                    {
                        uint midDuration = (uint)Extensions.Lerp(m_tamePeriod.Start, m_tamePeriod.End, 0.70f);


                        DragonEngine.Log(AttackerMotion.PlayInfo.tick_gmt_now_);

                        if(AttackerMotion.PlayInfo.tick_gmt_now_ >= midDuration)
                        {
                            m_tameMidDuration = true;
                            SoundManager.PlayCue(5, 13, 0);
                        }

                    }

                    AttackerMotion.SetTempSpeed(0.15f);
                }
            }
        }


        //Doing some updates in Draw gives us more control abotu commanding the player
        //SetAngleY wont work in Job Update for example, but will do so in Draw.
        public void AttackDrawUpdate()
        {
            if (!m_attacking || !m_attackGmtReady)
                return;

            if (CurrentAttack.AttackType == AttackType.MoveGMTOnly)
                Attacker.Character.Status.SetNoInputTemporary();

            if (AttackerMotion.Frame >= AttackerMotion.Frames - 1 || AttackerMotion.GmtID == 0)
            {
                if (m_currentGroup != null)
                {
                    m_animationEndedThisAttack = true;

                    foreach (Attack attack in m_currentGroup.Attacks.Where(x => x.HasConditionOfType(AttackConditionType.AnimationOver)))
                        if (CheckConditions(attack))
                        {
                            ExecuteAttack(m_currentSet, attack);
                            return;
                        }
                }

                m_attacking = false;
                m_attackGmtReady = false;
                m_currentSet = null;
                LastEnemyHitCurrentCombo = new Fighter();
                CurrentAttack = null;
                ClearInputs();
                return;
            }
        }
        private void ProcessCooldowns()
        {
            List<Attack> m_availableAtks = new List<Attack>();

            for (int i = 0; i < m_cooldowns.Count; i++)
            {
                var kv = m_cooldowns.ElementAt(i);

                float cd = m_cooldowns[kv.Key] - DragonEngine.deltaTime;

                if (cd <= 0)
                {
                    m_availableAtks.Add(kv.Key);
                    continue;
                }

                m_cooldowns[kv.Key] = cd;
            }

            foreach (Attack atk in m_availableAtks)
                m_cooldowns.Remove(atk);
        }

        public void ExecuteAttack(YFC moveset, Attack attack)
        {
            if (attack == null)
                return;

            ClearInputs();
            m_currentSyncType = AttackSyncType.Invalid;
            m_attacking = true;
            m_attackGmtReady = false;
            m_animationEndedThisAttack = false;
            m_hadWeaponThisAttack = Attacker.GetWeapon(AttachmentCombinationID.right_weapon).Unit.IsValid();
            m_attackTime = 0;

            if (attack.Cooldown > 0)
                m_cooldowns[attack] = attack.Cooldown;

            if (Attacker.IsSync())
            {
                Attacker.Character.HumanModeManager.ToEndReady();
                Attacker.GetSyncPair().Get().HumanModeManager.ToEndReady();
                new DETask(delegate { return !Attacker.IsSync(); }, delegate { OnReadyExecuteAttack(moveset, attack); });

                /*
                if (LastEnemyHitCurrentCombo != null)
                {
                    LastEnemyHitCurrentCombo.Character.HumanModeManager.ToEndReady();
                    //making a very scary assumption that the nearest 
                    new DETask(delegate { return !Attacker.IsSync(); }, delegate { OnReadyExecuteAttack(moveset, attack); });
                }
                else
                    Stop(); //failsafe
                */
            }
            else
                OnReadyExecuteAttack(moveset, attack);

            foreach (AttackCondition input in attack.GetAllInputConditions().Where(x => x.Param2B))
                ModInput.Input[(AttackInputID)input.Param1U32].TimeHeld = 0;

        }

        private void OnReadyExecuteAttack(YFC moveset, Attack attack)
        {
            ClearInputs();

            m_attacking = true;
            CurrentAttack = attack;

            AttackCondition tameCondition = attack.GetAllInputConditions().FirstOrDefault(x => x.Param1I32 == 2);


            if (tameCondition != null)
            {
                m_tamePeriod.Start = (int)new GameTick(tameCondition.Param1F).Tick;
                m_tamePeriod.End = (int)new GameTick(tameCondition.Param2F).Tick;
                m_tameInput = (AttackInputID)tameCondition.Param1U32;
                m_tameProcedure = true;
                m_tameMidDuration = false;
            }
            else
            {
                m_tamePeriod.Start = -1;
                m_tamePeriod.End = -1;
                m_tameProcedure = false;
            }

            switch (attack.AttackType)
            {
                case AttackType.MoveCFC:
                    AttackCFC cfcAtk = attack as AttackCFC;
                    Attacker.Character.HumanModeManager.ToAttackMode(new FighterCommandID(cfcAtk.MovesetID, cfcAtk.Index));
                    break;
                case AttackType.MoveRPG:
                    AttackRPG rpgAtk = attack as AttackRPG;

                    //TODO: IF ADDING PARTY MEMBERS UNHARDCODE TARGETING KASUGA
                    if (Attacker.IsPlayer())
                        if(BrawlerBattleManager.EnemiesNearest.Length > 0)
                        BattleTurnManager.ForceCounterCommand(Attacker, BrawlerBattleManager.EnemiesNearest[0], (RPGSkillID)rpgAtk.ID);
                    else
                        BattleTurnManager.ForceCounterCommand(Attacker, BrawlerBattleManager.Kasuga, (RPGSkillID)rpgAtk.ID);
                    break;
                case AttackType.MoveSync:
                    m_currentSyncType = (attack as AttackSync).SyncType;
                    goto case AttackType.MoveRPG;
                case AttackType.MoveGMTOnly:
                    AttackGMT motionAtk = attack as AttackGMT;
                    AttackerMotion.RequestGMT((MotionID)motionAtk.MotionID);
                    break;
                case AttackType.MoveSidestep:
                    Attacker.Character.HumanModeManager.ToSway();
                    break;
                case AttackType.MoveCFCRange:
                    AttackCFCRange cfcRangeAtk = attack as AttackCFCRange;

                    HActRangeInfo cfcRangeAtkInf = new HActRangeInfo();
                    uint rangeID = (uint)cfcRangeAtkInf.Range;

                    if(BrawlerBattleManager.Kasuga.GetStatus().HAct.GetPlayInfo(ref cfcRangeAtkInf, HActRangeType.hit_wall))
                    {
                        Vector3 finalPos = (Vector3)cfcRangeAtkInf.Pos;
                        finalPos += (cfcRangeAtkInf.Rot * Vector3.forward) * cfcRangeAtk.OffsetForward;
                        finalPos += (cfcRangeAtkInf.Rot * Vector3.up) * cfcRangeAtk.OffsetUp;
                        finalPos += (cfcRangeAtkInf.Rot * -Vector3.right) * cfcRangeAtk.OffsetLeft;

                        Attacker.Character.WarpPosAndOrient(Attacker.Character.Transform.Position, cfcRangeAtkInf.Rot);
                        Attacker.Character.RequestWarpPose(new PoseInfo(finalPos, Attacker.Character.GetAngleY()));
                    }

                    Attacker.Character.HumanModeManager.ToAttackMode(new FighterCommandID(cfcRangeAtk.MovesetID, cfcRangeAtk.Index));
                    break;

            }

            if (moveset != null)
            {
                if (attack.TransitionGroup >= moveset.Groups.Count)
                {
                    m_currentGroup = null;
                    DragonEngine.Log("INDEXING ERROR! " + attack.Name + " " + attack.TransitionGroup + " " + moveset.Groups.Count);
                }
                else
                    m_currentGroup = (attack.TransitionGroup > -1 ? moveset.Groups[attack.TransitionGroup] : null);
            }

            new DETaskList
                (
                 new DETaskNextFrame(),
                 new DETaskNextFrame(),
                 new DETaskNextFrame(),
                 new DETaskNextFrame(OnGMTReady)
            );
        }

        private void OnGMTReady()
        {
            m_attackGmt = AttackerMotion.GmtID;
            m_followupWindow = MotionService.SearchTimingDetail(AttackerMotion.PlayInfo.tick_now_, AttackerMotion.BepID, 71);
            m_attackGmtReady = true;
        }

        public void ExecuteSingleCFCAttack(FighterCommandID attack)
        {
            ExecuteAttack(null, new AttackCFC() { MovesetID = attack.set_, Index = attack.cmd });
        }

        public void ExecuteSingleGMTAttack(MotionID gmt)
        {
            ExecuteAttack(null, new AttackGMT() { MotionID = (uint)gmt });
        }

        //Determined by the damage transit hooks
        public void OnAttackLanded(Fighter victim, BattleDamageInfoSafe dmg)
        {
            if (!Attacking())
                return;

            m_attackHitThisAttack = true;
            LastEnemyHitCurrentCombo = victim;
        }

        public bool TransitCounter(Fighter attacker, BattleDamageInfoSafe dmg)
        {
            if (m_currentGroup == null)
                return false;

            m_counterAttackThisFrame = true;

            Attack counterAttack = null;

            foreach (Attack attack in m_currentGroup.Attacks.Where(x => x.IsCounterAttack()))
            {
                if (CheckConditions(attack))
                {
                    counterAttack = attack;
                    break;
                }
            }

            m_counterAttackThisFrame = false;

            if (counterAttack != null)
            {
                ExecuteAttack(m_currentSet, counterAttack);
                LastEnemyHitCurrentCombo = attacker;
                return true;
            }

            return false;
        }

        private bool CheckConditions(Attack attack)
        {
            if (m_cooldowns.ContainsKey(attack))
                return false;

            foreach (AttackCondition cond in attack.Conditions)
            {
                if (!CheckFlag(cond))
                    return false;
            }

            return true;
        }

        private bool CheckFlag(AttackCondition cond)
        {
            bool flag = false;

            BrawlerFighterInfo inf;

            if (Attacker.IsPlayer())
                inf = BrawlerPlayer.Info;
            else
            {
                EnemyAI ai = EnemyManager.GetAI(Attacker);

                if (ai == null)
                    return false;

                inf = ai.Info;
            }


            switch (cond.Type)
            {
                case AttackConditionType.Down:
                    flag = inf.IsDown;
                    break;
                case AttackConditionType.AttackHit:
                    flag = m_attackHitThisAttack;
                    break;
                case AttackConditionType.CanAttackOverall:
                    flag = !inf.CantAttackOverall();
                    break;
                case AttackConditionType.AnimationOver:
                    flag = m_animationEndedThisAttack;
                    break;
                case AttackConditionType.GettingUp:
                    flag = inf.IsGettingUp;
                    break;
                case AttackConditionType.IsExtremeHeat:
                    if (Attacker.IsPlayer())
                        flag = BrawlerPlayer.IsEXGamer;
                    break;
                case AttackConditionType.LowHealth:
                    flag = Attacker.IsBrawlerCriticalHP();
                    break;
                case AttackConditionType.IsFlinching:
                    flag = BrawlerPlayer.Info.IsFlinching;
                    break;
                case AttackConditionType.CharacterLevel:

                    if (Attacker.IsPlayer())
                    {
                        uint charaLevel = Player.GetLevel(Player.ID.kasuga);
                        flag = CheckNumberLogicalOperator(charaLevel, cond.Param1U32, cond.LogicalOperator);
                    }
                    else
                        flag = CheckNumberLogicalOperator(Attacker.GetStatus().Level, cond.Param1U32, cond.LogicalOperator);
                    break;
                case AttackConditionType.CharacterJobLevel:
                    if (Attacker.IsPlayer())
                    {
                        uint jobLevel = Player.GetJobLevel(Player.ID.kasuga);
                        flag = CheckNumberLogicalOperator(jobLevel, cond.Param1U32, cond.LogicalOperator);
                    }
                    break;

                case AttackConditionType.Running:
                    flag = CheckNumberLogicalOperator(inf.MoveTime, cond.Param1F, cond.LogicalOperator);
                    break;

                //TODO: ADD ENEMY VER AND UNHARDCODE KASUGA FOR PARTY MEMBERS
                case AttackConditionType.LockedToEnemy:
                    if (Attacker.IsPlayer())
                        flag = BrawlerPlayer.IsInputKamae();
                    break;
                case AttackConditionType.LockedEnemyDown:
                    Fighter lockOnTarget = new Fighter();

                    if (Attacker.IsPlayer())
                        lockOnTarget = BrawlerPlayer.GetLockOnTarget(BrawlerBattleManager.Kasuga);
                    else
                        lockOnTarget = BrawlerBattleManager.Kasuga;
                    flag = lockOnTarget.IsValid() && lockOnTarget.IsDown();
                    break;


                case AttackConditionType.JobID:
                    if (Attacker.IsPlayer())
                        flag = CheckNumberLogicalOperator(cond.Param1U32, (uint)Player.GetCurrentJob(Player.ID.kasuga), cond.LogicalOperator);
                    break;

                case AttackConditionType.Sync:
                    flag = Attacker.IsSync();
                    break;

                case AttackConditionType.MoveInput:
                    flag = Attacker.Character.HumanModeManager.IsInputMove();
                    break;
                //TODO: ADD KASUGA VERSION
                case AttackConditionType.EnemyResponse:
                    Fighter respondingEnemy = inf.IsSync ? Attacker.GetSyncPair().Get().GetFighter() : BrawlerBattleManager.EnemiesNearest[0];
                    EnemyAI respondingAI = EnemyManager.GetAI(respondingEnemy);

                    if (respondingAI == null)
                        return false;

                    switch (cond.Param1U32)
                    {
                        case 1:
                            flag = respondingAI.ResponseFlags[(EnemyAIResponse)cond.Param1U32];
                            break;
                    }
                    break;
                case AttackConditionType.SyncType:
                    if (CurrentAttack == null || CurrentAttack.AttackType != AttackType.MoveSync)
                        return false;

                    flag = (CurrentAttack as AttackSync).SyncType == (AttackSyncType)cond.Param1U32;
                    break;

                case AttackConditionType.SyncDirection:
                    if (CurrentAttack == null || CurrentAttack.AttackType != AttackType.MoveSync)
                        return false;

                    flag = (CurrentAttack as AttackSync).SyncDirection == (AttackSyncDirection)cond.Param1U32;
                    break;

                case AttackConditionType.InputKey:

                    //Already monitored by game
                    if (/*!Mod.IsGameFocused || Mod.IsGamePaused ||*/ !Attacker.IsPlayer())
                        break;

                    AttackInputID input = (AttackInputID)cond.Param1U32;
                    bool bufferedAttack = cond.Param1B;
                    bool hold = cond.Param2B;
                    bool counterAttackTiming = cond.Param1I32 == 1;
                    bool tame = cond.Param1I32 == 2;


                    AttackCondition chargingUpHold = null;

                    //not the first attack which means we will calculate followup here.
                    if (m_attacking)
                    {
                        if (counterAttackTiming)
                        {
                            if (!m_counterAttackThisFrame)
                                flag = false;
                            else
                                flag = ModInput.IsTimingPush(input, 0.85f);
                            break;
                        }

                        if (!bufferedAttack)
                        {
                            MotionService.TimingResult timing = AttackerMotion.GetTiming(71, 0);

                            float holdTime = cond.Param1F;

                            //If timing is valid and we are within range or there is no timing and we press
                            if ((timing.IsValid() && AttackerMotion.InTimingRange(71, 0)))
                            {
                                if (!hold)
                                    flag = ModInput.JustPressed(input);
                                else
                                    flag = ModInput.Holding(input) && ModInput.TimeHeld(input) >= holdTime;
                            }
                            else
                            {
                                if (!timing.IsValid())
                                {
                                    if (!hold)
                                        flag = ModInput.JustPressed(input);
                                    else
                                        flag = ModInput.Holding(input) && ModInput.TimeHeld(input) >= holdTime;
                                }

                            }
                        }
                        else
                        {
                            MotionService.TimingResult timing = AttackerMotion.GetTiming(71, 0);

                            if (timing.IsValid())
                            {
                                if (BrawlerPlayer.MotionInfo.tick_gmt_now_ < timing.Start)
                                {
                                    if ((!hold && ModInput.JustPressed(input) || (hold && (ModInput.JustPressed(input) || ModInput.Holding(input)))))
                                        m_pressedInputsThisAttack[input] = true;
                                }
                                else if (m_pressedInputsThisAttack[input])
                                    flag = true;
                            }
                        }
                    }
                    else
                    {
                        if (counterAttackTiming)
                        {
                            if (!m_counterAttackThisFrame)
                                flag = false;
                            else
                                flag = ModInput.TimeHeld(input) > 0 && ModInput.TimeHeld(input) <= 0.7f;
                            break;
                        }
                        else if (tame)
                        {
                            if (!m_tameProcedure)
                            {
                                m_tameProcedure = true;
                                flag = false;
                            }
                        }

                        if (hold)
                        {
                            float holdTime = cond.Param1F;

                            if (ModInput.Holding(input))
                            {
                                if (ModInput.TimeHeld(input) < holdTime)
                                    chargingUpHold = cond;
                                else
                                    flag = true;
                            }
                        }
                        else
                        {
                            if (input == AttackInputID.Grab)
                                if (BrawlerPlayer.CanPickupWeapon())
                                    return false;

                            if (chargingUpHold != null && ModInput.JustPressed(input) && cond.Param1U32 == chargingUpHold.Param1U32)
                                flag = false;
                            else
                                flag = ModInput.JustPressed(input);
                        }
                    }

                    break;

                case AttackConditionType.DistanceToRange:
                    HActRangeInfo rangeDistInf = new HActRangeInfo();

                    if (Attacker.GetStatus().HAct.GetPlayInfo(ref rangeDistInf, (HActRangeType)cond.Param1U32))
                        flag = Logic.CheckNumberLogicalOperator(Vector3.Distance(Attacker.Character.GetPosCenter(), (Vector3)rangeDistInf.Pos), cond.Param1F, cond.LogicalOperator);
                    else
                        return false;
                    break;


                case AttackConditionType.FacingRange:
                    HActRangeInfo facingInf = new HActRangeInfo();


                    if (Attacker.GetStatus().HAct.GetPlayInfo(ref facingInf, (HActRangeType)cond.Param1U32))
                    {
                        float faceDot = Vector3.Dot(Attacker.Character.Transform.forwardDirection, ((Vector3)facingInf.Pos - Attacker.Character.Transform.Position).normalized);
                        flag = faceDot >= 0.2;
                    }
                    else
                        return false;
                    break;

                case AttackConditionType.NearestEnemyFlag:

                    if (BrawlerBattleManager.EnemiesNearest.Length <= 0)
                        flag = false;

                    Fighter nearestEnemy = BrawlerBattleManager.EnemiesNearest[0];

                    switch((NearestEnemyFlag)cond.Param1U32)
                    {
                        default:
                            flag = true;
                            break;

                        case NearestEnemyFlag.IsDown:
                            return inf.IsDown;
                        case NearestEnemyFlag.IsGettingUp:
                            return inf.IsGettingUp;
                        case NearestEnemyFlag.FacingPlayer:
                            float faceDot = Vector3.Dot(nearestEnemy.Character.Transform.forwardDirection, (BrawlerBattleManager.KasugaChara.Transform.Position - nearestEnemy.Character.Transform.Position).normalized);
                            flag = faceDot >= 0.2;
                            break;
                        case NearestEnemyFlag.Distance:
                            float dist = Vector3.Distance(nearestEnemy.Character.Transform.Position, BrawlerBattleManager.KasugaChara.Transform.Position);
                            flag = CheckNumberLogicalOperator(dist, cond.Param1F, cond.LogicalOperator);
                            break;
                    }

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

        public static bool CheckNumberLogicalOperator(double value, double target, LogicalOperator logicOperator)
        {
            value = Math.Round(value, 3);
            target = Math.Round(target, 3);

            switch (logicOperator)
            {
                default:
                    return false;
                case LogicalOperator.TRUE:
                    goto case LogicalOperator.EQUALS;
                case LogicalOperator.FALSE:
                    goto case LogicalOperator.NOT_EQUALS;
                case LogicalOperator.EQUALS:
                    return value == target;
                case LogicalOperator.NOT_EQUALS:
                    return value != target;
                case LogicalOperator.EQUALS_GREATER:
                    return value >= target;
                case LogicalOperator.EQUALS_LESS:
                    return value <= target;
            }
        }
        private void ClearInputs()
        {
            for (int i = 0; i < m_pressedInputsThisAttack.Count; i++)
                m_pressedInputsThisAttack[(AttackInputID)i] = false;

            m_tameTime = 0;
            m_attackHitThisAttack = false;
        }
    }
}

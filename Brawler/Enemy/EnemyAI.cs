using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Brawler.Enemy;
using DragonEngineLibrary;
using DragonEngineLibrary.Service;
using DragonEngineLibrary.Unsafe;
using MinHook.NET;
using YazawaCommand;

namespace Brawler
{
    //Callback based Enemy AI
    internal unsafe class EnemyAI
    {
        public struct BrawlerAIFlags
        {
            public bool ShouldGuardBreakFlag;
        }

        public Fighter Character;
        public EntityHandle<Character> Chara;

        public BrawlerFighterInfo Info;
        public BrawlerFighterInfo PreviousInfo;
        public BrawlerAIFlags Flags;

        public float LastHitTime = 10000;
        public float LastGuardTime = 10000;


        /// <summary>
        /// 0 to 1.0f ratio, how much percentage of damage will we resist from a heat action?
        /// </summary>
        public float HeatActionDamageResist = 0f;

        public bool IsHuman { get; private set; }
        public bool IsMachinery { get; private set; }

        public float DistanceToPlayer { get; protected set; }


        //AI Response
        public Dictionary<EnemyAIResponse, bool> ResponseFlags = new Dictionary<EnemyAIResponse, bool>();

        //counter attacking

        private bool m_gettingUp = false;
        private bool m_getupHyperArmorDoOnce = false;

        /// <summary>
        /// Set true when we want to show counter attack effect, then set false after
        /// </summary>
        protected bool m_allowCounterEffectDoOnce = false;

        public int RecentDefensiveAttacks = 0;
        //We can determine if RNG has failed us and force things as needed
        //Primarily meant for bosses
        public int RecentHitsWithoutDefensiveMove = 0;

        /// <summary>
        /// Recent total hits we ate without attacking, this includes evasion/guarding
        /// </summary>
        public int RecentHitsWithoutAttack;

        //Player is spamming same attacks 0 iq
        private int RecentHits = 0;

        private float m_grabbedTime = 0;
        private float m_lastTimeSinceGrabbed = float.MaxValue;

        //Time for the AI to issue shakeoff command
        private const float Y7B_BTL_AI_GRAB_PATIENCE_TIME = 2.5f;
        private const float Y7B_BTL_AI_SPM_TIME = 1.8f;

        //Amount of spam hits that makes the AI realize its being spam attacked
        private const int Y7B_BTL_AI_SPM_COUNT = 5;

        public List<RPGSkillID> CounterAttacks = new List<RPGSkillID>();

        public EnemyEvasionModule EvasionModule = null;
        public EnemyBlockModule BlockModule = null;
        public EnemySyncHActModule SyncHActModule = null;
        public EnemyJuggleModule JuggleModule = null;

        /// <summary>
        /// List of heat actions we have done.
        /// </summary>
        protected HashSet<TalkParamID> m_performedHacts = new HashSet<TalkParamID>();

        public event Action OnGetUp;
        public event Action OnCounterAttack;

        public virtual void Start()
        {
            for (int i = 0; i < (int)EnemyAIResponse.Count; i++)
                ResponseFlags[(EnemyAIResponse)i] = false;

            BlockModule = new EnemyBlockModule() { AI = this };
            EvasionModule = new EnemyEvasionModule() { AI = this };
            SyncHActModule = new EnemySyncHActModule() { AI = this };
            JuggleModule = new EnemyJuggleModule() { AI = this};
#if DEBUG
            Console.WriteLine(Chara.Get().Attributes.soldier_data_id + " " + 
                Chara.Get().Attributes.enemy_id + " " + 
                Chara.Get().Attributes.ctrl_type + " Arts: " + 
                Character.GetStatus().GetArts() + " Asset: " + 
                Character.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get().AssetID + "(" + Asset.GetArmsCategory(Character.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get().AssetID) + ")" + "(" + Asset.GetArmsCategorySub(Character.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get().AssetID) + ")" +
                 " " + "Is Boss?: " + IsBoss());
#endif


            CharacterAnimalKind animalKind = Chara.Get().Attributes.animal_kind;

            IsHuman = animalKind == CharacterAnimalKind.human;
            IsMachinery = animalKind == CharacterAnimalKind.cranetruck || animalKind == CharacterAnimalKind.excavator || animalKind == CharacterAnimalKind.clean_robot;

            OnGetUp += new Action(EvasionModule.OnGetUp);

            new DETaskTime(2.0f, LateStart);
        }

        //Called when the Battle AI is FULLY initialized
        EnemyAIHook.EnemyAIChooseCustomAttack _selDeleg;
        EnemyAIHook.EnemyAIChooseCustomAttack _trampoline;
        public virtual void LateStart()
        {
        }

        public virtual void InitResources()
        {

        }

        //Decide on a strategy based on player input
        private void ReadPlayerInput()
        {
            if (m_gettingUp)
                return;

            if (AttackSimulator.PlayerInstance.CurrentAttack == null)
            {
                BlockModule.RecentlyBlockedHits = 0;
                return;
            }

            Attack playerMove = AttackSimulator.PlayerInstance.CurrentAttack;

            /*
            if(playerMove.AttackType == AttackType.MoveComboString)
            {
                //We are being combod
                //We treat this seperately because it depends on last guard time which is RNG
                //Gonna be different from sidesteps etc
                if (LastHitTime < Y7B_BTL_AI_SPM_TIME && LastGuardTime < Y7B_BTL_AI_SPM_TIME)
                    m_forceGuard = true;
            }
            */

        }

        /// <summary>
        /// Would this attack kill the enemy
        /// </summary>
        public unsafe bool WouldDieToDamage(BattleDamageInfoSafe dmg)
        {
            ECBattleStatus status = Character.GetStatus();

            if (status.CurrentHP - dmg.Damage <= 0)
                return true;
            else
                return false;
        }

        public void Update()
        {
            PreviousInfo = Info;
            Info = new BrawlerFighterInfo() { Fighter = Character };
            Info.Update(Character);
            ReadPlayerInput();

            MotionID mtnID = Character.Character.GetMotion().GmtID;

            if (!m_gettingUp && Info.IsGettingUp)
                OnGetUp?.Invoke();

            m_gettingUp = Info.IsGettingUp;

            CombatUpdate();
            HActProcedure();

            float delta = DragonEngine.deltaTime;

            LastGuardTime += delta;

            //Helps with timings even if it doesnt look like it makes sense.
            if(!Info.IsDown && !Info.IsRagdoll)
                LastHitTime += delta;

            if (LastHitTime > Y7B_BTL_AI_SPM_TIME)
            {
                RecentHits = 0;
                RecentHitsWithoutDefensiveMove = 0;
                RecentDefensiveAttacks = 0;
                RecentHitsWithoutAttack = 0;
            }
            
            //Not working as intended for some reason, disappointing.
            //if (IsAttacking() && RecentHitsWithoutAttack > 0)
                //RecentHitsWithoutAttack = 0;
        }

        public virtual void CombatUpdate()
        {
            Vector3 playerPos = (Vector3)BrawlerBattleManager.Kasuga.Character.Transform.Position;
            DistanceToPlayer = Vector3.Distance((Vector3)Chara.Get().Transform.Position, playerPos);

            BlockModule.Update();
            EvasionModule.Update();
            SyncHActModule.Update();
            JuggleModule.Update();

            if (m_getupHyperArmorDoOnce && !m_gettingUp)
            {
                m_getupHyperArmorDoOnce = false;
                Character.GetStatus().SetSuperArmor(false);
            }

            if (m_gettingUp && !m_getupHyperArmorDoOnce)
            {
                m_getupHyperArmorDoOnce = true;
                Character.GetStatus().SetSuperArmor(true);

                OnStartGettingUp();
            }

            SyncProcedure();
        }

        private void SyncProcedure()
        {
            if(AttackSimulator.PlayerInstance.Attacking() && 
                AttackSimulator.PlayerInstance.CurrentAttack.AttackType == AttackType.MoveSync &&
                (AttackSimulator.PlayerInstance.LastEnemyHitCurrentCombo == Character || BrawlerBattleManager.Kasuga.GetSyncPair() == Chara))
            {

                m_lastTimeSinceGrabbed = 0;

                AttackSync playerSync = AttackSimulator.PlayerInstance.CurrentAttack as AttackSync;

                if (playerSync.SyncType == 0)
                    m_grabbedTime += DragonEngine.deltaTime;


                if (m_grabbedTime >= Y7B_BTL_AI_GRAB_PATIENCE_TIME)
                    ResponseFlags[EnemyAIResponse.GrabShakeoff] = true;
            }
            else
            {
                m_grabbedTime = 0;
                m_lastTimeSinceGrabbed += DragonEngine.deltaTime;
                ResponseFlags[EnemyAIResponse.GrabShakeoff] = false;
            }
        }

        /// <summary>
        /// Returns true: Attack has been "processed" (aka it wont deal dmg)
        /// </summary>
        public unsafe virtual bool DamageTransit(BattleDamageInfoSafe dmg)
        {
            if (CanCounter() && DamageTransitCounter(dmg))
            {
                if (m_allowCounterEffectDoOnce)
                {
                    Chara.Get().Components.EffectEvent.Get().PlayEvent((EffectEventCharaID)206);
                    SoundManager.PlayCue(SoundCuesheetID.battle_common, 5, 0);
                }
                
                m_allowCounterEffectDoOnce = false;

                return true;
            }

            if(!BlockModule.BlockProcedure)
            {
                bool startedBlocking = DamageTransitGuard(dmg);

                if (!startedBlocking)
                {
                    RecentHitsWithoutDefensiveMove++;
                    RecentHitsWithoutAttack++;
                }
                else
                {
                    BlockModule.BlockProcedure = true;
                    Vector2 blockRange = GetBlockRange();
                    BlockModule.GuaranteedBlocks = new Random().Next((int)blockRange.x, (int)blockRange.y);

                    DragonEngine.Log("Start of block procedure");
                }

            }

            //Started guarding/guarding
            if (BlockModule.BlockProcedure)
            {
                if (Info.IsSync || Info.IsDown)
                    return false;

                AttackFlags brawlerSpecialProperty = (AttackFlags)(*((uint*)(dmg._ptr.ToInt64() + 0xE8)));


                if(BlockModule.GuaranteedBlocks <= 0 || brawlerSpecialProperty.HasFlag(AttackFlags.GuardBreak))
                    OnGuardBroke();
                else
                    OnBlocked();
            }

            return false;
        }


        /// <summary>
        /// Called on Damage Exec Valid, determines if the character should start blocking
        /// <br></br>
        /// Will start guard procedure.
        /// </summary>
        /// <returns></returns>
        public virtual bool DamageTransitGuard(BattleDamageInfoSafe dmg)
        {
            if (BlockModule.ShouldBlockAttack(dmg))
                return true;
            return false;
        }

        /// <summary>
        /// Return true: Attack cancelled out by a counter attack!
        /// </summary>
        /// <param name="dmg"></param>
        /// <returns></returns>
        public virtual bool DamageTransitCounter(BattleDamageInfoSafe dmg)
        {
            if (!CanDoCounterAttack())
                return false;

            //MANUAL COUNTER ATTACK, BASED ON LIST
            if (ShouldDoCounterAttack())
            {
                if(CounterAttacks.Count > 0)
                    EvasionModule.DoCounterAttack();

                return true;
            }

            return false;
        }


        /// <summary>
        /// Called on JustGuard.ValidEvent to determine if the enemy will block.
        /// </summary>
        /// <returns></returns>
        public bool GuardProcedure()
        {
            return BlockModule.BlockProcedure;
        }

        public virtual void HActProcedure()
        {

        }

        public void TurnUpdate(BattleTurnManager.ActionStep phase)
        {
            CheckChange();
        }


        public bool CanCounter()
        {
            if (Character.IsDown() || Character.IsDead() || !Character.IsCanStandAction())
                return false;

            return true;
        }

        public virtual bool CanDodge()
        {
            return true;
        }

        public virtual bool AllowDamage()
        {
            return TutorialManager.AllowEnemyDamage();
        }

        public virtual bool CanDoCounterAttack()
        {
            if (CounterAttacks.Count <= 0)
                return false;

            return true;
        }

        public virtual bool ShouldDoCounterAttack()
        {
            if (RecentHitsWithoutDefensiveMove >= 4 || RecentDefensiveAttacks > 3 || RecentHitsWithoutAttack > 5)
                return true;

            return false;
        }

        public void ExecuteCounterAttack(RPGSkillID id, bool showEffect)
        {
            m_allowCounterEffectDoOnce = showEffect;
            BattleTurnManager.ForceCounterCommand(Character, BrawlerBattleManager.Kasuga, id);

            OnCounterAttack?.Invoke();
        }

        public bool IsBeingJuggled()
        {
            MotionID gmtID = Chara.Get().GetMotion().GmtID;

            return gmtID.ToString().Contains("dwn_uch") || gmtID.ToString().Contains("dwn_bnd") ||
                gmtID == (MotionID)17100;
        }
        public virtual bool IsAttacking()
        {
            return BrawlerBattleManager.CurrentAttacker.Character.UID == Chara.UID &&
                BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Action &&
                BattleTurnManager.CurrentActionStep == BattleTurnManager.ActionStep.Action && Chara.Get().HumanModeManager.IsAttack();
        }

        //Check if we are too far. If we are, give turn to nearest enemy instead.
        private void CheckChange()
        {

            if (BrawlerBattleManager.Enemies.Length <= 1)
                return;

            //We are already the closest (this array is auto filtered by BrawlerBattleManager based on distance)
            if (BrawlerBattleManager.Enemies[0].Character.UID == Chara.UID)
                return;

            if(DistanceToPlayer > 13 || Info.IsDown)
            {
                EnemyManager._OverrideNextAttackerOnce = BrawlerBattleManager.Enemies[0];
                BattleTurnManager.ChangeActionStep(BattleTurnManager.ActionStep.ActionEnd);
            }
        }


        public virtual void OnBlocked()
        {
            BlockModule.OnBlocked();
        }

        public virtual void OnGuardBroke()
        {
           // Chara.Get().GetMotion().RequestGMT((MotionID)5542);
            SoundManager.PlayCue(SoundCuesheetID.battle_common, 12, 0);

            //Processed by SetMotionID guardbreak reaction
            Flags.ShouldGuardBreakFlag = true;

            BlockModule.BlockProcedure = false;
            BlockModule.GuaranteedBlocks = 0;

            if (!IsBoss())
                BlockModule.BlockPenalty = 2.5f;
        }


        protected virtual void OnStartGettingUp()
        {

        }

        public void Sway()
        {
            Chara.Get().HumanModeManager.ToSway();
        }

        public void DoGrabHActSync(EnemyMoveSync sync)
        {

        }

        /// <summary>
        /// Player is about to die, nothing can save him. The boss could play a hact where they finish off Ichiban.
        /// </summary>
        public virtual bool DoFinisher(BattleDamageInfoSafe dmgInf)
        {
            return false;
        }

        public virtual bool DoSpecial(BattleDamageInfoSafe inf)
        {
            return false;
        }

        public virtual bool IsBoss()
        {
            return false;
        }

        //Returns the amount of damage that the AI has agreed to take
        //This is important to reduce heat action damage when its spammed.
        public virtual long ProcessHActDamage(TalkParamID hact, long dmg)
        {
            return dmg;
        }

        //Counts for dodges and counter attacks too
        //Maybe dont?
        public virtual void OnHit()
        {
            LastHitTime = 0;
            RecentHits++;
        }

        /// <summary>
        /// Player is doing non-stop combos 0 IQ
        /// </summary>
        public bool IsBeingSpammed()
        {
            return RecentHits >= Y7B_BTL_AI_SPM_COUNT && LastHitTime <= Y7B_BTL_AI_SPM_TIME;
        }

        //Action, Action
        public virtual void OnStartAttack()
        {
     
        }

        public virtual void OnPlayerGettingUp()
        {

        }

        public virtual void OnPlayerDown()
        {

        }

        public virtual void OnJuggleHit(Vector4 hitPos)
        {
           JuggleModule.OnJuggleHit(hitPos);
        }

        public void DoHAct(TalkParamID hact, Vector4 position, params Fighter[] allies)
        {
            HActRequestOptions opts = new HActRequestOptions();
            opts.base_mtx.matrix = Chara.Get().GetMatrix();

            if (position != Vector4.zero)
                opts.base_mtx.matrix.m_vm3 = position;

            opts.id = hact;
            opts.is_force_play = true;

            opts.Register(HActReplaceID.hu_enemy_00, Chara.UID);
            opts.Register(HActReplaceID.hu_player, BrawlerBattleManager.KasugaChara.UID);

            opts.RegisterWeapon(AuthAssetReplaceID.we_enemy_00_r, Character.GetWeapon(AttachmentCombinationID.right_weapon));
            opts.RegisterWeapon(AuthAssetReplaceID.we_enemy_00_l, Character.GetWeapon(AttachmentCombinationID.left_weapon));
            opts.RegisterWeapon(AuthAssetReplaceID.we_player_r, BrawlerBattleManager.Kasuga.GetWeapon(AttachmentCombinationID.right_weapon));
            opts.RegisterWeapon(AuthAssetReplaceID.we_player_l, BrawlerBattleManager.Kasuga.GetWeapon(AttachmentCombinationID.left_weapon));

            int curReplace = (int)HActReplaceID.hu_enemy_01;

            foreach (Fighter fighter in allies)
            {
                if (fighter.IsValid())
                    opts.Register((HActReplaceID)curReplace, fighter.Character.UID);

                curReplace++;
            }

            m_performedHacts.Add(hact);
            BattleTurnManager.RequestHActEvent(opts);
        }

        public void DoHAct(TalkParamID hact, Matrix4x4 matrix, params Fighter[] allies)
        {
            HActRequestOptions opts = new HActRequestOptions();
            opts.base_mtx.matrix = matrix;

            opts.id = hact;
            opts.is_force_play = true;

            opts.Register(HActReplaceID.hu_enemy_00, Chara.UID);
            opts.Register(HActReplaceID.hu_player, BrawlerBattleManager.KasugaChara.UID);

            opts.RegisterWeapon(AuthAssetReplaceID.we_enemy_00_r, Character.GetWeapon(AttachmentCombinationID.right_weapon));
            opts.RegisterWeapon(AuthAssetReplaceID.we_enemy_00_l, Character.GetWeapon(AttachmentCombinationID.left_weapon));
            opts.RegisterWeapon(AuthAssetReplaceID.we_player_r, BrawlerBattleManager.Kasuga.GetWeapon(AttachmentCombinationID.right_weapon));
            opts.RegisterWeapon(AuthAssetReplaceID.we_player_l, BrawlerBattleManager.Kasuga.GetWeapon(AttachmentCombinationID.left_weapon));

            int curReplace = (int)HActReplaceID.hu_enemy_01;

            foreach (Fighter fighter in allies)
            {
                if (fighter.IsValid())
                    opts.Register((HActReplaceID)curReplace, fighter.Character.UID);

                curReplace++;
            }

            m_performedHacts.Add(hact);
            BrawlerBattleManager.CurrentHActIsY7B = true;
            BattleTurnManager.RequestHActEvent(opts);
        }

        public virtual Vector2 GetBlockRange()
        {
            return new Vector2(3, 6);
        }

        public virtual bool CanDieOnHAct()
        {
            return true;
        }
    }
}

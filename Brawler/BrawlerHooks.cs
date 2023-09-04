using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DragonEngineLibrary;
using MinHook.NET;
using YazawaCommand;

namespace Brawler
{
    public static class BrawlerHooks
    {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
            private delegate void BattleTurnManagerRequestWarpFighter(IntPtr mng, IntPtr fighter, IntPtr inf, IntPtr res);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void BattleTurnManagerDmgNotify(IntPtr mng, IntPtr inf);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void BattleTurnManagerRequestShowMiss(IntPtr mng, IntPtr fighter);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate bool BattleTurnManagerExecPhaseCleanup(IntPtr mngr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate bool HumanModeManagerIsInputKamae(IntPtr mng);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate bool HumanModeManagerTransitDamage(IntPtr mng, void* battleDamageInf);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate bool HumanModeManagerDamageExecValid(IntPtr mng, void* battleDamageInf);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return:MarshalAs(UnmanagedType.U1)]
        private unsafe delegate bool HumanModeManagerTransitDamageCounter(IntPtr thisPtr, IntPtr args);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void EffectEventPlay(IntPtr effect, EffectEventCharaID id);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate bool FighterTP(IntPtr character, IntPtr tpinf);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate bool ECRenderCharacterBattleTransformOn(IntPtr character);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void DamageNodeProcHook(IntPtr node, IntPtr damageInf, IntPtr fighter);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate float UICalcHealthGaugeWidth(float hp_max, float width_min, float width_max, float fit_range_min, float fit_range_max);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate bool HijackedGuardFunc(IntPtr fighter);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void BattleTurnManagerChangePhase(IntPtr mng, BattleTurnManager.TurnPhase phase);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate void BattleTurnManagerChangeActionStep(IntPtr mng, BattleTurnManager.ActionStep phase);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate void TimingDropAssetPlay(IntPtr node, uint tick, IntPtr matrix, uint* parentHandle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate void GuardReactionSetMotionID(IntPtr guardReactionOb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate void BattleStartActionRequestMotion(IntPtr fighterMode, uint gmtID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate bool BattleStartManagerIsTransformDelay(IntPtr battleStartMng);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate void FighterModeHumanDamageRequestMotion(IntPtr fighterModeDamage, MotionID id);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate bool FighterModeHumanDamageIsDown(IntPtr fighterModeDamage);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate bool HumanModeManagerTransitCounter(IntPtr humanMode, IntPtr battleDamageInfo);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return:MarshalAs(UnmanagedType.U1)]
        private unsafe delegate bool HumanModeManagerTransitExecPickup(IntPtr humanMode);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate bool CameraFreeIsRotateBehind(IntPtr cam);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate uint GetCurrentInputState(IntPtr ent);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return:MarshalAs(UnmanagedType.U1)]
        private unsafe delegate bool PauseManagerIsPauseButtonPressed(IntPtr pauseManager, int mask, int slot);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        private unsafe delegate bool CharacterRequestStartFighter(IntPtr character);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate IntPtr ParticleManagerPlay(IntPtr manager, IntPtr result, uint particleID, IntPtr mtx, uint type);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe delegate void PauseManagerRequestPause1(IntPtr pauseManager, uint pauseID, ulong param, uint focus, uint slot);

        public unsafe static void Init()
        {
            _inputStateDeleg = new GetCurrentInputState(GetInputState);
            _kamaeDeleg = new HumanModeManagerIsInputKamae(HumanModeManager_IsInputKamae);
            _swayDeleg = new HumanModeManagerIsInputKamae(HumanModeManager_IsInputSway);
            _pickupDeleg = new HumanModeManagerIsInputKamae(HumanModeManager_IsInputPickup);
            _btlTurnManagerDmgNotifyDeleg = new BattleTurnManagerDmgNotify(BattleTurnManager_OnAfterDamage);
            _btlTurnManagerRequestShowMissDeleg = new BattleTurnManagerRequestShowMiss(BattleTurnManager_RequestShowMiss);
            _btlTurnManagerRequestWarpFighterDeleg = new BattleTurnManagerRequestWarpFighter(BattleTurnManager_RequestWarpFighter);
            _btlTurnManagerChangeActionStepDeleg = new BattleTurnManagerChangeActionStep(BattleTurnManager_ChangeActionStep);
            _ecRenderCharacterBattleTransformOnDeleg = new ECRenderCharacterBattleTransformOn(ECRenderCharacter_BattleTransformOn);
            _calcHpDeleg = new UICalcHealthGaugeWidth(CalcHealthGaugeWidth);
            _btlStartReqMotDeleg = new BattleStartActionRequestMotion(BattleStartAction_RequestMotion);
            _btlStartManagerIsDelayTransformDeleg = new BattleStartManagerIsTransformDelay(BattleStartManager_IsDelayTransform);
            _fighterModeDamageRequestMotionDeleg = new FighterModeHumanDamageRequestMotion(FighterModeHumanDamage_RequestMotion);
            _humanModeTransitDamageDeleg = new HumanModeManagerTransitDamage(HumanModeManager_TransitDamage);
            _fighterModeHumanDamageIsDownDeleg = new FighterModeHumanDamageIsDown(FighterModeHumanDamage_IsDown);
            _humanModeTransitDmgCounterDeleg = new HumanModeManagerTransitDamageCounter(HumanModeManager_TransitCounterDamage);
            _humanModeTransitDmgSwayDeleg = new HumanModeManagerTransitDamageCounter(HumanModeManager_TransitSwayDamage);
            _damageExecValidDeleg = new HumanModeManagerDamageExecValid(HumanModeManager_DamageExecValid);
            _cameraFreeIsRotateBehindDeleg = new CameraFreeIsRotateBehind(CameraFree_IsRotateBehind);
            _ptcManPlayDeleg = new ParticleManagerPlay(ParticleManager_Play);

            unsafe
            {
                _justCounterValidEventDeleg = new HumanModeManagerTransitDamageCounter(JustGuard_ValidEvent);
                _dropPlayDeleg = new TimingDropAssetPlay(TimingDropAsset_Play);
                _guardReactionIDDeleg = new GuardReactionSetMotionID(GuardReaction_SetMotionID);
            }

            _npcGuardDeleg = new HijackedGuardFunc(HijackedGuardFunct);
            _pauseBtnDeleg = new PauseManagerIsPauseButtonPressed(PauseManager_IsPauseButtonPressed);
            _pauseReq1Deleg = new PauseManagerRequestPause1(PauseManager_RequestPause1);
            _charaReqStartFighterDeleg = new CharacterRequestStartFighter(Character_Request_Start_Fighter);

            //DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x14094E919, 5);

            try
            {
                MinHookHelper.initialize();
            }
            catch { }

            MinHookHelper.createHook((IntPtr)0x1406D7BE0, _kamaeDeleg, out _kamaeTrampoline);
           // MinHookHelper.createHook((IntPtr)0x1406D7B90, _swayDeleg, out _swayTrampoline);
            MinHookHelper.createHook((IntPtr)0x1406D8A50, _pickupDeleg, out _pickupTrampoline);
            MinHookHelper.createHook((IntPtr)0x1404D2B10, _btlTurnManagerDmgNotifyDeleg, out _btlTurnManagerDmgNotifyTrampoline);
            MinHookHelper.createHook((IntPtr)0x140FF0ED0, _btlTurnManagerRequestShowMissDeleg, out _btlTurnManagerDmgRequestShowMissTrampoline);
            MinHookHelper.createHook((IntPtr)0x1404DDEF0, _btlTurnManagerRequestWarpFighterDeleg, out _btlTurnManagerRequestWarpFighterTrampoline);
           
            MinHookHelper.createHook((IntPtr)0x1404C9850, _btlTurnManagerChangeActionStepDeleg, out _btlTurnManagerChangeActionStepTrampoline);
            MinHookHelper.createHook((IntPtr)0x1407E2B20, _ecRenderCharacterBattleTransformOnDeleg, out _ecRenderCharacterBattleTransformOnTrampoline);
            MinHookHelper.createHook((IntPtr)0x140944EE0, _justCounterValidEventDeleg, out _justCounterValidEventTrampoline);
            MinHookHelper.createHook((IntPtr)0x157A9E640, _dropPlayDeleg, out _dropPlayTrampoline);
            //    MinHookHelper.createHook((IntPtr)0x1406EB940, _btlStartReqMotDeleg, out _btlStartReqMotTrampoline);
            MinHookHelper.createHook((IntPtr)0x1409AB480, _btlStartManagerIsDelayTransformDeleg, out _btlStartManagerIsDelayTransformTrampoline);
            MinHookHelper.createHook((IntPtr)0x1406E1490, _fighterModeDamageRequestMotionDeleg, out _fighterModeDamageRequestMotionTrampoline);
            MinHookHelper.createHook((IntPtr)0x1406DE9F0, _humanModeTransitDamageDeleg, out _humanModeTransitDamageTrampoline);
            MinHookHelper.createHook((IntPtr)0x1406E24C0, _fighterModeHumanDamageIsDownDeleg, out _fighterModeHumanDamageIsDownTrampoline);
            MinHookHelper.createHook((IntPtr)0x1406F22D0, _humanModeTransitDmgCounterDeleg, out _humanModeTransitDmgCounterTrampoline);
            MinHookHelper.createHook((IntPtr)0x1406F2890, _humanModeTransitDmgSwayDeleg, out _humanModeTransitDmgSwayTrampoline);
            MinHookHelper.createHook((IntPtr)0x1406DE870, _damageExecValidDeleg, out _damageExecValidTrampoline);
         //   MinHookHelper.createHook((IntPtr)0x1404D5920, _execPhaseCleanupDeleg, out _execPhaseCleanupTrampoline);
            MinHookHelper.createHook((IntPtr)0x140795190, _cameraFreeIsRotateBehindDeleg, out _cameraFreeIsRotateBehindTrampoline);

            MinHookHelper.createHook((IntPtr)0x140527390, _charaReqStartFighterDeleg, out _charaReqStartFighterTrampoline);

            CameraHooks.Hook();
            BattleStartManager.Hook();
            BrawlerBattleTransition.Init();
            HActDamage.Hook();
            HumanModeManagerHook.Hook();
            FighterModeHook.Hook();
            UIHooks.Hook();
            AuthHooks.Hook();
            BattleMusic.Hook();

            //SYSTEM: Allow us to change the speed of "Unprocessed" (observed in hacts)
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x141ED0B80, 5);

            //HACT MANAGER: Force the game to allow more than one hact at once
            //DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x1405E0CD4, new byte[] {0x90, 0x90, 0xEB, 0x30});

            //CAMERA FREE: DISABLE CAMERA TWERKING ON BATTLE/CAMERA LINK OUT
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x141F4461E, 5);

            //COMBAT INPUT (KEYBOARD): Ensure we are always on input state 0x10 to allow movement with keyboard.
            MinHookHelper.createHook((IntPtr)0x141F48690, _inputStateDeleg, out _inputStateTrampoline);

            //COMBAT AI: Force enemies to leftover Judgement AI (Zako for now, TODO: switch bosses to boss AI)
            // DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x140A400A0, new byte[] { 0x48, 0x8D, 0x15, 0x8C, 0xC6, 0x7B, 0x01 });

            //COMBAT: Prevent non-functional stun escape prompt from stuttering the game
            //on the extremely rare chance that we get Y6 wallbound/JE stunned
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x1406F4AAB, 5);

            //COMBAT: Remove pausing block
            //TODO: TURN THIS INTO A HOOK FOR FINER CONTROL!
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x1405C5210, new byte[] { 0x31, 0xC0, 0xC3, 0x90 });

            //COMBAT: Force RPG pause menu instead of generic
            MinHookHelper.createHook((IntPtr)0x140CBE5B0, _pauseReq1Deleg, out _pauseReq1Trampoline);

            //COMBAT: Allow keyboard to be able to pause
            MinHookHelper.createHook((IntPtr)0x14170E090, _pauseBtnDeleg, out _pauseBtnTrampoline);

            //COMBAT: Prevent code that forces the enemy to get up when its their turn
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x1404D9382, 6);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x1404CB7A6, new byte[] {0x90, 0x90, 0xEB});

            //COMBAT (RANGE): Disable filtering for cec_hact
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x14062500D, 2);
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x140625076, 6);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x140624F7A, new byte[] { 0xE9, 0x87, 0x0, 0x0, 0x0, 0x90 });
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x140624F73, 7);

            //COMBAT: Prevent Ishioda from getting stunned when he is thrown from the crane truck.
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x14025E47E, 5);

            //COMBAT: Prevent the game from re-applying stun effects on Ishioda
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x14024FBE0, 5);

            //COMBAT: Allow Ishioda to attack instead of forcing "defense command" on him which prevents him from doing so.
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x14025ABC0, new byte[] {0xB8, 0x0, 0x0, 0x0, 0x0, 0xC3});

            //COMBAT: Ensuring smoother combat transitions by removing the battle end fade.
            //DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x1404C84C9, 5);

            //JUDGMENT UI GAUGE: Replace the Judgment check with Yazawa (YLAD)
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x1410E4C8C, new byte[] { 0x7 });

            //JUDGMENT UI GAUGE: Create fake function at the address because MinHook believes it isnt a valid one.
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x1411F45C0, new byte[] { 0xB0, 0x01, 0xC3 });
            MinHookHelper.createHook((IntPtr)0x1411F45C0, _calcHpDeleg, out _calcHpTrampoline);

            //GUARDING: Swap condition, player will have its humanmode checked. Enemies will use a hijacked function
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x157B1E062, 0x75);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x157B1E07B, 0xE8, 0x40, 0x00, 0x46, 0xE9); //call hijacked func

            //CRANE TRUCK: prevent teleportation
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x140256BCF, 5);
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x140257156, 5);

            //HUMANMODEMANAGER: Disable hardcoded any hardcoded counters. Our AI will take care of it.
            //    DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x1406F22D0, new byte[] { 0xB0, 0x00, 0xC3 });

            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x157B1E080, 7);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x157B1E087, 0x84, 0xC0, 0x90);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x157B1E08C, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x74); ;
            MinHookHelper.createHook((IntPtr)0x140F7E0C0, _npcGuardDeleg, out _npcGuardTrampoline);
            MinHookHelper.createHook((IntPtr)0x1406F2E90, _guardReactionIDDeleg, out _guardReactionIDTrampoline);

            //COMBAT: Remove teleportation of player after combat ends
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x1404CD190, 0xC3);

            //COMBAT: Force shift input on swaying (aka face enemy while quickstepping) (includes enemies)
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x1406F7690, 0xB0, 0x01, 0xC3);

            //COMBAT: Disable perfect guard
            DragonEngineLibrary.Unsafe.CPP.NopMemory((IntPtr)0x140944E8E, 2);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory((IntPtr)0x140944E90, 0xEB);

            //COMBAT EFFECT: Use our own custom pibs
            MinHookHelper.createHook((IntPtr)0x141613190, _ptcManPlayDeleg, out _ptcManPlayTrampoline);


            MinHookHelper.enableAllHook();
        }

        private static CharacterRequestStartFighter _charaReqStartFighterDeleg;
        private static CharacterRequestStartFighter _charaReqStartFighterTrampoline;
        private static bool Character_Request_Start_Fighter(IntPtr charaPtr)
        {
            Character chara = new Character() { Pointer = charaPtr};
            CharacterAttributes attribs = chara.Attributes;

            if (!attribs.is_supporter)
            {
                return _charaReqStartFighterTrampoline(charaPtr);
            }

            return false;        
        }


        private static PauseManagerIsPauseButtonPressed _pauseBtnDeleg;
        private static PauseManagerIsPauseButtonPressed _pauseBtnTrampoline;
        private static bool PauseManager_IsPauseButtonPressed(IntPtr mng, int mask, int slot)
        {
            //Esc key will not work for some reason on keyboard
            //Requiring us to do this.

            if (!GameVarManager.GetValueBool(GameVarID.is_battle))
                return _pauseBtnTrampoline(mng, mask, slot);
            else
            {
                if (InputInterface.IsKeyboardActive())
                    return ModInput.KeyboardPausePressed;
                else
                    return _pauseBtnTrampoline(mng, mask, slot);
            }
        }

        private static PauseManagerRequestPause1 _pauseReq1Deleg;
        private static PauseManagerRequestPause1 _pauseReq1Trampoline;
        private static void PauseManager_RequestPause1(IntPtr mng, uint pauseID, ulong param, uint focus, uint slot)
        {
            if (GameVarManager.GetValueBool(GameVarID.is_battle))
                if (pauseID == 0x59)
                    pauseID = 0x8;

            _pauseReq1Trampoline(mng, pauseID, param, focus, slot);
;        }


        private static GetCurrentInputState _inputStateDeleg;
        private static GetCurrentInputState _inputStateTrampoline;
        private static uint GetInputState(IntPtr obj)
        {
            return _inputStateTrampoline(obj);
        }

        private static GuardReactionSetMotionID _guardReactionIDDeleg;
        private static GuardReactionSetMotionID _guardReactionIDTrampoline;
        private unsafe static void GuardReaction_SetMotionID(IntPtr guardReactionObj)
        {
            //GuardReaction provides incomplete data for its damage info.
            //Solution? We set flags before and check them here

            long* mngPtr = (long*)(guardReactionObj.ToInt64() + 0x28);
            uint* motionID = (uint*)(guardReactionObj.ToInt64() + 0x78);

            HumanModeManager manager = new HumanModeManager() { Pointer = (IntPtr)(*mngPtr) };
            Fighter fighter = manager.Human.GetFighter();
            EnemyAI ai = EnemyManager.GetAI(fighter);


            if (!fighter.IsValid() || !fighter.IsEnemy() || ai == null)
            {
                _guardReactionIDTrampoline(guardReactionObj);
                return;
            }

            if (ai.Flags.ShouldGuardBreakFlag)
            {
                Console.WriteLine("GUARD BREAK!");
                *motionID = 5542;
                ai.Flags.ShouldGuardBreakFlag = false;
            }
            else
            {
                //TODO: Fix AI code. Add boss traits to goons but never activate them
                if (ai.IsBoss())
                {
                    if ((ai as EnemyAIBoss).BlockModule.OnBlockedAnimEvent())
                        return;
                }

                _guardReactionIDTrampoline(guardReactionObj);
            }
        }

        private static HijackedGuardFunc _npcGuardDeleg;
        private static HijackedGuardFunc _npcGuardTrampoline;
        private static bool HijackedGuardFunct(IntPtr fighterPtr)
        {
            return true;
        }

        private static UICalcHealthGaugeWidth _calcHpDeleg;
        private static UICalcHealthGaugeWidth _calcHpTrampoline;
        private static float CalcHealthGaugeWidth(float hp_max, float width_min, float width_max, float fit_range_min, float fit_range_max)
        {
            if (hp_max > width_max)
                return width_max;

            if (hp_max < width_min)
                return width_min;

            return hp_max;
        }

        //This hook will be utilized for attack counters
        //This hook will be utilized for attack counters/special damage events.
        private static HumanModeManagerDamageExecValid _damageExecValidDeleg;
        private static HumanModeManagerDamageExecValid _damageExecValidTrampoline;
        private unsafe static bool HumanModeManager_DamageExecValid(IntPtr humanModePtr, void* damageInfo)
        {
            if (BrawlerBattleManager.HActIsPlaying)
                return _damageExecValidTrampoline(humanModePtr, damageInfo);

            BattleDamageInfoSafe safeDmg = new BattleDamageInfoSafe((IntPtr)damageInfo);
            Fighter attacker = safeDmg.Attacker.Get().GetFighter();
            Fighter victim = new HumanModeManager() { Pointer = humanModePtr }.Human.GetFighter();

            //Enemy on hit procedure
            if (!victim.IsPlayer())
            {
                EnemyAI ai = EnemyManager.GetAI(victim);

                if (ai != null)
                {
                    if (!ai.AllowDamage())
                        safeDmg.Damage = 0;

                    bool attackCountered = ai.DamageTransit(safeDmg);

                    //we have intercepted/countered the attack.
                    if (attackCountered)
                    {
                        ai.BlockModule.BlockProcedure = false;
                        return true;
                    }

                    bool handled = _damageExecValidTrampoline(humanModePtr, damageInfo);

                    if (handled)
                        if (safeDmg.Attacker.UID == BrawlerBattleManager.KasugaChara.UID)
                            AttackSimulator.PlayerInstance.OnAttackLanded(victim, safeDmg);

                    return handled;
                }
                else
                    return _damageExecValidTrampoline(humanModePtr, damageInfo);

            }

            if (!BrawlerPlayer.AllowDamage((*(BattleDamageInfo*)damageInfo)))
                safeDmg.Damage = 0;

            if (BrawlerPlayer.DamageTransit(safeDmg))
                return true;


            BrawlerPlayer.OnGetHit(safeDmg.Attacker.Get().GetFighter(), safeDmg);

            return _damageExecValidTrampoline(humanModePtr, damageInfo);
        }

        //This hook will be utilized for attack counters
        private static HumanModeManagerTransitDamageCounter _justCounterValidEventDeleg;
        private static HumanModeManagerTransitDamageCounter _justCounterValidEventTrampoline;
        private unsafe static bool JustGuard_ValidEvent(IntPtr thisPtr, IntPtr args)
        {
            Fighter victim = new Fighter((IntPtr)(((CalcDamageEventArgs*)args)->damage_fighter_));

            CalcDamageEventArgs* argsPtr = (CalcDamageEventArgs*)args;
            BattleDamageInfo* damageInf = argsPtr->info;
            BattleDamageInfoSafe safeDmg = new BattleDamageInfoSafe((IntPtr)damageInf);

            if (victim.IsPlayer())
                return true;

            EnemyAI ai = EnemyManager.GetAI(victim);

            if (ai == null)
                return false;

            return ai.GuardProcedure();
        }

        private static HumanModeManagerIsInputKamae _kamaeDeleg;
        private static HumanModeManagerIsInputKamae _kamaeTrampoline;
        public static bool HumanModeManager_IsInputKamae(IntPtr HumanModeManager)
        {
            
            if (!BrawlerBattleManager.Kasuga.IsValid())
                return false;

            if (!BrawlerPlayer.GenericShouldExecuteAttack())
                return false;

            if (HumanModeManager == BrawlerBattleManager.KasugaChara.HumanModeManager.Pointer)
            {
                MotionID gmt = BrawlerBattleManager.KasugaChara.GetMotion().GmtID;

                if (BrawlerPlayer.Info.IsDown ||
                    BrawlerPlayer.FreezeInput ||
                    gmt > 0)

                    return false;

                if (ModInput.Holding(AttackInputID.LockOn))
                    return true;
            }

            return false;
           
        }

        private static HumanModeManagerIsInputKamae _swayDeleg;
        private static HumanModeManagerIsInputKamae _swayTrampoline;
        public static bool HumanModeManager_IsInputSway(IntPtr HumanModeManager)
        {
            //Sway moves are handled by YFC
            return false;
            // if (!BrawlerPlayer.GenericShouldExecuteAttack())
            //   return false;

            if (HumanModeManager == BrawlerBattleManager.KasugaChara.HumanModeManager.Pointer)
            { }

            return _swayTrampoline(HumanModeManager);
        }


        private static HumanModeManagerIsInputKamae _pickupDeleg;
        private static HumanModeManagerIsInputKamae _pickupTrampoline;
        public static bool HumanModeManager_IsInputPickup(IntPtr HumanModeManager)
        {
            if (!Mod.ShouldExecBrawlerInput())
                return false;

            if (BrawlerPlayer.IsEXGamer)
                return false;

            if (BrawlerPlayer.Info.IsDown ||
                BrawlerPlayer.FreezeInput)

                return false;

            if (HumanModeManager == BrawlerBattleManager.KasugaChara.HumanModeManager.Pointer)
                return ModInput.JustPressed(AttackInputID.Grab);
            else
                return _pickupTrampoline(HumanModeManager);
        }

        private static BattleTurnManagerDmgNotify _btlTurnManagerDmgNotifyDeleg;
        private static BattleTurnManagerDmgNotify _btlTurnManagerDmgNotifyTrampoline;
        private static void BattleTurnManager_OnAfterDamage(IntPtr mng, IntPtr inf)
        {
            FighterID id = Marshal.PtrToStructure<FighterID>(inf + 0x8);

            if (new EntityHandle<Character>(id.Handle).Get().Attributes.is_player)
            {
                if (!IniSettings.ShowPlayerDamage || !BrawlerPlayer.AllowDamage(new BattleDamageInfo()))
                    return;
            }
            else
                if (!IniSettings.ShowEnemyDamage)
                return;

            _btlTurnManagerDmgNotifyTrampoline(mng, inf);
        }

        private static BattleTurnManagerRequestShowMiss _btlTurnManagerRequestShowMissDeleg;
        private static BattleTurnManagerRequestShowMiss _btlTurnManagerDmgRequestShowMissTrampoline;
        private static void BattleTurnManager_RequestShowMiss(IntPtr mng, IntPtr fighter)
        {
            return;
        }


        private static BattleTurnManagerRequestWarpFighter _btlTurnManagerRequestWarpFighterDeleg;
        private static BattleTurnManagerRequestWarpFighter _btlTurnManagerRequestWarpFighterTrampoline;
        private static void BattleTurnManager_RequestWarpFighter(IntPtr mng, IntPtr fighter, IntPtr inf, IntPtr res)
        {
            return;
        }

        private static FighterTP _fighterTPDeleg;
        private static FighterTP _fighterTPTrampoline;
        private static bool Fighter_TP(IntPtr fighter, IntPtr tpInf)
        {
            return true;
        }

        private static ECRenderCharacterBattleTransformOn _ecRenderCharacterBattleTransformOnDeleg;
        private static ECRenderCharacterBattleTransformOn _ecRenderCharacterBattleTransformOnTrampoline;
        private static bool ECRenderCharacter_BattleTransformOn(IntPtr render)
        {
            ECRenderCharacter rend = new ECRenderCharacter() { Pointer = render };

            if (rend.Owner.UID == BrawlerBattleManager.KasugaChara.UID)
            {
                if (!BrawlerBattleManager.AllowPlayerTransformationDoOnce)
                    return true;
                else
                    BrawlerBattleManager.AllowPlayerTransformationDoOnce = false;
            }

            return _ecRenderCharacterBattleTransformOnTrampoline(render);
        }

        private static BattleTurnManagerChangeActionStep _btlTurnManagerChangeActionStepDeleg;
        private static BattleTurnManagerChangeActionStep _btlTurnManagerChangeActionStepTrampoline;
        private static void BattleTurnManager_ChangeActionStep(IntPtr mng, BattleTurnManager.ActionStep actionStep)
        {
            _btlTurnManagerChangeActionStepTrampoline(mng, actionStep);
        }

        private static TimingDropAssetPlay _dropPlayDeleg;
        private static TimingDropAssetPlay _dropPlayTrampoline;
        private unsafe static void TimingDropAsset_Play(IntPtr node, uint tick, IntPtr matrix, uint* parentHandle)
        {
            Character chara = new EntityHandle<Character>(*parentHandle).Get();
            chara.GetFighter().DropWeapon(new DropWeaponOption(AttachmentCombinationID.right_weapon, false));
        }

        private static BattleStartActionRequestMotion _btlStartReqMotDeleg;
        private static BattleStartActionRequestMotion _btlStartReqMotTrampoline;
        private unsafe static void BattleStartAction_RequestMotion(IntPtr fighterMode, uint gmtID)
        {
            //TEMP:

            gmtID = 0;
            return;
        }

        private static BattleStartManagerIsTransformDelay _btlStartManagerIsDelayTransformDeleg;
        private static BattleStartManagerIsTransformDelay _btlStartManagerIsDelayTransformTrampoline;
        private unsafe static bool BattleStartManager_IsDelayTransform(IntPtr startManager)
        {
            return false;
        }

        private static FighterModeHumanDamageRequestMotion _fighterModeDamageRequestMotionDeleg;
        private static FighterModeHumanDamageRequestMotion _fighterModeDamageRequestMotionTrampoline;
        private static unsafe void FighterModeHumanDamage_RequestMotion(IntPtr fighterMode, MotionID gmt)
        {
            long* mngPtr = (long*)(fighterMode.ToInt64() + 0x28);
            HumanModeManager manager = new HumanModeManager() { Pointer = (IntPtr)(*mngPtr) };

            EnemyAI ai = EnemyManager.GetAI(manager.Human.GetFighter());

            //Only enemies can be juggled
            if (ai == null || !ai.IsBeingJuggled())
            {
                _fighterModeDamageRequestMotionTrampoline(fighterMode, gmt);
                return;
            }

            IntPtr damageInfoPtr = (IntPtr)(fighterMode.ToInt64() + 0x90);
            Vector4* pos = (Vector4*)(damageInfoPtr + 0x40);

            Vector4 charaPos = manager.Human.Transform.Position;
            charaPos.y = pos->y + 0.15f;

            if (charaPos != Vector4.zero)
                ai.OnJuggleHit(charaPos);

            _fighterModeDamageRequestMotionTrampoline(fighterMode, (MotionID)17100);
            manager.Human.GetMotion().RequestGMT(17100);

        }

        private static HumanModeManagerTransitDamage _humanModeTransitDamageDeleg;
        private static HumanModeManagerTransitDamage _humanModeTransitDamageTrampoline;
        private static unsafe bool HumanModeManager_TransitDamage(IntPtr humanModeManagerPtr, void* battleDamageInfo)
        {
            //DE moment: i have to do this extra hook for juggies

            HumanModeManager manager = new HumanModeManager() { Pointer = humanModeManagerPtr };
            BattleDamageInfoSafe safeDmg = new BattleDamageInfoSafe((IntPtr)battleDamageInfo);
            EnemyAI enemyAI = EnemyManager.GetAI(manager.Human.GetFighter());

            bool result = _humanModeTransitDamageTrampoline(humanModeManagerPtr, battleDamageInfo);

            if (enemyAI == null)
                return result;
            else
            {
                //We must do this regardles of transmitting because if we dont these wont work for
                //hyperarmor enemies.
                enemyAI.OnHit();
                BrawlerPlayer.OnHitEnemy(enemyAI.Character, safeDmg);
            }

            if (!result)
                return result;

            if (enemyAI.IsBeingJuggled())
            {
                if (enemyAI.JuggleModule.JuggleCount <= 1)
                    return result;

                enemyAI.Chara.Get().GetMotion().RequestGMT(17100);
                enemyAI.JuggleModule.OnJuggleHit(*(Vector4*)((long)battleDamageInfo + 0x40));
            }

            return result;
        }

        private static FighterModeHumanDamageIsDown _fighterModeHumanDamageIsDownDeleg;
        private static FighterModeHumanDamageIsDown _fighterModeHumanDamageIsDownTrampoline;
        private static unsafe bool FighterModeHumanDamage_IsDown(IntPtr fighterMode)
        {
            long* mngPtr = (long*)(fighterMode.ToInt64() + 0x28);
            HumanModeManager manager = new HumanModeManager() { Pointer = (IntPtr)(*mngPtr) };

            EnemyAI ai = EnemyManager.GetAI(manager.Human.GetFighter());

            if (ai == null)
                return _fighterModeHumanDamageIsDownTrampoline(fighterMode);

            if (ai.IsBeingJuggled())
                return true;
            else
                return _fighterModeHumanDamageIsDownTrampoline(fighterMode);
        }


        private static HumanModeManagerTransitDamageCounter _humanModeTransitDmgCounterDeleg;
        private static HumanModeManagerTransitDamageCounter _humanModeTransitDmgCounterTrampoline;
        private static unsafe bool HumanModeManager_TransitCounterDamage(IntPtr humanModeManager, IntPtr damagePtr)
        {
            return false;
        }

        private static HumanModeManagerTransitDamageCounter _humanModeTransitDmgSwayDeleg;
        private static HumanModeManagerTransitDamageCounter _humanModeTransitDmgSwayTrampoline;
        private static unsafe bool HumanModeManager_TransitSwayDamage(IntPtr humanModeManager, IntPtr damagePtr)
        {
            bool orig = _humanModeTransitDmgSwayTrampoline(humanModeManager, damagePtr);

            if (orig)
                return true;

            Fighter fighter = new HumanModeManager() { Pointer = humanModeManager}.Human.GetFighter();


            if (!fighter.IsPlayer() && !fighter.IsDown() && !fighter.IsSync())
            {
                EnemyAI ai = EnemyManager.GetAI(fighter);

                if(ai != null)
                {
                    bool evasion = ai.CanDodge() && ai.EvasionModule.ShouldEvade(new BattleDamageInfoSafe(damagePtr));

                    if (evasion)
                    {
                        ai.Chara.Get().HumanModeManager.ToSway();
                        return true;
                    }
                }
            }

                return false;
        }

        private static CameraFreeIsRotateBehind _cameraFreeIsRotateBehindDeleg;
        private static CameraFreeIsRotateBehind _cameraFreeIsRotateBehindTrampoline;
        private static unsafe bool CameraFree_IsRotateBehind(IntPtr camera)
        {
            if (BrawlerBattleManager.Kasuga.IsValid())
                return false;

            return true;
        }


        private static ParticleManagerPlay _ptcManPlayDeleg;
        private static ParticleManagerPlay _ptcManPlayTrampoline;
        private static unsafe IntPtr ParticleManager_Play(IntPtr manager, IntPtr result, uint particleID, IntPtr mtx, uint type)
        {
            particleID = ParticleHook.DetermineID(particleID);
            return _ptcManPlayTrampoline(manager, result, particleID, mtx, type);
        }
    }
}

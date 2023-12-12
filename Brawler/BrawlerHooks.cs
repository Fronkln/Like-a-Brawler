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

            try
            {
                MinHookHelper.initialize();
            }
            catch { }

            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 83 EC ? 48 8B 05 ? ? ? ? 80 B8 58 02 00 00 ? 74 ? 48 83 C1 ?"), _kamaeDeleg, out _kamaeTrampoline);

            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 83 EC ? 48 83 C1 ? E8 ? ? ? ? 0F B6 80 AC 00 00 00"), _pickupDeleg, out _pickupTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("40 56 57 48 83 EC ? 48 8B 02"), _btlTurnManagerDmgNotifyDeleg, out _btlTurnManagerDmgNotifyTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 08 57 48 83 EC ? 48 8B F9 48 8B DA 48 8B CA E8 ? ? ? ? 84 C0 75 ? 4C 8B CB"), _btlTurnManagerRequestShowMissDeleg, out _btlTurnManagerDmgRequestShowMissTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 08 57 48 83 EC ? 48 8B 0D ? ? ? ? 49 8B D8"), _btlTurnManagerRequestWarpFighterDeleg, out _btlTurnManagerRequestWarpFighterTrampoline);

            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8B C4 55 41 54 41 55 41 56 41 57 48 8D A8 78 FE FF FF 48 81 EC ? ? ? ? 48 C7 45 30 ? ? ? ?"), _btlTurnManagerChangeActionStepDeleg, out _btlTurnManagerChangeActionStepTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.ReadCall(DragonEngineLibrary.Unsafe.CPP.PatternSearch("E8 ? ? ? ? 8B 43 14 89 45 A7")), _ecRenderCharacterBattleTransformOnDeleg, out _ecRenderCharacterBattleTransformOnTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 08 57 48 83 EC ? 48 8B 4A 08 48 8B DA"), _justCounterValidEventDeleg, out _justCounterValidEventTrampoline);


            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC ? 83 79 50 ?"), _btlStartManagerIsDelayTransformDeleg, out _btlStartManagerIsDelayTransformTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 18 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 80 00 00 00 8B FA"), _fighterModeDamageRequestMotionDeleg, out _fighterModeDamageRequestMotionTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8B C4 55 57 41 54 41 56 41 57 48 8D A8 F8 F6 FF FF"), _humanModeTransitDamageDeleg, out _humanModeTransitDamageTrampoline);

            long* damageVfTable = (long*)DragonEngineLibrary.Unsafe.CPP.ResolveRelativeAddress(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8D 05 ? ? ? ? 48 89 03 48 8D 05 ? ? ? ? 48 89 43 18 C5 C1 EF FF"), 7);
            MinHookHelper.createHook((IntPtr)damageVfTable[26], _fighterModeHumanDamageIsDownDeleg, out _fighterModeHumanDamageIsDownTrampoline);

            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8B C4 55 57 41 56 48 8D A8 18 FF FF FF 48 81 EC ? ? ? ? 48 C7 44 24 48 ? ? ? ?"), _humanModeTransitDmgCounterDeleg, out _humanModeTransitDmgCounterTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("40 57 48 83 EC ? 48 C7 44 24 20 ? ? ? ? 48 89 5C 24 48 48 89 74 24 58 48 8B F2 48 8B F9 48 83 C1 ?"), _humanModeTransitDmgSwayDeleg, out _humanModeTransitDmgSwayTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("40 56 57 41 56 41 57"), _damageExecValidDeleg, out _damageExecValidTrampoline);

            long* camFreeVfTable = (long*)DragonEngineLibrary.Unsafe.CPP.ResolveRelativeAddress(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8D 05 ? ? ? ? 48 89 07 48 8D B7 60 02 00 00"), 7);
            MinHookHelper.createHook((IntPtr)damageVfTable[79], _cameraFreeIsRotateBehindDeleg, out _cameraFreeIsRotateBehindTrampoline);

            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("40 57 48 83 EC ? 48 C7 44 24 20 ? ? ? ? 48 89 5C 24 70 48 89 74 24 78 48 8B F9 48 8D 99 20 4B 00 00"), _charaReqStartFighterDeleg, out _charaReqStartFighterTrampoline);

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
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("80 F9 ? 74 ? 0F B6 C1"), 5);

            //CAMERA FREE: DISABLE CAMERA TWERKING ON BATTLE/CAMERA LINK OUT
            //TODO URGENT: Only do this on combat!
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("E8 ? ? ? ? C5 FC 10 44 24 40 C4 C1 7C 11 87 40 01 00 00"), 5);

            //COMBAT: Prevent non-functional stun escape prompt from stuttering the game
            //on the extremely rare chance that we get Y6 wallbound/stunned
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 81 C1 ? ? ? ? 44 8D 42 05 E8 ? ? ? ? 48 8D 4B 30 48 83 C4 ?") + 11, 5);

            //COMBAT: Remove pausing block
            //TODO: TURN THIS INTO A HOOK FOR FINER CONTROL!
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 83 EC ? 48 8B 05 ? ? ? ? 80 B8 58 02 00 00 ? 0F 84 ? ? ? ?"), new byte[] { 0x31, 0xC0, 0xC3, 0x90 });

            //COMBAT: Force RPG pause menu instead of generic
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("40 57 41 54 41 55 41 56 41 57 48 83 EC ? 48 C7 44 24 20 ? ? ? ? 48 89 5C 24 68 48 89 6C 24 70 48 89 74 24 78 45 8B F9 4D 8B E0"), _pauseReq1Deleg, out _pauseReq1Trampoline);

            //COMBAT: Allow keyboard to be able to pause
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC ? 41 8B C0 8B FA"), _pauseBtnDeleg, out _pauseBtnTrampoline);

            //COMBAT: Prevent code that forces the enemy to get up when its their turn
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("FF 97 A8 01 00 00 49 8B CE E8 ? ? ? ? 84 C0"), 6);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("84 C0 74 ? 49 8B 06 48 8B 98 28 4B 00 00 48 8B 3B"), new byte[] { 0x90, 0x90, 0xEB });

            //COMBAT (RANGE): Disable filtering for cec_hact
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("74 ? C5 E9 EF D2 C5 FA 11 54 24 40"), 2);
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("0F 84 ? ? ? ? 48 8D 54 24 40 48 8B CF E8 ? ? ? ? 8D 43 FF"), 6);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("0F 84 ? ? ? ? 8D 4B FF 48 69 C1 ? ? ? ?"), new byte[] { 0xE9, 0x87, 0x0, 0x0, 0x0, 0x90 });
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("85 94 87 E0 00 00 00"), 7);

            //COMBAT: Prevent Ishioda from getting stunned when he is thrown from the crane truck.
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("E8 ? ? ? ? C5 F9 EF C0 C5 FA 7F 45 E8 48 8D 05 ? ? ? ? 48 89 45 D8 48 8D 55 D8"), 5);

            //COMBAT: Prevent the game from re-applying stun effects on Ishioda
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("45 33 C9 4C 8D 44 24 60 BA ? ? ? ?") + 13, 5);

            //COMBAT: Allow Ishioda to attack instead of forcing "defense command" on him which prevents him from doing so.
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 10 57 48 83 EC ? 48 8B F9 48 8B DA 48 8B 49 08 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? 48 8B CB 48 8B 90 80 0F 00 00"), new byte[] { 0xB8, 0x0, 0x0, 0x0, 0x0, 0xC3 });

            //GUARDING: Swap condition, player will have its humanmode checked. Enemies will use a hijacked function
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("E8 ? ? ? ? 48 8B 88 98 0F 00 00 48 85 C9 74 ? 83 B9 C0 00 00 00 ? 75 ? 48 8B 03") - 25, 0x75);
            IntPtr funcLoc = DragonEngineLibrary.Unsafe.CPP.PatternSearch("E8 ? ? ? ? 48 8B 88 98 0F 00 00 48 85 C9 74 ? 83 B9 C0 00 00 00 ? 75 ? 48 8B 03");
            IntPtr targetFunc = DragonEngineLibrary.Unsafe.CPP.ReadCall(DragonEngineLibrary.Unsafe.CPP.PatternSearch("E8 ? ? ? ? 48 8B 43 28 48 8B B8 F0 03 00 00 48 8B CF E8 ? ? ? ? C4 E3 79 04 83 80 00 00 00 ?"));
            DragonEngineLibrary.Unsafe.CPP.InjectHook(funcLoc, targetFunc);

            //CRANE TRUCK: prevent teleportation
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("E8 ? ? ? ? C5 F8 28 7C 24 70 C5 F8 28 B4 24 80 00 00 00 48 8B BC 24 B0 00 00 00"), 5);
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("E8 ? ? ? ? C5 F8 28 7C 24 70 C5 F8 28 B4 24 80 00 00 00 48 8B 4C 24 60 48 33 CC E8 ? ? ? ? 48 8B 9C 24 B0 00 00 00"), 5);

            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8B 88 98 0F 00 00 48 85 C9 74 ? 83 B9 C0 00 00 00 ? 75 ? 48 8B 03"), 7);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 85 C9 74 ? 83 B9 C0 00 00 00 ? 75 ? 48 8B 03"), 0x84, 0xC0, 0x90);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("83 B9 C0 00 00 00 ? 75 ? 48 8B 03"), 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x74);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("40 57 48 83 EC ? 48 C7 44 24 20 ? ? ? ? 48 89 5C 24 58 48 89 6C 24 60 48 89 74 24 68 48 8B F1 E8 ? ? ? ? 48 8B 46 28 48 8B B8 F0 03 00 00 48 8D 05 ? ? ? ? 48 89 44 24 28 33 ED 89 6C 24 30 48 8D 9F 08 06 00 00 48 89 5C 24 50 48 8B CB E8 ? ? ? ? 90 8D 4D 30 E8 ? ? ? ? 48 89 44 24 50 48 85 C0 74 ? 48 8D 0D ? ? ? ? 48 89 08 48 89 70 08 C5 F8 10 44 24 28 C5 F8 11 40 10 8B 48 18 48 85 C9 75 ? 48 89 68 18 48 89 68 20 48 89 68 28 EB ? 48 8B C5 48 83 BF F8 05 00 00 ? 75 ? 48 89 87 00 06 00 00 48 89 68 28 48 89 68 20 48 89 87 F8 05 00 00 48 8B 87 00 06 00 00 EB ? 48 8B 8F 00 06 00 00 48 89 41 28 48 8B 8F 00 06 00 00 48 89 48 20 48 89 68 28 48 89 87 00 06 00 00 48 8B 8F 10 06 00 00 48 39 48 20 75 ? 48 89 87 18 06 00 00 48 8B CB E8 ? ? ? ? 90 C6 86 98 00 00 00 ?"), _npcGuardDeleg, out _npcGuardTrampoline);
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8B C4 55 41 54 41 55 41 56 41 57 48 8D 68 A1 48 81 EC ? ? ? ? 48 C7 45 DF ? ? ? ? 48 89 58 10 48 89 70 18 48 89 78 20 48 8B 05 ? ? ? ? 48 33 C4 48 89 45 27 4C 8B F9"), _guardReactionIDDeleg, out _guardReactionIDTrampoline);

            //COMBAT: Remove teleportation of player after combat ends
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 89 5C 24 10 57 48 83 EC ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 44 24 60 48 8B F9 48 8B 0D ? ? ? ?"), 0xC3);

            //COMBAT: Force shift input on swaying (aka face enemy while quickstepping) (includes enemies)
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 83 EC ? 48 83 C1 ? E8 ? ? ? ? 0F B6 80 AE 00 00 00"), 0xB0, 0x01, 0xC3);

            //COMBAT: Disable perfect guard
            DragonEngineLibrary.Unsafe.CPP.NopMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("84 C0 74 ? B0 ? 48 83 C4 ? C3 32 C0 48 83 C4 ? C3 48 8B 02"), 2);
            DragonEngineLibrary.Unsafe.CPP.PatchMemory(DragonEngineLibrary.Unsafe.CPP.PatternSearch("74 ? B0 ? 48 83 C4 ? C3 32 C0 48 83 C4 ? C3 48 8B 02"), 0xEB);

            //COMBAT EFFECT: Use our own custom pibs
            MinHookHelper.createHook(DragonEngineLibrary.Unsafe.CPP.PatternSearch("48 8B C4 57 41 54 41 55 41 56 41 57 48 81 EC ? ? ? ? 48 C7 40 C0 ? ? ? ?"), _ptcManPlayDeleg, out _ptcManPlayTrampoline);


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

        //You stupid motherfucker, this caused the platforming bug on chapter 7
        //...but it's fixed now!
        private static HijackedGuardFunc _npcGuardDeleg;
        private static HijackedGuardFunc _npcGuardTrampoline;
        private static bool HijackedGuardFunct(IntPtr fighterPtr)
        {
            if (BrawlerBattleManager.Kasuga.IsValid())
                return true;
            else
                return _npcGuardTrampoline(fighterPtr);
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

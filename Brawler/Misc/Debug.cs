using DragonEngineLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.ConstrainedExecution;

namespace Brawler
{
    internal class Debug
    {
        public static bool ForceTargetingNearest = false;
        public static bool DontEraseRPGUI = false;
        public static bool HyperArmor = false;
        public static void InputUpdate()
        {
#if DEBUG
            if (DragonEngine.IsKeyDown(VirtualKey.Numpad7))
            {
                bool toggle = !FighterManager.IsBrawlerMode();
                FighterManager.ForceBrawlerMode(toggle);

                DragonEngine.Log("Brawler Mode: " + toggle);
            }

            else if (DragonEngine.IsKeyHeld(VirtualKey.LeftShift))
            {

                if (DragonEngine.IsKeyHeld(VirtualKey.Y))
                {
                    ForceTargetingNearest = !ForceTargetingNearest;
                    DragonEngine.Log("Force Targeting Nearest: " + ForceTargetingNearest);
                }
                else if (DragonEngine.IsKeyDown(VirtualKey.J))
                {
                    ECBattleStatus status = DragonEngine.GetHumanPlayer().GetBattleStatus();
                    status.AttackPower = 0;

                    DragonEngine.Log("0 damage");
                }
                else if (DragonEngine.IsKeyDown(VirtualKey.L))
                {
                    DontEraseRPGUI = !DontEraseRPGUI;
                    DragonEngine.Log("Dont Erase RPG UI: " + DontEraseRPGUI);
                }
                else if (DragonEngine.IsKeyDown(VirtualKey.F))
                    Mod.GodMode = !Mod.GodMode;
                else if (DragonEngine.IsKeyDown(VirtualKey.H))
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }

                }
                else if (DragonEngine.IsKeyDown(VirtualKey.G))
                {
                    IntPtr addr = SaveData.GetItem(0x15);
                    /*
                    unsafe
                    {
                        uint* brawlerFlags = (uint*)(addr.ToInt64() + 8);

                        DragonEngine.Log(*brawlerFlags + " Brawler Flags");
                    }
                    */


                    string configPath = Path.Combine(Mod.ModPath, "mod_config");
                    string iniPath = Path.Combine(configPath, "likeabrawler.ini");

                    BrawlerConfig.Init();
                    IniSettings.Read(iniPath);

                    BrawlerPlayer.Init();
                    WeaponManager.InitWeaponMovesets();
                    HeatActionManager.Init();

                    EffectEventManager.LoadScreen(0x1A);
                    EffectEventManager.PlayScreen(0x1A);
                    EffectEventManager.PlayScreen(0x21);
                }

                if (DragonEngine.IsKeyDown(VirtualKey.Numpad9))
                {
                    ECBattleStatus status = DragonEngine.GetHumanPlayer().GetBattleStatus();
                    status.AttackPower = 3500;

                    DragonEngine.Log("3500 damage");
                }

                if (DragonEngine.IsKeyDown(VirtualKey.T))
                {
                    Mod.DisableAttacksFromAI = !Mod.DisableAttacksFromAI;
                    DragonEngine.Log("AI Attack: " + !Mod.DisableAttacksFromAI); ;
                }

                if (DragonEngine.IsKeyDown(VirtualKey.B))
                {

                    foreach (Fighter f in BrawlerBattleManager.Enemies)
                        f.Character.ToDead();

                    DragonEngine.Log("Kill All");
                }

                if (DragonEngine.IsKeyDown(VirtualKey.X))
                {

                    AssetUnit asset = AssetManager.FindNearestAssetFromAll(BrawlerBattleManager.KasugaChara.GetPosCenter(), 0).Get();
                    BrawlerBattleManager.KasugaChara.RequestWarpPose(new PoseInfo(asset.GetPosCenter(), 0));


                    DragonEngine.Log(asset.AssetID + " " + asset.EntityUID.UID + " " + asset.IsCanBreak());

                }
            }

            else if (DragonEngine.IsKeyHeld(VirtualKey.LeftControl))
            {
                if (DragonEngine.IsKeyDown(VirtualKey.M))
                {
                    if (BrawlerBattleManager.Kasuga.IsValid())
                    {
                        Fighter kasuga = BrawlerBattleManager.Kasuga;

                        HActRangeInfo output = new HActRangeInfo();

                        if (kasuga.GetStatus().HAct.GetPlayInfo(ref output, HActRangeType.hit_wall))
                        {
                            Vector3 pos = (Vector3)output.Pos - ((output.Rot * Vector3.forward) * 0.08f);

                            kasuga.Character.WarpPosAndOrient(kasuga.Character.Transform.Position, output.Rot);
                            kasuga.Character.RequestWarpPose(new PoseInfo(pos, kasuga.Character.GetAngleY()));
                            kasuga.Character.HumanModeManager.ToAttackMode(new FighterCommandID(1351, 6));
                          //  kasuga.Character.RequestWarpPose(output.Pos, new  output.Rot);
                            //kasuga.Character.GetMotion().RequestGMT(MotionID.P_ICH_BTL_SUD_wal_jmp_f);
                        }


                    }
                }
            }
#endif
        }
    }
}

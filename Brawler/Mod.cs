using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using DragonEngineLibrary;
using Newtonsoft.Json;
using YazawaCommand;
using System.Globalization;
using Brawler.Auth;

namespace Brawler
{
    public class Mod : DragonEngineMod
    {

        public static bool IsGamePaused = false;
        public static bool IsGameFocused = false;

        public static bool DebugAutomaticCombo = false;
        public static bool DebugNoUpdate = false;
        public static bool DebugShowMenu = false;
        public static bool DisableAttacksFromAI = false;

        public const float CriticalHPRatio = 0.3f;

        public static bool GodMode = false;

        private static JsonSerializerSettings m_jsonWriteSettings = new JsonSerializerSettings { Formatting = Formatting.Indented, DefaultValueHandling = DefaultValueHandling.Ignore };

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);


        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        public static string ModPath;

#if DEBUG
        public static void DrawMenu()
        {
            //if (!DebugShowMenu)
            //   return;

            /*
            if(ImGui.Begin("Like a Brawler"))
            {
                ImGui.Text("Move Executing: " + (BrawlerPlayer.m_lastMove != null ? BrawlerPlayer.m_lastMove.MoveExecuting() : false));
                ImGui.Text("Attack Cooldown: " + BrawlerPlayer.m_attackCooldown);
            }
            */
        }
#endif

        //https://stackoverflow.com/a/7162873/14569631
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();

            if (activatedHandle == IntPtr.Zero)
                return false;       // No window is currently activated

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }

        public void InputThread()
        {
            if (GameVarManager.GetValueBool(GameVarID.is_pause))
                return;

            while (true)
            {

                ModInput.InputUpdate();
                Thread.Sleep(10);
#if DEBUG
                Debug.InputUpdate();
#endif
            }
        }

        private static void GameUpdate()
        {

            IsGamePaused = GameVarManager.GetValueBool(GameVarID.is_pause);
            IsGameFocused = ApplicationIsActivated();

            if (IsGamePaused)
            {
                if(BrawlerBattleManager.Kasuga.IsValid())
                {
                    //Ichiban may use some items on the pause menu
                    ECBattleStatus status = BrawlerBattleManager.Kasuga.GetStatus();
                    status.SetHPMax(Player.GetHPMax(Player.ID.kasuga));
                    status.SetHPCurrent(Player.GetHPNow(Player.ID.kasuga));
                }
                return;
            }

            ModInput.GameUpdate();

            BrawlerBattleManager.Update();
            YoshinoManager.Update();
            EXFollowups.Update();

            ModInput.Clear();

        }

        public static bool ShouldExecBrawlerInput()
        {
            Fighter kasugaFighter = BrawlerBattleManager.Kasuga;

            if (!kasugaFighter.IsValid() || BrawlerPlayer.Info.IsDead)
                return false;

            if (HeatActionManager.AnimProcedure)
                return false;

            if (TutorialManager.IsTutorialPromptVisible())
                return false;

            if (BrawlerPlayer.ThrowingWeapon)
                return false;

            //dont exec when synced
            if (BrawlerPlayer.Info.IsSync && !(AttackSimulator.PlayerInstance.Attacking() && AttackSimulator.PlayerInstance.CurrentAttack.IsSyncMove()))
                return false;

            if (BattleTurnManager.CurrentPhase != BattleTurnManager.TurnPhase.Action)
                return false;

            if (BrawlerBattleManager.HActIsPlaying)
                return false;

            if (BrawlerBattleManager.BattleTime < BrawlerBattleManager.BattleStartTime)
                return false;

            if (BrawlerBattleManager.Enemies.Length <= 0)
                return false;

            // if (allEnemies.Where(x => x.IsDead()).ToArray().Length == allEnemies.Length)
            //  return false;

            return true;
        }


        public static bool AllowInputUpdate()
        {
#if DEBUG
            if (Debugger.IsAttached)
                return true;
            else
#endif
                return Mod.IsGameFocused && !Mod.IsGamePaused;
        }

        public override void OnModInit()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Assembly assmb = Assembly.GetExecutingAssembly();
            ModPath = Path.GetDirectoryName(assmb.Location);

            BrawlerConfig.Init();

            string configPath = Path.Combine(ModPath, "mod_config");
            string iniPath = Path.Combine(configPath, "likeabrawler.ini");

            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);

            if (!File.Exists(iniPath))
                IniSettings.WriteDefault(iniPath);

            IniSettings.Read(iniPath);

            AuthConditionManager.Init();
            AuthCustomNodeManager.Init();


            BattleMusic.Init();
            BrawlerPlayer.Init();
            HeatActionManager.Init();
            HActLifeGaugeManager.Init();
            RevelationManager.Init();
            WeaponManager.InitWeaponMovesets();
            TutorialManager.Init();

            DragonEngine.RegisterJob(GameUpdate, DEJob.Update);
            FighterManager.ForceBrawlerMode(true);

            BrawlerHooks.Init();

            DragonEngine.RegisterJob(AttackSimulator.PlayerInstance.AttackDrawUpdate, DEJob.Draw, true);

            //Problematic, rose temperatures from 70 to 85???
            Thread thread = new Thread(InputThread);
            thread.Start();
        }

        public static YFC ReadYFC(string name)
        {
            return YazawaCommandManager.LoadYFC(name);
        }

        public static YHC ReadYHC(string name)
        {
            return YazawaCommandManager.LoadYHC(name);
        }
    }
}

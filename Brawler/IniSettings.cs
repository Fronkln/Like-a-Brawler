using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal static class IniSettings
    {
        public static bool PreferGreenUI = true;
        public static bool ShowPlayerDamage = false;
        public static bool ShowEnemyDamage = false;
        public static bool EnableBattleResults = false;
        public static bool PreferJudgeGauge = false;


        public static void Read(string iniPath)
        {
            Ini ini = new Ini(iniPath);
            ShowPlayerDamage = ini.GetValue("ShowPlayerDamage", "Display") == "1";
            ShowEnemyDamage = ini.GetValue("ShowEnemyDamage", "Display") == "1";
            EnableBattleResults = ini.GetValue("ShowBattleResult", "Display") == "1";
            PreferJudgeGauge = ini.GetValue("PreferJudgeGauge", "Display") == "1";
        }

        public static void WriteDefault(string iniPath)
        {
            Ini ini = new Ini(iniPath);
            ini.WriteValue("ShowPlayerDamage", "Display", "1");
            ini.WriteValue("ShowEnemyDamage", "Display", "0");
            ini.WriteValue("ShowBattleResult", "Display", "0");
            ini.WriteValue("PreferJudgmentGauge", "Display", "0");
            ini.Save();
        }
    }
}

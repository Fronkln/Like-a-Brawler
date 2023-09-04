using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal class BrawlerConfig
    {
        //Amount of uses before the weapon breaks
        private static Dictionary<AssetArmsCategoryID, int> m_useCounts = new Dictionary<AssetArmsCategoryID, int>();
        private const int m_defaultUseCount = 3;

        public static void Init()
        {
            ReadAssetUseCounts();
        }

        private static void SetDefaultAssetUseCounts()
        {
            for (int i = 0; i < Enum.GetValues(typeof(AssetArmsCategoryID)).Length; i++)
                m_useCounts[(AssetArmsCategoryID)i] = m_defaultUseCount;
        }

        private static void ReadAssetUseCounts()
        {
            SetDefaultAssetUseCounts();

            string filePath = Path.Combine(Mod.ModPath, "mod_config", "weapon_category_use_count.txt");

            //Default settings
            if (!File.Exists(filePath))
                return;

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] split = line.Split(' ');

                if (split.Length <= 0)
                    continue;

                AssetArmsCategoryID category;

                if (!Enum.TryParse(split[0], out category))
                    continue;

                int useCount = m_defaultUseCount;

                if (split.Length <= 1 || !int.TryParse(split[1], out useCount))
                {
                    m_useCounts[category] = m_defaultUseCount;
                    continue;
                }

                m_useCounts[category] = useCount;
            }
        }


        public static int GetUseCountForAssetCategory(AssetArmsCategoryID category)
        {
            return m_useCounts[category];
        }

    
    }
}

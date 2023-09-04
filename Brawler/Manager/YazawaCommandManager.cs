using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YazawaCommand;

namespace Brawler
{
    internal static class YazawaCommandManager
    {
        private static Dictionary<string, YFC> m_loadedYFC = new Dictionary<string, YFC>();
        private static Dictionary<string, YHC> m_loadedYHC = new Dictionary<string, YHC>();

        public static YFC GetYFCByName(string name)
        {
            if (!m_loadedYFC.ContainsKey(name))
                return null;

            return m_loadedYFC[name];
        }

        public static YHC GetYHCByName(string name)
        {
            if (!m_loadedYHC.ContainsKey(name))
                return null;

            return m_loadedYHC[name];
        }

        public static YHC LoadYHC(string name)
        {
            string path = Path.Combine(Mod.ModPath, "battle", "yhc", name);
            YHC yhc = YHC.Read(path);

#if DEBUG
            if (yhc == null)
                Mod.MessageBox((IntPtr)0, $"Error reading YHC at {path}\n\nMissing/invalid file", "YHC Error", 0x00000010);
#endif

            m_loadedYHC[Path.GetFileNameWithoutExtension(path)] = yhc;

            return yhc;
        }

        public static YFC LoadYFC(string name)
        {
            string path = Path.Combine(Mod.ModPath, "battle", "yfc", name);
            YFC yfc = YFC.Read(path);

#if DEBUG
            if (yfc == null)
                Mod.MessageBox((IntPtr)0, $"Error reading YFC at {path}\n\nMissing/invalid file", "YFC Error", 0x00000010);
#endif

            m_loadedYFC[Path.GetFileNameWithoutExtension(path)] = yfc;

            return yfc;
        }
    }
}

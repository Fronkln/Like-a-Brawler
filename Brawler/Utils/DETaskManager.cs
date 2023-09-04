using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    public static class DETaskManager
    {
        public static List<DETask> Tasks = new List<DETask>();

        public static void Update()
        {
            if (Mod.IsGamePaused)
                return;

            List<DETask> tasks = new List<DETask>(Tasks);

            foreach (DETask task in tasks)
                task.Run();
        }
    }
}

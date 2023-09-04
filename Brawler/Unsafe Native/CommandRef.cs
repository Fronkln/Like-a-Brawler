using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal unsafe struct CommandRef
    {
        public int CommandType;
        public RPGSkillID Command;
        public ItemID Item;
    }
}

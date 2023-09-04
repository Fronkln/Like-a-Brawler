using System.Collections.Generic;
using DragonEngineLibrary;
using YazawaCommand;

namespace Brawler
{
    //Information returned by checking YHC
    public class HeatActionInformation
    {
        public HeatActionAttack Hact = null;
        public Fighter Performer = null;
        public HActRangeInfo RangeInfo; //Assigned if found
        public Dictionary<HeatActionActorType, Fighter> Map = new Dictionary<HeatActionActorType, Fighter>();
    }
}

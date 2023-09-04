using System;
using System.Collections.Generic;
using DragonEngineLibrary;

namespace Brawler
{
    //Isn't used in generic fights.
    //Used in special fights to co-operate tag teams and misc.
    internal class EnemyAITeam
    {
        public Fighter[] Members = new Fighter[0];
        public List<EnemyCoopHAct> TagTeams = new List<EnemyCoopHAct>();


    }
}

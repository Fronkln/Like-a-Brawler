using System;
using System.Timers;
using System.Linq;
using System.Collections.Generic;
using DragonEngineLibrary;


namespace Brawler
{
    [System.Serializable]
    public class Moveset
    {
        public List<MoveBase> Moves = new List<MoveBase>();
        public RPGSkillID RepelCounter;

        public Moveset(RPGSkillID repel = RPGSkillID.invalid, params MoveBase[] moves)
        {
            Moves = moves.ToList();
            RepelCounter = repel;
        }

        public T GetMoveOfType<T>() where T : MoveBase, new()
        {
            foreach (MoveBase move in Moves)
                if (move as T != null)
                    return (T)move;

            return null;
        }
    }
}

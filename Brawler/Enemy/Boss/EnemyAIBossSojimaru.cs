using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonEngineLibrary;

namespace Brawler
{
    internal class EnemyAIBossSojimaru : EnemyAIBoss
    {
        private float m_aliveTime = 0;

        public override void CombatUpdate()
        {
            base.CombatUpdate();

            m_aliveTime += DragonEngine.deltaTime;

            //Anti frustration, this battle doesnt work in brawler
            //Die after 40 secs if not dead
            if (m_aliveTime > 40)
                Chara.Get().ToDead();
        }
    }
}

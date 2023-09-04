using System;
using DragonEngineLibrary;

namespace Brawler.Enemy
{
    /// <summary>
    /// Module when the enemy is being juggled by Ichiban (he is heated as hell)
    /// </summary>
    internal class EnemyJuggleModule : EnemyModule
    {
        public bool JuggleProcedure = false;
        private Vector4 m_lastHitPos;

        private float m_fallSpeedFactor;

        private const float JUGGLE_START_FALL_SPEED = 0f;
        private const float JUGGLE_FALL_SPEED_INCREMENT = 0.0005f;

        public int JuggleCount = 0;

        public void OnJuggleHit(Vector4 hitPos)
        {
            int antiAbuse = JuggleCount / 2;

            m_fallSpeedFactor = JUGGLE_START_FALL_SPEED + ((JUGGLE_FALL_SPEED_INCREMENT * 2.5f) * antiAbuse);

            if (!JuggleProcedure)
            {
                m_lastHitPos = hitPos;
                JuggleProcedure = true;
                //AI.Chara.Get().RequestMovePose(new PoseInfo(hitPos, AI.Chara.Get().GetAngleY()));
            }
            else
            {
                m_lastHitPos += -AI.Chara.Get().Transform.forwardDirection * 0.3f;
            }

            AI.Chara.Get().RequestMovePose(new PoseInfo(hitPos, AI.Chara.Get().GetAngleY()));

            JuggleCount++;
        }

        public override void Update()
        {
            base.Update();

            if (JuggleProcedure)
            {
                m_fallSpeedFactor += JUGGLE_FALL_SPEED_INCREMENT;
                m_lastHitPos.y -= m_fallSpeedFactor;

                Character enemyChara = AI.Chara.Get();

                Vector3 interpolatedPos = (Vector3)m_lastHitPos;
                enemyChara.RequestMovePose(new PoseInfo(interpolatedPos, AI.Chara.Get().GetAngleY()));

                if(BrawlerBattleManager.KasugaChara.Transform.Position.y - 0.2f > interpolatedPos.y) 
                {;
                    JuggleProcedure = false;
                    JuggleCount = 0;
                    AI.Chara.Get().GetMotion().RequestRagdoll(new RequestRagdollOptions());
                }
            }

        }
    }
}

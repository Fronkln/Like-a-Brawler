using DragonEngineLibrary;

namespace Brawler
{
    public static class EXModule
    {
        private static RepeatingTask m_exHeatDecay = new RepeatingTask(
            delegate
            {
                if(BrawlerPlayer.IsEXGamer)
                Player.SetHeatNow(Player.ID.kasuga, Player.GetHeatNow(Player.ID.kasuga) - 1);
            }, 0.1f
            );

        public static void Update()
        {
            if (!BrawlerPlayer.IsEXGamer || BrawlerBattleManager.HActIsPlaying || BattleTurnManager.CurrentPhase != BattleTurnManager.TurnPhase.Action)
            {
                m_exHeatDecay.Paused = true;
                return;
            }

            m_exHeatDecay.Paused = false;

            if (BrawlerPlayer.IsEXGamer && !BrawlerBattleManager.HActIsPlaying)
            {
                int heat = Player.GetHeatNow(Player.ID.kasuga);

                if (heat <= 0)
                {
                    BrawlerPlayer.IsEXGamer = false;
                    BrawlerPlayer.WantTransform = true;
                }
            }
        }
    }
}

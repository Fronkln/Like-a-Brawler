using System;
using System.Collections.Generic;
using DragonEngineLibrary;

namespace Brawler
{
    public static class HeatModule
    {
        //Play sounds/effects once player can do heat actions
        private static int m_HeatActionCount; //calculated amount of heat actions the player can do as of now
        private static bool m_canDoHeatDoOnce = true;

        public static void Update()
        {
            if(!BrawlerBattleManager.BattleStartDoOnce)
            {
                m_canDoHeatDoOnce = true;
                return;
            }

            int curHeat = Player.GetHeatNow(Player.ID.kasuga);
            int heatMax = Player.GetHeatMax(Player.ID.kasuga);

            int numHeatActions = 0;

            if(heatMax < HeatActionManager.TEMP_HACT_COST)
            {
                if(curHeat == heatMax)
                    numHeatActions = 1;
            }
            else
                numHeatActions = curHeat / HeatActionManager.TEMP_HACT_COST;

            if(numHeatActions > m_HeatActionCount)
                OnCanDoHeat();

            m_HeatActionCount = numHeatActions;
        }

        private static void OnCanDoHeat()
        {
            SoundManager.PlayCue(SoundCuesheetID.battle_common, 15, 0);
            BrawlerBattleManager.KasugaChara.Components.EffectEvent.Get().PlayEvent(EffectEventCharaID.OgrefHeatAuraKr02);
        }
    }
}

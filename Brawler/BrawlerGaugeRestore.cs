#define DO_NOT_USE_NATIVE_IMP
using DragonEngineLibrary;
using DragonEngineLibrary.Service;
using System;

namespace Brawler
{

    //Hooking into constructor would crash unless debugging.
    //This is the only way for now!
    internal static class BrawlerGaugeRestore
    {
        private static bool m_createdUIDoOnce = false;
        private static UIHandleBase m_rpgHandle;
        private static UIHandleBase m_healthGauge;
        private static UIHandleBase m_heatGauge;
        private static UIHandleBase m_canHact;
        private static UIHandleBase m_hactPrompt;

#if DO_NOT_USE_NATIVE_IMP
        private static UIHandleBase m_healthFrame;
        private static UIHandleBase m_healthGaugeNow;

        private static UIHandleBase m_heatGaugeNow;

        //recovery is simulated but damage isnt, up to me to save the day now
        private static UIHandleBase m_gaugeDamaged;

        private static bool m_isPinch = false;
        private static bool m_isDead = false;
        private static float lastPlayerHP;

        private static bool m_canHactFlag = false;

#endif

        public static void Update()
        {
#if !DO_NOT_USE_NATIVE_IMP
            EntityHandle<UIEntityPlayerGauge> gauge = SceneService.CurrentScene.Get().GetSceneEntity<UIEntityPlayerGauge>(SceneEntity.ui_entity_player_gauge);

            if (gauge.IsValid())
            {
                if (!m_createdUIDoOnce)
                        Create(gauge.Get());
            }
            else
                m_createdUIDoOnce = false;

            Test();
#else

            if (DragonEngine.GetHumanPlayer().IsValid() || DragonEngine.GetHumanPlayer().IsDead())
            {
                if (!m_createdUIDoOnce)
                    Create(null);
            }
            else
                m_createdUIDoOnce = false;

            HActPromptUpdate(HeatActionManager.CanHAct());


            if (IniSettings.PreferJudgeGauge)
            {
                if (m_healthGauge.Handle > 0)
                    Test();
            }
            else
            {

                if (m_createdUIDoOnce)
                {
                    //Do this or you dereference a null pointer at getcontrolbase
                    if (!BrawlerBattleManager.KasugaChara.IsValid() || BrawlerBattleManager.KasugaChara.IsDead())
                        return;

                    for (int i = 1; i < 4; i++)
                        m_rpgHandle.GetChild(i).SetVisible(false);

                    UIHandleBase kasugaUIRoot = m_rpgHandle.GetChild(0).GetChild(0);

                    BattleTurnManager.TurnPhase phase = BattleTurnManager.CurrentPhase;

                    if (phase == BattleTurnManager.TurnPhase.Action || phase == BattleTurnManager.TurnPhase.Cleanup)
                        kasugaUIRoot.SetPosition(new Vector2(-1275, 0));
                    else
                        kasugaUIRoot.SetPosition(new Vector2(-150, 0));

                    UIHandleBase kasugaImage = kasugaUIRoot.GetChild(1).GetChild(0);
                    kasugaImage.SetPosition(new Vector2(100, 0));
                    kasugaImage.GetControlBase().GetPlayer().Scale = new Vector2(2, 1);

                    long curHP = Player.GetHPNow(Player.ID.kasuga);
                    long maxHP = Player.GetHPMax(Player.ID.kasuga);

                    UIHandleBase healthGaugeRoot = kasugaUIRoot.GetChild(1).GetChild(2);
                    healthGaugeRoot.SetValue((float)curHP / (float)maxHP);
                    healthGaugeRoot.SetWidth(250);

                    UIHandleBase healthGaugeText = kasugaUIRoot.GetChild(1).GetChild(5);
                    healthGaugeText.SetText(curHP.ToString());

                    UIHandleBase heatGaugeRoot = kasugaUIRoot.GetChild(1).GetChild(3);
                    heatGaugeRoot.SetWidth(250);
                    heatGaugeRoot.SetValue((float)Player.GetHeatNow(Player.ID.kasuga) / (float)Player.GetHeatMax(Player.ID.kasuga));

                    kasugaUIRoot.GetChild(1).GetChild(4).SetText(Player.GetHeatNow(Player.ID.kasuga).ToString());
                    kasugaUIRoot.GetChild(1).GetChild(7).SetVisible(false);
                }
            }
            
            if (!BrawlerBattleManager.KasugaChara.IsValid())
                return;

            lastPlayerHP = Player.GetHPNow(Player.ID.kasuga);

            if (lastPlayerHP <= (int)(Player.GetHPMax(Player.ID.kasuga) * Mod.CriticalHPRatio))
            {
                if (BrawlerBattleManager.Kasuga.IsDead())
                {
                    if (!m_isDead)
                        m_isDead = true;
                }
                else if (!m_isPinch)
                {
                    EffectEventManager.PlayScreen(7, true, true);
                    m_isPinch = true;
                }
            }
            else
            {
                if (m_isDead)
                    m_isDead = false;

                if (m_isPinch)
                {
                    EffectEventManager.StopScreen(7);
                    m_healthGauge.PlayAnimationSet(0x23F);
                    m_isPinch = false;
                }
            }
#endif
        }

        private static void Create(UIEntityPlayerGauge gauge)
        {
            EffectEventManager.LoadScreen(7);

            m_canHact = UI.Create(324, 1);
            CreateHActPromptWorkaround();

            if (IniSettings.PreferJudgeGauge)
            {
                m_healthGauge = UI.Create(301, 1);
                //Reusing player health gauge for heat gauge for now...
                m_heatGauge = UI.Play(356, 1);

                DragonEngine.Log("Health Gauge Handle: " + m_healthGauge.Handle);
                DragonEngine.Log("Heat Gauge Handle: " + m_heatGauge.Handle);

#if !DO_NOT_USE_NATIVE_IMP
            gauge.LifeGauge = m_healthGauge;
#else
                m_healthFrame = m_healthGauge.GetChild(0).GetChild(0).GetChild(0);
                m_healthGaugeNow = m_healthGauge.GetChild(0).GetChild(0).GetChild(1);
                //TEMPORARY: better than a delayed gauge.
                if (!IniSettings.PreferGreenUI)
                    m_healthGaugeNow.GetChild(2).SetVisible(false);

                m_heatGauge.SetVisible(false);
                //   m_heatGauge.GetChild(0).GetChild(0).GetChild(0).SetWidth(200);


                m_heatGauge.GetChild(0).GetChild(0).SetWidth(200);
                m_heatGauge.GetChild(0).GetChild(0).GetChild(1);
                m_heatGaugeNow = m_heatGauge.GetChild(0).GetChild(0).GetChild(1);
                m_heatGaugeNow.GetChild(2).SetVisible(false);

                //CONVERT
                m_heatGauge.SetPosition(new Vector4(0, 17, 0, 0));

                m_createdUIDoOnce = true;

#endif

            }
            else
            {
                unsafe
                {
                    IntPtr gaugeMan = (IntPtr)BrawlerBattleManager.KasugaChara.GetSceneEntity<EntityBase>((SceneEntity)0xB8).Get().Pointer.ToInt64() + 0xE8;
                    ulong* handlePtr = (ulong*)(gaugeMan.ToInt64() + 0xC30);
                    ulong handle = *handlePtr;

                    if (handle <= 0)
                        return;

                    m_rpgHandle = new UIHandleBase(handle);
                    m_createdUIDoOnce = true;

                    DragonEngine.Log(m_rpgHandle);
                }
            }

        }

        //TODO: Only do this on battle start
        //Create Kiwami action UI (texture has been made so small it looks like a prompt)
        //Set to 2.5 seconds
        //Pause
        //Profit
        private static void CreateHActPromptWorkaround()
        {
            m_hactPrompt = UI.Play(995, 0);
            m_hactPrompt.SetVisible(false);
            m_hactPrompt.SetFrame(90);

            m_hactPrompt.Pause();
        }

        public static float CalcHealthGaugeWidth(float min, float max, float maxHP)
        {
            if (maxHP < min)
                return min;
            if (maxHP > max)
                return max;

            return maxHP;
        }


        public static void Test()
        {

            if (IniSettings.PreferJudgeGauge)
            {
#if !DO_NOT_USE_NATIVE_IMP
            UIEntityPlayerGauge gauge = SceneService.CurrentScene.Get().GetSceneEntity<UIEntityPlayerGauge>(SceneEntity.ui_entity_player_gauge);

            if (gauge.IsValid())
            {
                gauge.GaugeNow.SetValue(1);
                gauge.GaugeNow.SetWidth(800);
                gauge.GaugeFrame.SetWidth(800);
            }
#else
                if (!DragonEngine.GetHumanPlayer().IsValid() || !BrawlerBattleManager.Kasuga.IsValid())
                {
                    m_healthGauge.SetVisible(false);
                    m_heatGauge.SetVisible(false);
                    m_canHact.SetVisible(false);
                    EffectEventManager.StopScreen(7, true);
                    return;
                }
                else
                {
                    m_healthGauge.SetVisible(true);
                    m_heatGauge.SetVisible(true);
                    m_canHact.SetVisible(true);
                }

                int curHeat = Player.GetHeatNow(Player.ID.kasuga);
                int maxHeat = Player.GetHeatMax(Player.ID.kasuga);

                bool canHeat = curHeat >= (maxHeat * 0.3f);

                if (!m_canHactFlag)
                {
                    if (canHeat)
                    {
                        m_canHactFlag = true;
                        m_canHact.PlayAnimationSet(6056);
                    }
                }
                else
                {
                    if (!canHeat)
                    {
                        m_canHactFlag = false;
                        m_canHact.PlayAnimationSet(6057);
                    }
                }

                long playerHP = Player.GetHPNow(Player.ID.kasuga);
                long playerMaxHP = Player.GetHPMax(Player.ID.kasuga);

                float gaugeWidth = CalcHealthGaugeWidth(400, 800, Player.GetHPMax(Player.ID.kasuga));

                m_healthGaugeNow.SetWidth(gaugeWidth);
                m_healthFrame.SetWidth(gaugeWidth);

                if (lastPlayerHP <= (int)(playerMaxHP * Mod.CriticalHPRatio))
                {
                    if (BrawlerBattleManager.Kasuga.IsDead())
                    {
                        if (!m_isDead)
                        {
                            m_healthGauge.PlayAnimationSet(0x241);
                            m_isDead = true;
                        }
                    }
                    else
                    {
                        if (m_isDead)
                        {
                            m_healthGauge.PlayAnimationSet(0x23A);
                            m_isDead = false;
                        }
                    }

                    if (!m_isPinch)
                    {
                        EffectEventManager.PlayScreen(7, true, true);
                        m_healthGauge.PlayAnimationSet(0x23E);
                        m_isPinch = true;
                    }
                }
                else
                {
                    if (m_isPinch)
                    {
                        EffectEventManager.StopScreen(7);
                        m_healthGauge.PlayAnimationSet(0x23F);
                        m_isPinch = false;
                    }
                }

                if (playerHP != lastPlayerHP)
                {
                    if (playerHP < lastPlayerHP)
                        m_healthGauge.PlayAnimationSet(576);
                }

                m_heatGaugeNow.SetValue((float)curHeat / (float)maxHeat);
                m_healthGaugeNow.SetValue((float)playerHP / (float)playerMaxHP);

                lastPlayerHP = playerHP;
            }
#endif
        }

        public static void HActPromptUpdate(bool canHact)
        {
            m_hactPrompt.SetVisible(canHact);

            m_hactPrompt.SetFrame(canHact ? 75 : 90);
        }


        //Update RPG UI manually, primarily used on pause menu
        public static void UpdateManual()
        {

        }
    }
}

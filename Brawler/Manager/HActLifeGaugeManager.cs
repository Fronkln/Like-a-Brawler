using DragonEngineLibrary;
using System;

namespace Brawler
{
    //TODO: Also maybe use this for bosses?
    public static class HActLifeGaugeManager
    {
        private static UIHandleBase m_uiRoot;
        private static UIHandleBase m_nameRoot;
        private static UIHandleBase m_gaugeRoot;
        private static UIHandleBase m_lifeRoot;


        private static Fighter m_lastInitedFighter = new Fighter();

        public static void Init()
        {
            HActDamage.OnDamageDealt += OnDamageDealt;
            HeatActionManager.OnHActStart += OnHActStart;
            HeatActionManager.OnHActEnd += OnHActEnd;
        }

        private static void InitGauge()
        {
            //TODO VERY IMPORTANT: RELEASE OLD HANDLE
            if(m_uiRoot.Handle != 0)
            {
                //release
                m_uiRoot.Release();
                m_uiRoot.Handle = 0;
            }

            m_uiRoot = UI.Create(355, 0); //boss_life_gauge leftover from JE
            m_uiRoot.PlayAnimationSet(693);
            m_nameRoot = m_uiRoot.GetChild(0);
            m_gaugeRoot = m_uiRoot.GetChild(0).GetChild(0);
            m_lifeRoot = m_gaugeRoot.GetChild(3);
            m_lifeRoot.SetMaterialColor(0xFF0000ED); //237 0 0 255

            SetVisible(false);
        }

        private static void InitFighter(Fighter fighter)
        {
            ECBattleStatus status = fighter.GetStatus();
            SetText(fighter.Character.GetConstructor().GetAgentComponent().SoldierInfo.Get().Name);
            SetValue(status.CurrentHP, status.MaxHP);
        }

        public static void SetText(string text)
        {
            m_nameRoot.SetText(text);
        }

        public static void SetValue(long curHp, long maxHp)
        {
            m_gaugeRoot.SetValue((float)curHp / (float)maxHp);
        }

        public static void SetVisible(bool visible)
        {
            m_uiRoot.SetVisible(visible);
        }


        public static void Update()
        {
            if(m_gaugeRoot.Handle != 0)
                if(!BrawlerBattleManager.KasugaChara.IsValid() || BrawlerBattleManager.Kasuga.IsDead())
                {
                    m_uiRoot.Release();
                    m_uiRoot.Handle = 0;
                }    
        }

        private static void OnHActStart()
        {
            BattleTurnManager.TurnPhase phase = BattleTurnManager.CurrentPhase;

            if (BrawlerBattleManager.EnemiesNearest.Length <= 0 || phase <= BattleTurnManager.TurnPhase.Start || !BrawlerBattleManager.CurrentHActIsY7B)
            {
                SetVisible(false);
                return;
            }

            InitGauge();

            SetVisible(true);

            if(m_lastInitedFighter != BrawlerBattleManager.EnemiesNearest[0])
                InitFighter((BrawlerBattleManager.EnemiesNearest[0]));;
        }

        private static void OnHActEnd()
        {
            SetVisible(false);
        }

        private static void OnDamageDealt(Fighter fighter, long oldHp, long newHp)
        {
            if (fighter.IsPlayer())
                return;

            ECBattleStatus fighterStatus = fighter.GetStatus();
            long maxHp = fighterStatus.MaxHP;

            SetValue(oldHp, maxHp);
            SetValue(newHp, maxHp);

            if(newHp == 0)
                m_uiRoot.PlayAnimationSet(695); //boss_life_gauge_judge/play_dead
            else
                m_uiRoot.PlayAnimationSet(696); //boss_life_gauge_judge/play_damage
        }
    }
}

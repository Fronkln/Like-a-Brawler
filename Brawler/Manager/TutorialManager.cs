using System;
using System.Collections.Generic;
using DragonEngineLibrary;


namespace Brawler
{
    internal static class TutorialManager
    {
        public static bool Active = false;

        private static UIHandleBase m_tutorialHandle;
        private static UIHandleBase m_textHandle;
        private static UIHandleBase m_timerHandle;

        private const uint ANM_SHOW_TUTORIAL_UI = 271;
        private const uint ANM_HIDE_TUTORIAL_UI = 272;
        private const uint ANM_TUTORIAL_SUCCESS = 273;

        private static TutorialSegment[] m_tutorialSegments;
        private static int m_curSegment = 0;
        private static float m_curSegmentTimer = 0;

        private static bool m_tutorialVisible = false;
        private static bool m_wait = false;

        private static bool m_goNext = false;

        public static void Initialize(TutorialSegment[] segments)
        {
            new DETask(delegate { return !HActManager.IsPlaying() && BattleTurnManager.CurrentPhase == BattleTurnManager.TurnPhase.Action; }, delegate {  OnCombatStart(segments); });
        }

        private static void OnCombatStart(TutorialSegment[] segments)
        {
            m_tutorialHandle = UI.Create(201, 1);
            m_textHandle = m_tutorialHandle.GetChild(0);
            m_timerHandle = m_tutorialHandle.GetChild(1);

            //Details section
            m_tutorialHandle.GetChild(2).SetVisible(false);

            //  m_textHandle.SetText("Hit the <color=heal_hp>griddy</color> <color=red>1337</color> times or die\n Performing Twerk Actions");

            m_tutorialSegments = segments;
            m_curSegment = 0;

            new DETaskTime(1.5f, delegate { Active = true; StartSegment(0); });
        }

        public static void SetVisible(bool visible)
        {
            m_tutorialHandle.PlayAnimationSet(visible ? ANM_SHOW_TUTORIAL_UI : ANM_HIDE_TUTORIAL_UI);
            m_tutorialVisible = visible;
        }

        public static void StartSegment(int idx)
        {
            if (m_curSegment >= m_tutorialSegments.Length)
                return;

            TutorialSegment segment = m_tutorialSegments[idx];

            if (!segment.Silent)
            {
                if (m_tutorialVisible)
                {
                    SetVisible(false);
                    new DETaskTime(1.0f,
                        delegate
                        {
                            m_textHandle.SetText(segment.Instructions);
                            SetVisible(true);
                        });
                }
                else
                {
                    m_textHandle.SetText(segment.Instructions);
                    SetVisible(true);
                }
            }
            else
                SetVisible(false);

            m_timerHandle.SetVisible(segment.TimeToComplete > 0);
            m_curSegmentTimer = segment.TimeToComplete;
            segment.OnStart?.Invoke();
        }

        public static bool IsTutorialPromptVisible()
        {
            return UI.Get(0x2BC).Handle != 0;
        }

        public static void OnSuccess()
        {
            m_tutorialSegments[m_curSegment].OnEnd?.Invoke();
            m_wait = true;
            new DETaskTime(1.5f, delegate { StartSegment(++m_curSegment); m_wait = false ; });
            m_tutorialHandle.PlayAnimationSet(ANM_TUTORIAL_SUCCESS);
        }

        public static void OnFail()
        {
            m_tutorialSegments[m_curSegment].OnEnd?.Invoke();
            if (!m_tutorialSegments[m_curSegment].Silent)
            {
                m_wait = true;
                new DETaskTime(1, delegate { StartSegment(++m_curSegment); m_wait = false; });
            }
            else
                StartSegment(++m_curSegment);
        }

        public static bool AllowPlayerDamage()
        {
            if (!Active)
                return true;
            else
                return !m_tutorialSegments[m_curSegment].Modifiers.HasFlag(TutorialModifier.PlayerDontTakeDamage);
        }

        public static bool AllowEnemyDamage()
        {
            if (!Active)
                return true;
            else
                return !m_tutorialSegments[m_curSegment].Modifiers.HasFlag(TutorialModifier.EnemyDontTakeDamage);
        }

        public static void Update()
        {
            if (!Active || Mod.IsGamePaused || BrawlerBattleManager.HActIsPlaying || IsTutorialPromptVisible() || m_wait)
                return;


            if (m_curSegment >= m_tutorialSegments.Length)
            {
                Active = false;
                m_curSegment = 0;
                m_tutorialSegments = null;
                SetVisible(false);
                return;
            }

#if DEBUG
            if(DragonEngine.IsKeyHeld(VirtualKey.LeftShift))
                if(DragonEngine.IsKeyHeld(VirtualKey.F))
                {
                    OnSuccess();
                    return;
                }
#endif

            TutorialSegment curSegment = m_tutorialSegments[m_curSegment];

            if(m_curSegmentTimer <= 0 && curSegment.HasTime())
            {
                if (curSegment.TimeoutIsSuccess)
                    OnSuccess();
                else
                    OnFail();

                return;
            }
            else
            {
                if (curSegment.IsCompleteDelegate != null && curSegment.IsCompleteDelegate.Invoke())
                {
                    OnSuccess();
                    return;
                }
            }

            curSegment.UpdateDelegate?.Invoke();

            if (curSegment.TimeToComplete <= 0 || m_curSegmentTimer < 0)
                return;

            m_curSegmentTimer -= DragonEngine.deltaTime;
            TimeSpan timerTime = TimeSpan.FromSeconds(m_curSegmentTimer);

            m_timerHandle.SetText(timerTime.ToString(@"mm\:ss"));
        }

        public static void SetText(string txt)
        {
            m_textHandle.SetText(txt);
        }

        public static string GetFormattedButtonStr(TutorialButton button)
        {
            bool isKeyboard = InputInterface.IsKeyboardActive();

            switch(button)
            {
                
                case TutorialButton.LightAttack:
                    if (isKeyboard)
                        return "<symbol=y7b_mouse_button_l>";
                    else
                        return "<symbol=button_shikaku>";
                case TutorialButton.HeavyAttack:
                    if (isKeyboard)
                        return "<symbol=y7b_mouse_button_r>";
                    else
                        return "<symbol=button_sankaku>";
                case TutorialButton.Dodge:
                    if (isKeyboard)
                        return "<symbol=y7b_keyboard_space>";
                    else
                        return "<symbol=button_batsu>";
                case TutorialButton.Block:
                    if (isKeyboard)
                        return "<symbol=y7b_keyboard_shift>";
                    else
                        return "<symbol=button_l1>";
                case TutorialButton.Grab:
                    if (isKeyboard)
                        return "<symbol=y7b_keyboard_e>";
                    else
                        return "<symbol=button_maru>";
                
            }

            return "(INVALID_BUTTON!)";
        }
    }
}

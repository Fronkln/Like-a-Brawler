using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Brawler.Mod;
using YazawaCommand;
using DragonEngineLibrary;
using System.CodeDom;

namespace Brawler
{
    public static class ModInput
    {
        public static Dictionary<AttackInputID, KeyInfo> Input = new Dictionary<AttackInputID, KeyInfo>();

        public static bool KeyboardPausePressed = false;

        static ModInput()
        {
            Input = new Dictionary<AttackInputID, KeyInfo>();
            Array values = Enum.GetValues(typeof(AttackInputID));

            for (int i = 0; i < values.Length; i++)
            {
                Input[(AttackInputID)i] = new KeyInfo();
            }

            Input[AttackInputID.LightAttack].Key = VirtualKey.LeftButton;
            Input[AttackInputID.HeavyAttack].Key = VirtualKey.RightButton;
            Input[AttackInputID.Sway].Key = VirtualKey.Space;
            Input[AttackInputID.Guard].Key = VirtualKey.LeftShift;
            Input[AttackInputID.ExtremeHeat].Key = VirtualKey.Q;
            Input[AttackInputID.Grab].Key = VirtualKey.E;
            Input[AttackInputID.LockOn].Key = VirtualKey.MiddleButton;
        }

        public static void InputUpdate()
        {
            //Keyboard only;
            if (!Mod.IsGamePaused)
            {
                if(DragonEngine.IsKeyDown(VirtualKey.Escape))
                    KeyboardPausePressed = true;
            }
            else
                KeyboardPausePressed = false;
        }

        public static void GameUpdate()
        {
            foreach (var kv in Input)
            {
                kv.Value.Held = Holding(kv.Key);

                if (kv.Value.Held)
                    kv.Value.TimeHeld += DragonEngine.deltaTime;
                else
                    kv.Value.TimeHeld = 0;
            }
        }

        public static void Clear()
        {
            foreach (var kv in Input)
            {
                if (kv.Value.Pressed)
                    kv.Value.Pressed = false;
            }
        }

        public static bool JustPressed(AttackInputID id)
        {
            return BattleManager.PadInfo.CheckCommand(GetBattleButtonForAttackInput(id), 0, 0, 1); // BattleManager.PadInfo.IsJustPush(GetBattleButtonForAttackInput(id));
        }

        public static bool IsTimingPush(AttackInputID id, uint tick)
        {
            return BattleManager.PadInfo.IsTimingPush(GetBattleButtonForAttackInput(id), tick);
        }

        public static bool IsTimingPush(AttackInputID id, float sec)
        {
            return IsTimingPush(id, (uint)(sec * 1000));
        }

        public static bool Holding(AttackInputID id)
        {
                BattleButtonID btn = GetBattleButtonForAttackInput(id);
            return Holding(btn);
        }

        public static bool Holding(BattleButtonID btn)
        {
            return !BattleManager.PadInfo.CheckCommand(btn, 0, 0) && BattleManager.PadInfo.CheckCommand(btn, 1, 0);
        }

        public static float TimeHeld(AttackInputID id)
        {
            return Input[id].TimeHeld;
        }

        public static AttackInputID GetAttackInputForBattleButton(BattleButtonID id)
        {
            switch (id)
            {
                default:
                    DragonEngine.Log("invalid input " + id.ToString());
                    return AttackInputID.LightAttack;
                case BattleButtonID.light:
                    return AttackInputID.LightAttack;
                case BattleButtonID.heavy:
                    return AttackInputID.HeavyAttack;
                case BattleButtonID.sway:
                    return AttackInputID.Sway;
                case BattleButtonID.action:
                    return AttackInputID.Grab;
                case BattleButtonID.guard:
                    return AttackInputID.Guard;
                case BattleButtonID.shift:
                    return AttackInputID.LockOn;
                case BattleButtonID.run:
                    return AttackInputID.ExtremeHeat;
            }
        }

        public static BattleButtonID GetBattleButtonForAttackInput(AttackInputID id)
        {
            switch (id)
            {
                default:
                    DragonEngine.Log("invalid input " + id.ToString());
                    return BattleButtonID.light;
                case AttackInputID.LightAttack:
                    return BattleButtonID.light;
                case AttackInputID.HeavyAttack:
                    return BattleButtonID.heavy;
                case AttackInputID.Sway:
                    return BattleButtonID.sway;
                case AttackInputID.Grab:
                    return BattleButtonID.action;
                case AttackInputID.Guard:
                    return BattleButtonID.guard;
                case AttackInputID.LockOn:
                    return BattleButtonID.shift;
                case AttackInputID.ExtremeHeat:
                    return BattleButtonID.run;
            }
        }
    }

    public class KeyInfo
    {
        public VirtualKey Key;
        public bool Pressed;
        public bool Held;

        public float TimeHeld = 0;
        public float LastTimeSincePressed = 999999999;
    }
}

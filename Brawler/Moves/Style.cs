using DragonEngineLibrary;
using YazawaCommand;

namespace Brawler
{
    [System.Serializable]
    public class Style
    {
        public MotionID SwapAnimation;
        public YFC CommandSet;

        public Style(MotionID swapanim, YFC moveset)
        {
            SwapAnimation = swapanim;
            CommandSet = moveset;
        }
    }
}

namespace YazawaCommand
{
    [Serializable]
    public class GameTick
    {
        public uint Tick { get; set; }
        public float Frame
        {
            get { return TickToFrame(Tick); }
            set { Tick = FrameToTick(value); }
        }

        ///<summary>Convert Dragon Engine tick to frame.</summary>
        public static float TickToFrame(uint tick)
        {
            return (tick / 3000.0f) * 30.0f;
        }

        ///<summary>Convert frame to Dragon Engine tick </summary>
        public static uint FrameToTick(float frame)
        {
            return (uint)((frame * 3000.0f) / 30.0f);
        }

        public GameTick()
        {

        }
        public GameTick(uint tick)
        {
            Tick = tick;
        }

        public GameTick(float frame)
        {
            Frame = frame;
        }

        public static implicit operator uint(GameTick tick)
        {
            return tick.Tick;
        }

        public static implicit operator float(GameTick tick)
        {
            return tick.Frame;
        }
    }
}

namespace Brawler
{
    [System.Flags]
    public enum AttackFlags : int
    {
        None,
        GuardBreak = 1 << 31,
    }
}

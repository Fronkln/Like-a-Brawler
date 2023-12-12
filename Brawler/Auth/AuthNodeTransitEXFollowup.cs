using System;
using System.Linq;
using System.Runtime.InteropServices;
using YazawaCommand;

namespace Brawler.Auth
{
    //60010: Brawler Player Transit EX Followup
    internal class AuthNodeTransitEXFollowup
    {
        public static unsafe void Play(IntPtr thisObj, uint tick, IntPtr mtx, IntPtr unk)
        {
            IntPtr yhcNamePtr = (IntPtr)(thisObj.ToInt64() + 52);
            IntPtr yhcAttackNamePtr = (IntPtr)(thisObj.ToInt64() + 84);

            string yhcName = Marshal.PtrToStringAnsi(yhcNamePtr);
            string yhcAttackName = Marshal.PtrToStringAnsi(yhcAttackNamePtr);
            uint jobRestriction = *(uint*)(thisObj.ToInt64() + 116);
            bool extremeHeatOnly = *(uint*)(thisObj.ToInt64() + 120) > 0;

            YHC set = YazawaCommandManager.GetYHCByName(yhcName);

            if (set == null)
                return;

            HeatActionAttack attack = set.Attacks.FirstOrDefault(x => x.Name == yhcAttackName);

            if (attack == null)
                return;

            HeatActionInformation inf = HeatActionSimulator.CheckSingle(BrawlerBattleManager.Kasuga, attack);

            if (inf == null)
                return;

            HeatActionManager.ExecHeatAction(inf);
        }
    }
}

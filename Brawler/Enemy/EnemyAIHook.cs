using DragonEngineLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Brawler
{
    internal static class EnemyAIHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe delegate bool EnemyAIChooseCustomAttack(IntPtr btlAiPtr, IntPtr selectCommandInfo);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe delegate bool EnemyAIChooseCustomAttack_Brawler(IntPtr btlAiPtr, IntPtr selectCommandInfo);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe delegate bool EnemyAISetCommand(IntPtr btlAiPtr, IntPtr selectCommandInfo, RPGSkillID skill);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public unsafe delegate IntPtr EnemyAISelectNormalCommandTarget(IntPtr btlAiPtr, ref uint outTarget, IntPtr rpgCommand);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public unsafe delegate void FighterCommandRefSet(IntPtr refF, IntPtr fighter_command);
    }
}

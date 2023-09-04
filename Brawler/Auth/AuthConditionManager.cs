using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DragonEngineLibrary;

namespace Brawler.Auth
{
    internal static class AuthConditionManager
    {
        private delegate bool ConditionDeleg(IntPtr dat, IntPtr node);
        private static List<ConditionDeleg> _condDelegates = new List<ConditionDeleg>();

        //TODO: EXTENSIONS/EX AUTH CONDITION WAS BROKEN FOR DEVILLEON! NOT GOOD!
        [DllImport("EX Auth Condition.asi", EntryPoint = "EX_AUTH_COND_REGISTER_CONDITION", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool _RegisterPrivate(uint id, IntPtr func);


        public static void Init()
        {
            Register(133700001, ConditionFolderPlayerJob.CheckDisabled);
        }


        public static bool Register(uint id, Func<IntPtr, IntPtr, bool> checkFunc)
        {
            ConditionDeleg del = new ConditionDeleg(checkFunc);
            _condDelegates.Add(del);

            return _RegisterPrivate(id, Marshal.GetFunctionPointerForDelegate(del));
        }
    }
}

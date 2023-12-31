﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Brawler.Auth;
using DragonEngineLibrary;

namespace Brawler
{
    internal unsafe static class AuthCustomNodeManager
    {
        private unsafe delegate void PlayDeleg(IntPtr thisObj, uint tick, IntPtr mtx, IntPtr unk);
        private static List<PlayDeleg> _playDelegates = new List<PlayDeleg>();

        //TODO: EXTENSIONS/EX AUTH CONDITION WAS BROKEN FOR DEVILLEON! NOT GOOD!
        [DllImport("mods/EX Auth/EXAuth.asi", EntryPoint = "RegisterNewNode", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool _RegisterNewNode(uint id);

        [DllImport("mods/EX Auth/EXAuth.asi", EntryPoint = "RegisterPlayFunc", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool _RegisterPlayFunc(uint id, IntPtr func);

        [DllImport("mods/EX Auth/EXAuth.asi", EntryPoint = "RegisterPlayFirstFunc", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool _RegisterPlayFirstFunc(uint id, IntPtr func);

        [DllImport("EXAuth.asi", EntryPoint = "RegisterPlayLastFunc", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool _RegisterPlayLastFunc(uint id, IntPtr func);

        public static bool RegisterNewNode(uint id)
        {
            return _RegisterNewNode(id);
        }

        public static bool RegisterPlayFunc(uint id, Action<IntPtr, uint, IntPtr, IntPtr> func)
        {
            PlayDeleg del = new PlayDeleg(func);
            _playDelegates.Add(del);

            return _RegisterPlayFunc(id, Marshal.GetFunctionPointerForDelegate(del));
        }

        public static bool RegisterPlayFirstFunc(uint id, Action<IntPtr, uint, IntPtr, IntPtr> func)
        {
            PlayDeleg del = new PlayDeleg(func);
            _playDelegates.Add(del);

            return _RegisterPlayFirstFunc(id, Marshal.GetFunctionPointerForDelegate(del));
        }

        public static bool RegisterPlayLastFunc(uint id, Action<IntPtr, uint, IntPtr, IntPtr> func)
        {
            PlayDeleg del = new PlayDeleg(func);
            _playDelegates.Add(del);

            return _RegisterPlayFirstFunc(id, Marshal.GetFunctionPointerForDelegate(del));
        }

        public static void Init()
        {
            RegisterNewNode(81);
            RegisterPlayFunc(82, AuthNodeBrawlerDrop.Play);

            RegisterNewNode(82);
            RegisterPlayFunc(82, AuthNodeBrawlerThrow.Play);

            RegisterNewNode(60010);
            RegisterPlayFunc(60010, AuthNodeTransitEXFollowup.Play);
        }
    }
}

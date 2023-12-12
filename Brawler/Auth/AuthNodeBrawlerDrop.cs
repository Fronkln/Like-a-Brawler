using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonEngineLibrary;

namespace Brawler.Auth
{
    //82: Weapon Throw
    internal unsafe static class AuthNodeBrawlerDrop
    {
        public static void Play(IntPtr thisObj, uint tick, IntPtr mtx, IntPtr unk)
        {
            Character chara = new EntityHandle<Character>(*(uint*)unk).Get();
            chara.GetFighter().DropWeapon(new DropWeaponOption(AttachmentCombinationID.right_weapon, false));
        }
    }
}

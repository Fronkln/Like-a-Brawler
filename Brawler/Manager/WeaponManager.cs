using System;
using System.Collections.Generic;
using DragonEngineLibrary;
using DragonEngineLibrary.Service;
using YazawaCommand;

namespace Brawler
{
    public static class WeaponManager
    {
        public static Dictionary<AssetArmsCategoryID, YFC> WeaponMovesets = new Dictionary<AssetArmsCategoryID, YFC>();
        public static float WeaponTime = 0;


        private static DETask m_throwProcedure;

        public static bool InputUpdate(AssetUnit weapon)
        {
            //Don't throw job weapon
            if (BrawlerPlayer.IsEXGamer)
                return false;

            if (!weapon.IsValid())
                return false;

            if (!BrawlerPlayer.ThrowingWeapon && !BrawlerBattleManager.KasugaChara.HumanModeManager.IsPickup() && ModInput.JustPressed(AttackInputID.Grab))
            {
                BrawlerPlayer.ThrowWeapon();
                return true;
            }

            return false;
        }

        public static bool Update(AssetUnit weapon)
        {
            if (BrawlerPlayer.IsEXGamer)
                return false;

            AssetID assetID = weapon.AssetID;
            AssetArmsCategoryID category = Asset.GetArmsCategory(assetID);

            if (assetID == AssetID.invalid)
            {
                WeaponTime = 0;
                return false;
            }

            WeaponTime += DragonEngine.deltaTime;

            Fighter kasuga = BrawlerBattleManager.Kasuga;
            bool wepMovesetExists = WeaponMovesets.ContainsKey(category);

            //dont have any movesets for this weapon, drop it
            if (!wepMovesetExists)
            {
#if !DEBUG
                DragonEngine.Log("Don't have moveset for arms category " + category + ", dropping");
                kasuga.DropWeapon(new DropWeaponOption(AttachmentCombinationID.right_weapon, false));
                return false;
#else
                DragonEngine.Log("Don't have moveset for arms category " + category + ". Will not drop on debug");
#endif
            }

            if (BrawlerPlayer.IsEXGamer)
                return false;

            if (!weapon.IsValid())
                return false;

            return false;
        }

        public static void InitWeaponMovesets()
        {
            /*
            //sword
            Moveset wepSetE = new Moveset()
            {

            };

            //hammer
            Moveset wepSetG = new Moveset
            (
                RPGSkillID.invalid,
                new MoveCFC(new MoveCFC.AttackFrame[]
                {
                    new MoveCFC.AttackFrame(new FighterCommandID(1339, 1), false, 1),
                    new MoveCFC.AttackFrame(new FighterCommandID(1339, 2), false, 0.8f),
                    new MoveCFC.AttackFrame(new FighterCommandID(1339, 3), false, 0.8f, 0.6f),

                }, new AttackInput[]
                {
                    new AttackInput(AttackInputID.LeftMouse, false)
                }, AttackConditionType.NotDown)
            );

            //medium object
            Moveset wepSetN = new Moveset
            (
                RPGSkillID.invalid,
                new MoveCFC(new MoveCFC.AttackFrame[]
                {
                    new MoveCFC.AttackFrame(new FighterCommandID(1347, 1), false, 1),
                    new MoveCFC.AttackFrame(new FighterCommandID(1347, 2), true, 0.8f),
                    new MoveCFC.AttackFrame(new FighterCommandID(1347, 3), false, 0.8f, 0.6f),

                }, new AttackInput[]
                {
                    new AttackInput(AttackInputID.LeftMouse, false)
                }, AttackConditionType.NotDown)
            );

            Moveset wepSetY = new Moveset
            (
                RPGSkillID.invalid,
                new MoveCFC(new MoveCFC.AttackFrame[]
                {
                    new MoveCFC.AttackFrame(new FighterCommandID(1342, 1), false, 0.5f, 0.6f),

                }, new AttackInput[]
                {
                    new AttackInput(AttackInputID.LeftMouse, false)
                }, AttackConditionType.NotDown)
            );

            Moveset wepSetQ = new Moveset
            (
                RPGSkillID.invalid,
                new MoveCFC(new MoveCFC.AttackFrame[]
                {
                    new MoveCFC.AttackFrame(new FighterCommandID(1343, 1), false, 1.5f),

                }, new AttackInput[]
                {
                    new AttackInput(AttackInputID.LeftMouse, false)
                }, AttackConditionType.NotDown)
            );

            WeaponMovesets[AssetArmsCategoryID.A] = wepSetA;
            WeaponMovesets[AssetArmsCategoryID.C] = wepSetC;
            WeaponMovesets[AssetArmsCategoryID.D] = wepSetD;
            WeaponMovesets[AssetArmsCategoryID.E] = wepSetE;
            WeaponMovesets[AssetArmsCategoryID.G] = wepSetG;
            WeaponMovesets[AssetArmsCategoryID.H] = wepSetH;
            WeaponMovesets[AssetArmsCategoryID.N] = wepSetN;
            WeaponMovesets[AssetArmsCategoryID.Y] = wepSetY;
            WeaponMovesets[AssetArmsCategoryID.Q] = wepSetQ;
            */

            WeaponMovesets[AssetArmsCategoryID.A] = Mod.ReadYFC("kasuga_wpa.yfc");
            WeaponMovesets[AssetArmsCategoryID.B] = Mod.ReadYFC("kasuga_wpb.yfc");
            WeaponMovesets[AssetArmsCategoryID.C] = Mod.ReadYFC("kasuga_wpc.yfc");
            WeaponMovesets[AssetArmsCategoryID.D] = Mod.ReadYFC("kasuga_wpd.yfc");
            WeaponMovesets[AssetArmsCategoryID.E] = Mod.ReadYFC("kasuga_wpe.yfc");
            WeaponMovesets[AssetArmsCategoryID.F] = Mod.ReadYFC("kasuga_wpf.yfc");
            WeaponMovesets[AssetArmsCategoryID.G] = Mod.ReadYFC("kasuga_wpg.yfc");
            WeaponMovesets[AssetArmsCategoryID.N] = Mod.ReadYFC("kasuga_wpn.yfc");
            WeaponMovesets[AssetArmsCategoryID.H] = Mod.ReadYFC("kasuga_wph.yfc");
        }
    }
}

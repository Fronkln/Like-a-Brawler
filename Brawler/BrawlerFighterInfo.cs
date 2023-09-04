using System.Collections.Generic;
using DragonEngineLibrary;

namespace Brawler
{
    public struct BrawlerFighterInfo
    {
        public Fighter Fighter;

        public static Dictionary<uint, BrawlerFighterInfo> Infos = new Dictionary<uint, BrawlerFighterInfo>();

        public bool IsDead;
        public bool IsFlinching;
        public bool IsSync;
        public bool IsDown;
        public bool IsFaceDown;
        public bool IsGettingUp;
        public bool IsRagdoll;
        public bool IsMove;
        public bool IsAttack;

        public float MoveTime;

        public ECAssetArms RightWeapon;
        public ECAssetArms LeftWeapon;


        //Purpose: Cache fighter variables
        //Reduces PInvoke(probably) and eliminates several crashes
        //Related to accesing those vars in input loop
        public void Update(Fighter fighter)
        {
            Fighter = fighter;

            if (fighter == null || !fighter.Character.IsValid())
            {
                Infos.Remove(fighter.Character.UID);
                return;
            }

            BattleFighterInfo inf = fighter.GetInfo();


            IsDead = fighter.IsDead();
            IsFlinching = fighter.Character.HumanModeManager.IsDamage();
            IsSync = fighter.IsSync();
            IsDown = fighter.IsDown();
            IsFaceDown = fighter.IsFaceDown();
            IsGettingUp = fighter.Character.HumanModeManager.IsStandup();
            IsRagdoll = inf.is_ragdoll_;//fighter.Character.IsRagdoll();
            IsMove = fighter.Character.HumanModeManager.IsMove();
            RightWeapon = fighter.GetWeapon(AttachmentCombinationID.right_weapon).Unit.Get().Arms;
            LeftWeapon = fighter.GetWeapon(AttachmentCombinationID.left_weapon).Unit.Get().Arms;
            IsAttack = fighter.Character.HumanModeManager.IsAttack();

            if (IsMove)
                MoveTime += DragonEngine.deltaTime;
            else
                MoveTime = 0;

            Infos[fighter.Character.UID] = this;
        }

        /// <summary>
        /// We are either down, dead, ragdolled, in sync, getting up or flinching in pain.
        /// </summary>
        /// <returns></returns>
        public bool CantAttackOverall()
        { 
            return IsDead || IsFlinching || IsSync || IsDown || IsGettingUp || IsRagdoll;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public enum HeatActionConditionType
    {
        Invalid = 0,
        CanAttackGeneric,
        Down,
        GettingUp,
        CriticalHP,
        Flinching,
        Moving,
        CharacterLevel,
        AssetID,
        WeaponType,
        WeaponSubtype,
        Distance,
        DistanceToHactPosition,
        Grabbing,
        StageID,
        JobLevel,
        Job,
        EXHeat,
        FacingTarget,
        Running,
        BattleStance,
        DistanceToNearestAsset,
        NearestAssetSpecialType,
        FacingRange,
        DistanceToRange,
        Attacking,
        NearestAssetFlag,
        DownOrGettingUp,
        InRange,
        IsBoss,
        Health,
        CtrlType,
        SoldierID,
        BattleConfigID,
        WouldDieInHAct,
        MotionID,
        InBepElementRange,
        BeingJuggled
    }
}

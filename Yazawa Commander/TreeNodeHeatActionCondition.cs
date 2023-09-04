using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using YazawaCommand;

namespace Yazawa_Commander
{
    public class TreeNodeHeatActionCondition : TreeNodeYHC
    {
        public HeatActionCondition Condition;
       
        public TreeNodeHeatActionCondition()
        {

        }
        
        public TreeNodeHeatActionCondition(HeatActionCondition condition)
        {
            Condition = condition;
            Update();
        }

        public override object Clone()
        {
            TreeNodeHeatActionCondition cloned = (TreeNodeHeatActionCondition)base.Clone();
            cloned.Condition = Condition.Copy();

            return cloned;
        }

        public override void Update()
        {
            base.Update();

            Text = GetName();
            ContextMenuStrip = Main.Instance.heatActionConditionContext;

            ImageIndex = 7;
            SelectedImageIndex = 7;
        }

        public string GetName()
        {
            string name = "";

            if (Condition.LogicalOperator == LogicalOperator.FALSE)
                name += "NOT ";

            switch(Condition.Type)
            {
                default:
                    name += Condition.Type;
                    break;

                case HeatActionConditionType.CanAttackGeneric:
                    name += "Can Attack (Generic)";
                    break;
                case HeatActionConditionType.Invalid:
                    name += "Invalid Condition";
                    break;
                case HeatActionConditionType.CriticalHP:
                    name += "Critical Health";
                    break;
                case HeatActionConditionType.GettingUp:
                    name += "Getting Up";
                    break;
                case HeatActionConditionType.Down:
                    name += "Down";
                    break;
                case HeatActionConditionType.Moving:
                    name += "Moving";
                    break;
                case HeatActionConditionType.CharacterLevel:
                    name += $"Character Level {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1U32}";
                    break;
                case HeatActionConditionType.Job:
                    name += $"Character Job {(RPGJobID)Condition.Param1U32}";
                    break;
                case HeatActionConditionType.JobLevel:
                    name += $"Job Level {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1U32}";
                    break;
                case HeatActionConditionType.Grabbing:
                    name += "Grabbing";
                    break;
                case HeatActionConditionType.DistanceToHactPosition:
                    name += $"Distance to HAct Position {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1F}";
                    break;
                case HeatActionConditionType.Distance:
                    if(!Condition.Param1B)
                        name += $"Distance to {((HeatActionActorType)Condition.Param1U32).ToString()} {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1F}";
                    else
                        name += $"Distance to {((HeatActionActorType)Condition.Param1U32).ToString()} Around {Condition.Param1F}, {Condition.Param2F}";
                    break;
                case HeatActionConditionType.WeaponType:

                    if (Condition.Param1U32 == 0)
                    {
                        if (Condition.LogicalOperator == LogicalOperator.GREATER)
                            name += "Has Weapon";
                        else
                            name += "Unarmed";
                    }
                    else
                        name += "Weapon Type " + ((AssetArmsCategoryID)Condition.Param1U32).ToString();
                    break;
                case HeatActionConditionType.WeaponSubtype:
                        name += "Weapon Subtype " + (Condition.Param1U32).ToString();
                    break;
                case HeatActionConditionType.EXHeat:
                    name += "Extreme Heat";
                    break;
                case HeatActionConditionType.FacingTarget:
                    name += $"Facing {((HeatActionActorType)Condition.Param1U32).ToString()}";
                    break;
                case HeatActionConditionType.BattleStance:
                    name += "Battle Stance";
                    break;
                case HeatActionConditionType.Running:
                    name += "Running";
                    break;
                case HeatActionConditionType.DistanceToNearestAsset:
                    name += $"Distance to Nearest Asset {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1F}";
                    break;
                case HeatActionConditionType.NearestAssetSpecialType:
                    name += $"Nearest Asset Is Special ({Condition.Param1U32})";
                    break;
                case HeatActionConditionType.FacingRange:
                    name += $"Facing Range {(HeatActionRangeType)Condition.Param1U32}";
                    break;
                case HeatActionConditionType.DistanceToRange:
                    name += $"Distance to Range {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1F}";
                    break;
                case HeatActionConditionType.Attacking:
                    name += "Attacking";
                    break;
                case HeatActionConditionType.NearestAssetFlag:
                    name += "Nearest Asset Flag: " + ((NearestAssetFlagType)Condition.Param1U32).ToString().SplitByCase();
                    break;
                case HeatActionConditionType.DownOrGettingUp:
                    name += "Down Or Getting Up";
                    break;
                case HeatActionConditionType.InRange:
                    name += "In Range: " + (HeatActionRangeType)Condition.Param1U32;
                    break;
                case HeatActionConditionType.IsBoss:
                    name += "Is Boss";
                    break;
                case HeatActionConditionType.Health:

                    if (Condition.Param1B)
                        name += $"Health Percentage {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1F}%";
                    else
                        name += $"Health Value {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1U64} ";

                    break;
                case HeatActionConditionType.CtrlType:
                    name += $"Control Type {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1U32}";
                    break;
                case HeatActionConditionType.SoldierID:
                    name += $"Soldier ID {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1U32}";
                    break;
                case HeatActionConditionType.BattleConfigID:
                    name += $"Battle Config ID {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1U32}";
                    break;
                case HeatActionConditionType.WouldDieInHAct:
                    name += $"Would Die To Damage {Condition.Param1U32}";
                    break;
                case HeatActionConditionType.MotionID:
                    name += $"Motion ID {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1U32}";
                    break;
                case HeatActionConditionType.InBepElementRange:
                    name += $"BEP In Range Of Element {Condition.Param1U32}";
                    break;
            }

            return name;
        }

        public string GetLogicalOperatorSymbol(LogicalOperator _operator)
        {
            switch (_operator)
            {
                default:
                    return "";
                case LogicalOperator.FALSE:
                    return "!=";
                case LogicalOperator.TRUE:
                    return "==";
                case LogicalOperator.NOT_EQUALS:
                    return "!=";
                case LogicalOperator.EQUALS_GREATER:
                    return ">=";
                case LogicalOperator.EQUALS_LESS:
                    return "<=";
            }
        }
    }
}

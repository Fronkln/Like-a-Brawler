using System.Windows.Forms;
using YazawaCommand;

namespace Yazawa_Commander
{
    public class TreeNodeCondition : TreeNodeYFC
    {
        public AttackCondition Condition;

        public TreeNodeCondition()
        {

        }

        public TreeNodeCondition(AttackCondition condition)
        {
            Condition = condition;
            Update();

            ContextMenuStrip = Main.Instance.conditionContext;
        }

        public override object Clone()
        {
            TreeNodeCondition cond = (TreeNodeCondition)base.Clone();
            cond.Condition = (AttackCondition)Condition.Copy();

            return cond;
        }

        public override void Update()
        {
            base.Update();

            Text = GetName();

            ImageIndex = 7;
            SelectedImageIndex = 7;

            if (Condition.Type == AttackConditionType.InputKey)
            {
                switch ((AttackInputID)Condition.Param1U32)
                {
                    case AttackInputID.LightAttack:
                        ImageIndex = 1;
                        SelectedImageIndex = 1;
                        break;
                    case AttackInputID.HeavyAttack:
                        ImageIndex = 2;
                        SelectedImageIndex = 2;
                        break;
                }
            }
        }

        public string GetName()
        {
            string name = "";

            if (Condition.LogicalOperator == LogicalOperator.FALSE)
                name += "NOT ";

            switch (Condition.Type)
            {
                default:
                    name += Condition.Type.ToString();
                    break;
                case AttackConditionType.Down:
                    name += "Player Down";
                    break;
                case AttackConditionType.LowHealth:
                    name += "Low Health";
                    break;
                case AttackConditionType.LockedEnemyDown:
                    name += "Locked Enemy Down";
                    break;
                case AttackConditionType.GettingUp:
                    name += "Player Getting Up";
                    break;
                case AttackConditionType.InputKey:
                    name += "Attack Input";
                    break;
                case AttackConditionType.CharacterLevel:
                    name += "Character Level " + GetLogicalOperatorSymbol(Condition.LogicalOperator) + $" {Condition.Param1U32}";
                    break;
                case AttackConditionType.CharacterJobLevel:
                    name += "Character Job Level " + GetLogicalOperatorSymbol(Condition.LogicalOperator) + $" {Condition.Param1U32}";
                    break;
                case AttackConditionType.Running:
                    name += "Running";
                    break;
                case AttackConditionType.AnimationOver:
                    name += "Animation Over";
                    break;
                case AttackConditionType.CanAttackOverall:
                    name += "Can Attack (Generic)";
                    break;
                case AttackConditionType.AttackHit:
                    name += "Attack Hit";
                    break;
                case AttackConditionType.IsFlinching:
                    name += "Flinching";
                    break;
                case AttackConditionType.LockedToEnemy:
                    name += "Battle Stance";
                    break;
                case AttackConditionType.IsExtremeHeat:
                    name += "Extreme Heat";
                    break;
                case AttackConditionType.Sync:
                    name += "Sync";
                    break;
                case AttackConditionType.EnemyResponse:
                    name += "AI Response " + (EnemyAIResponse)Condition.Param1U32;
                    break;
                case AttackConditionType.SyncType:
                    name += "Sync Type: " + (AttackSyncType)Condition.Param1U32;
                    break;
                case AttackConditionType.SyncDirection:
                    name += "Sync Type: " + (AttackSyncDirection)Condition.Param1U32;
                    break;

                case AttackConditionType.FacingRange:
                    name += "Facing Range " + (HeatActionRangeType)Condition.Param1U32;
                    break;

                case AttackConditionType.DistanceToRange:
                    name += $"Distance to Range {GetLogicalOperatorSymbol(Condition.LogicalOperator)} {Condition.Param1F}";
                    break;

                case AttackConditionType.NearestEnemyFlag:
                    name += $"Nearest Enemy Flag: {(NearestEnemyFlag)Condition.Param1U32}";
                    break;

            }

            return name;
        }

        public string GetLogicalOperatorSymbol(LogicalOperator _operator)
        {
            switch(_operator)
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

using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using YazawaCommand;

namespace Yazawa_Commander
{
    public partial class Main : Form
    {
        public static Main Instance;

        private TreeNode m_selectedNode = null;
        private TreeNode m_rightClickedNode = null;
        private TreeNode m_copiedNode = null;
        int rowCount = 1;

        private string m_filePath = "";

        private bool m_isYFC = true;

        public Main()
        {
            InitializeComponent();
            Instance = this;

            CreateHeader("Test");
        }


        public void NewFile()
        {
            Clean();
            m_filePath = "";
            m_isYFC = true;

            attacksTree.Nodes.Add(new TreeNodeAttackGroup(new AttackGroup() { Name = "Start" }));
        }

        private void newYHCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clean();
            m_filePath = "";
            m_isYFC = false;

            attacksTree.Nodes.Add(new TreeNodeHeatActionAttack(new HeatActionAttack() { Name = "HACT_000" }));
        }

        public void Clean()
        {
            attacksTree.SuspendLayout();

            attacksTree.Nodes.Clear();

            attacksTree.ResumeLayout();
        }

        public void CleanVarPanel(bool suspend = false)
        {
            varPanel.SuspendLayout();

            varPanel.Controls.Clear();
            varPanel.RowStyles.Clear();
            varPanel.ColumnStyles.Clear();
            rowCount = 1;

            varPanel.ResumeLayout();
        }

        private void attacksTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            DrawNode(e.Node);
        }

        private void DrawNode(TreeNode node)
        {
            
            CleanVarPanel();


            varPanel.SuspendLayout();

            if (node is TreeNodeAttackGroup)
                DrawAttackGroup(node as TreeNodeAttackGroup);
            else if (node is TreeNodeAttack)
                DrawAttack(node as TreeNodeAttack);
            else if (node is TreeNodeCondition)
                DrawCondition(node as TreeNodeCondition);
            if (node is TreeNodeHeatActionAttack)
                DrawHactAttack(node as TreeNodeHeatActionAttack);
            else if (node is TreeNodeHeatActionActor)
                DrawHactActor(node as TreeNodeHeatActionActor);
            else if (node is TreeNodeHeatActionCondition)
                DrawHactCondition(node as TreeNodeHeatActionCondition);


            varPanel.ResumeLayout();
        }

        private void DrawAttackGroup(TreeNodeAttackGroup group)
        {
            TextBox nameField = null;

            CreateHeader("Attack Group");
            nameField = CreateInput("Name", group.Group.Name, null);
            nameField.TextChanged += delegate { group.Group.Name = nameField.Text; group.Update(); };
        }

        private void DrawAttack(TreeNodeAttack attack)
        {
            TextBox nameField = null;

            CreateHeader("Attack");
            nameField = CreateInput("Name", attack.Attack.Name, null);
            nameField.TextChanged += delegate { attack.Attack.Name = nameField.Text; attack.Update(); };

            if (m_isYFC)
            {

                List<string> groups = new List<string>();

                groups.Add("None");

                foreach (TreeNodeAttackGroup group in attacksTree.Nodes)
                    groups.Add(group.Group.Name);

                CreateComboBox("Transition", attack.Attack.TransitionGroup + 1, groups.ToArray(), delegate (int idx) { attack.Attack.TransitionGroup = idx - 1; });
                CreateInput("Cooldown", attack.Attack.Cooldown.ToString(), delegate (string val) { attack.Attack.Cooldown = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
            }

            switch (attack.Attack.AttackType)
            {
                case AttackType.MoveCFC:
                    AttackCFC cfcAtk = attack.Attack as AttackCFC;

                    CreateInput("CFC Moveset ID", cfcAtk.MovesetID.ToString(), delegate (string val) { cfcAtk.MovesetID = ushort.Parse(val); }, NumberBox.NumberMode.Ushort);
                    CreateInput("CFC Attack Index", cfcAtk.Index.ToString(), delegate (string val) { cfcAtk.Index = ushort.Parse(val); }, NumberBox.NumberMode.Ushort);

                    break;
                case AttackType.MoveRPG:
                    AttackRPG rpgAtk = attack.Attack as AttackRPG;
                    CreateInput("Skill ID", rpgAtk.ID.ToString(), delegate (string val) { rpgAtk.ID = uint.Parse(val); }, NumberBox.NumberMode.UInt);
                    break;
                case AttackType.MoveGMTOnly:
                    AttackGMT motionAtk = attack.Attack as AttackGMT;
                    CreateInput("Motion ID", motionAtk.MotionID.ToString(), delegate (string val) { motionAtk.MotionID = uint.Parse(val); }, NumberBox.NumberMode.UInt);
                    break;
                case AttackType.MoveSync:
                    AttackSync syncAtk = attack.Attack as AttackSync;
                    CreateInput("Skill ID", syncAtk.ID.ToString(), delegate (string val) { syncAtk.ID = uint.Parse(val); }, NumberBox.NumberMode.UInt);
                    CreateComboBox("Sync Type", (int)syncAtk.SyncType, Enum.GetNames<AttackSyncType>(), delegate (int idx) { syncAtk.SyncType = (AttackSyncType)idx; });
                    CreateComboBox("Sync Direction", (int)syncAtk.SyncDirection, Enum.GetNames<AttackSyncDirection>(), delegate (int idx) { syncAtk.SyncDirection = (AttackSyncDirection)idx; });
                    CreateComboBox("Sync Category", (int)syncAtk.SyncCategory, Enum.GetNames<AttackSyncCategory>(), delegate (int idx) { syncAtk.SyncCategory = (AttackSyncCategory)idx; });
                    CreateInput("Loop", Convert.ToByte(syncAtk.Loop).ToString(), delegate (string val) { syncAtk.Loop = byte.Parse(val) > 0; });
                    CreateInput("Movement Sync", Convert.ToByte(syncAtk.MoveSync).ToString(), delegate (string val) { syncAtk.MoveSync = byte.Parse(val) > 0; CleanVarPanel(true); DrawAttack(attack); });

                    if (syncAtk.MoveSync)
                    {
                        CreateInput("Movement Speed", syncAtk.MoveSpeed.ToString(), delegate (string val) { syncAtk.MoveSpeed = Utils.InvariantParse(val); });
                        CreateInput("Movement Invert Direction", Convert.ToByte(syncAtk.InvertDirection).ToString(), delegate (string val) { syncAtk.InvertDirection = byte.Parse(val) > 0; });
                    }
                    break;

                case AttackType.MoveCFCRange:
                    AttackCFCRange cfcRangeAtk = attack.Attack as AttackCFCRange;

                    CreateComboBox("Range", (int)cfcRangeAtk.Range, Enum.GetNames<HeatActionRangeType>(), delegate (int idx) { cfcRangeAtk.Range = (HeatActionRangeType)idx; });

                    CreateInput("CFC Moveset ID", cfcRangeAtk.MovesetID.ToString(), delegate (string val) { cfcRangeAtk.MovesetID = ushort.Parse(val); }, NumberBox.NumberMode.Ushort);
                    CreateInput("CFC Attack Index", cfcRangeAtk.Index.ToString(), delegate (string val) { cfcRangeAtk.Index = ushort.Parse(val); }, NumberBox.NumberMode.Ushort);

                    CreateInput("Forward Offset", cfcRangeAtk.OffsetForward.ToString(), delegate (string val) { cfcRangeAtk.OffsetForward = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                    CreateInput("Up Offset", cfcRangeAtk.OffsetUp.ToString(), delegate (string val) { cfcRangeAtk.OffsetUp = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                    CreateInput("Left Offset", cfcRangeAtk.OffsetLeft.ToString(), delegate (string val) { cfcRangeAtk.OffsetLeft = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                    break;
            }

        }

        private void DrawCondition(TreeNodeCondition cond)
        {

            string[] conds = Enum.GetNames<AttackConditionType>();
            Array vals = Enum.GetValues<AttackConditionType>();

            CreateHeader("Condition");

            CreateComboBox("Flag", Array.IndexOf(conds, cond.Condition.Type.ToString()), conds,
                delegate (int idx)
                {
                    cond.Condition.Type = (AttackConditionType)vals.GetValue(idx);
                    cond.Update();
                    CleanVarPanel(true);
                    DrawCondition(cond);
                });

            CreateComboBox("Operator", (int)cond.Condition.LogicalOperator, Enum.GetNames<LogicalOperator>(), delegate (int idx) { cond.Condition.LogicalOperator = (LogicalOperator)idx; cond.Update(); });

            switch (cond.Condition.Type)
            {
                case AttackConditionType.InputKey:
                    CreateComboBox("Input", (int)cond.Condition.Param1U32, Enum.GetNames<AttackInputID>(),
                        delegate (int idx)
                        {
                            cond.Condition.Param1U32 = (uint)idx;
                            cond.Update();
                        });
                    CreateInput("Buffered", Convert.ToInt32(cond.Condition.Param1B).ToString(), delegate (string val) { cond.Condition.Param1B = byte.Parse(val) > 0; }, NumberBox.NumberMode.Byte);
                    CreateInput("Hold", Convert.ToInt32(cond.Condition.Param2B).ToString(), delegate (string val) { cond.Condition.Param2B = byte.Parse(val) > 0; }, NumberBox.NumberMode.Byte);
                    CreateComboBox("Special Type", cond.Condition.Param1I32, new string[] { "None", "Counter Attack Timing", "Battle Tame (Charged Attack)" }, delegate (int idx) { cond.Condition.Param1I32 = idx; cond.Update(); });

                    //Charge
                    if (cond.Condition.Param2B)
                    {
                        CreateInput("Hold Time", cond.Condition.Param1F.ToString(), delegate (string val) { cond.Condition.Param1F = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                    }

                    //Battle Tame
                    if (cond.Condition.Param1I32 == 2)
                    {
                        CreateInput("Tame Start", cond.Condition.Param1F.ToString(), delegate (string val) { cond.Condition.Param1F = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                        CreateInput("Tame End", cond.Condition.Param2F.ToString(), delegate (string val) { cond.Condition.Param2F = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                    }

                    break;

                case AttackConditionType.CharacterLevel:
                    CreateInput("Current Level", cond.Condition.Param1U32.ToString(), delegate (string val) { cond.Condition.Param1U32 = uint.Parse(val); }, NumberBox.NumberMode.UInt);
                    break;

                case AttackConditionType.CharacterJobLevel:
                    CreateInput("Job Level", cond.Condition.Param1U32.ToString(), delegate (string val) { cond.Condition.Param1U32 = uint.Parse(val); }, NumberBox.NumberMode.UInt);
                    break;

                case AttackConditionType.JobID:
                    CreateInput("Job ID ", cond.Condition.Param1U32.ToString(), delegate (string val) { cond.Condition.Param1U32 = uint.Parse(val); }, NumberBox.NumberMode.UInt);
                    break;
                case AttackConditionType.Running:
                    CreateInput("Run Time", cond.Condition.Param1F.ToString(),
                        delegate (string val)
                        {
                            cond.Condition.Param1F = Utils.InvariantParse(val);
                        }, NumberBox.NumberMode.Float);
                    break;
                case AttackConditionType.EnemyResponse:
                    CreateComboBox("Response", (int)cond.Condition.Param1U32, new string[] { "None", "Grabbed Shakeoff" }, delegate (int idx) { cond.Condition.Param1U32 = (uint)idx; });
                    break;
                case AttackConditionType.SyncType:
                    CreateComboBox("Sync Type", (int)cond.Condition.Param1U32, Enum.GetNames<AttackSyncType>(), delegate (int idx) { cond.Condition.Param1U32 = (uint)idx; });
                    break;

                case AttackConditionType.SyncDirection:
                    CreateComboBox("Sync Direction", (int)cond.Condition.Param1U32, Enum.GetNames<AttackSyncDirection>(), delegate (int idx) { cond.Condition.Param1U32 = (uint)idx; });
                    break;

                case AttackConditionType.FacingRange:
                    CreateComboBox("Range", (int)cond.Condition.Param1U32, Enum.GetNames<HeatActionRangeType>(), delegate (int idx) { cond.Condition.Param1U32 = (uint)idx; cond.Update(); });
                    break;

                case AttackConditionType.DistanceToRange:
                    CreateComboBox("Range", (int)cond.Condition.Param1U32, Enum.GetNames<HeatActionRangeType>(), delegate (int idx) { cond.Condition.Param1U32 = (uint)idx; cond.Update(); });
                    CreateInput("Distance", cond.Condition.Param1F.ToString(), delegate (string val) { cond.Condition.Param1F = Utils.InvariantParse(val); cond.Update(); }, NumberBox.NumberMode.Float);
                    break;

            }
        }

        private void DrawHactAttack(TreeNodeHeatActionAttack attack)
        {
            TextBox nameField = null;

            CreateHeader("Attack");
            nameField = CreateInput("Name", attack.Attack.Name, null);
            nameField.TextChanged += delegate { attack.Attack.Name = nameField.Text; attack.Update(); };

            CreateInput("HAct ID", attack.Attack.HactID.ToString(), delegate (string val) { attack.Attack.HactID = uint.Parse(val); }, NumberBox.NumberMode.UInt);
            CreateInput("Flags", ((uint)attack.Attack.Flags).ToString(), delegate (string val) { attack.Attack.Flags = (HeatActionAttackFlags)uint.Parse(val); }, NumberBox.NumberMode.UInt);

            CreateInput("Prefer Hact Position", (Convert.ToByte(attack.Attack.PreferHActPosition)).ToString(), delegate (string val) { attack.Attack.PreferHActPosition = byte.Parse(val) > 0; }, NumberBox.NumberMode.Byte);
            CreateInput("Dont Allow Movement On Linkout", (Convert.ToByte(attack.Attack.DontAllowMovementOnLinkOut)).ToString(), delegate (string val) { attack.Attack.DontAllowMovementOnLinkOut = byte.Parse(val) > 0; }, NumberBox.NumberMode.Byte);
            CreateInput("Followup Node Only", (Convert.ToByte(attack.Attack.IsFollowupOnly)).ToString(), delegate (string val) { attack.Attack.IsFollowupOnly = byte.Parse(val) > 0; }, NumberBox.NumberMode.Byte);

            CreateSpace(25);

            CreateInput("Position X", (attack.Attack.Position[0]).ToString(), delegate (string val) { attack.Attack.Position[0] = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
            CreateInput("Position Y", (attack.Attack.Position[1]).ToString(), delegate (string val) { attack.Attack.Position[1] = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
            CreateInput("Position Z", (attack.Attack.Position[2]).ToString(), delegate (string val) { attack.Attack.Position[2] = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);

            CreateInput("Y Rotation", (attack.Attack.RotationY).ToString(), delegate (string val) { attack.Attack.RotationY = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);

            CreateComboBox("Special Type", (int)attack.Attack.SpecialType, new string[] { "Normal", "Asset Position" }, delegate (int idx) { attack.Attack.SpecialType = (HeatActionSpecialType)idx; });
            CreateComboBox("Range", (int)attack.Attack.Range, Enum.GetNames<HeatActionRangeType>(), delegate (int idx) { attack.Attack.Range = (HeatActionRangeType)idx; });

            CreateInput("Use Matrix (Advanced)", Convert.ToByte(attack.Attack.UseMatrix).ToString(), delegate (string val) { attack.Attack.UseMatrix = byte.Parse(val) > 0; CleanVarPanel(true); DrawHactAttack(attack); });

            if(attack.Attack.UseMatrix)
            {
                CreateInput("Left Direction X", (attack.Attack.Mtx.LeftDirection.x).ToString(), delegate (string val) { attack.Attack.Mtx.LeftDirection.x = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Left Direction Y", (attack.Attack.Mtx.LeftDirection.y).ToString(), delegate (string val) { attack.Attack.Mtx.LeftDirection.y = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Left Direction Z", (attack.Attack.Mtx.LeftDirection.z).ToString(), delegate (string val) { attack.Attack.Mtx.LeftDirection.z = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Left Direction W", (attack.Attack.Mtx.LeftDirection.w).ToString(), delegate (string val) { attack.Attack.Mtx.LeftDirection.w = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);

                CreateInput("Up Direction X", (attack.Attack.Mtx.UpDirection.x).ToString(), delegate (string val) { attack.Attack.Mtx.UpDirection.x = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Up Direction Y", (attack.Attack.Mtx.UpDirection.y).ToString(), delegate (string val) { attack.Attack.Mtx.UpDirection.y = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Up Direction Z", (attack.Attack.Mtx.UpDirection.z).ToString(), delegate (string val) { attack.Attack.Mtx.UpDirection.z = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Up Direction W", (attack.Attack.Mtx.UpDirection.w).ToString(), delegate (string val) { attack.Attack.Mtx.UpDirection.w = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);

                CreateInput("Forward Direction X", (attack.Attack.Mtx.ForwardDirection.x).ToString(), delegate (string val) { attack.Attack.Mtx.ForwardDirection.x = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Forward Direction Y", (attack.Attack.Mtx.ForwardDirection.y).ToString(), delegate (string val) { attack.Attack.Mtx.ForwardDirection.y = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Forward Direction Z", (attack.Attack.Mtx.ForwardDirection.z).ToString(), delegate (string val) { attack.Attack.Mtx.ForwardDirection.z = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Forward Direction W", (attack.Attack.Mtx.ForwardDirection.w).ToString(), delegate (string val) { attack.Attack.Mtx.ForwardDirection.w = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);

                CreateInput("Coordinate X", (attack.Attack.Mtx.Coordinates.x).ToString(), delegate (string val) { attack.Attack.Mtx.Coordinates.x = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Coordinate Y", (attack.Attack.Mtx.Coordinates.y).ToString(), delegate (string val) { attack.Attack.Mtx.Coordinates.y = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Coordinate Z", (attack.Attack.Mtx.Coordinates.z).ToString(), delegate (string val) { attack.Attack.Mtx.Coordinates.z = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
                CreateInput("Coordinate W", (attack.Attack.Mtx.Coordinates.w).ToString(), delegate (string val) { attack.Attack.Mtx.Coordinates.w = Utils.InvariantParse(val); }, NumberBox.NumberMode.Float);
            }
        }

        private void DrawHactActor(TreeNodeHeatActionActor actor)
        {
            CreateHeader("Actor");
            CreateComboBox("Replace Type", (int)actor.Actor.Type, Enum.GetNames<HeatActionActorType>(), delegate (int idx) { actor.Actor.Type = (HeatActionActorType)idx; actor.Update(); DrawNode(actor); });
            CreateInput("Optional", Convert.ToInt32(actor.Actor.Optional).ToString(), delegate (string val) { actor.Actor.Optional = byte.Parse(val) > 0; }, NumberBox.NumberMode.Byte);
        }

        private void DrawHactCondition(TreeNodeHeatActionCondition condition)
        {
            CreateHeader("Condition");
            CreateComboBox("Type", (int)condition.Condition.Type, Enum.GetNames<HeatActionConditionType>(), delegate (int idx) { condition.Condition.Type = (HeatActionConditionType)idx; condition.Update(); DrawNode(condition); });
            CreateComboBox("Operator", (int)condition.Condition.LogicalOperator, Enum.GetNames<LogicalOperator>(), delegate (int idx) { condition.Condition.LogicalOperator = (LogicalOperator)idx; condition.Update(); });

            switch (condition.Condition.Type)
            {
                case HeatActionConditionType.DistanceToHactPosition:
                    CreateInput("Distance", condition.Condition.Param1F.ToString(), delegate (string val) { condition.Condition.Param1F = Utils.InvariantParse(val); condition.Update(); }, NumberBox.NumberMode.Float);
                    break;
                case HeatActionConditionType.Distance:
                    CreateComboBox("Target", (int)condition.Condition.Param1U32, Enum.GetNames<HeatActionActorType>(), delegate (int idx) { condition.Condition.Param1U32 = (uint)idx; condition.Update(); });
                    CreateInput("Range", Convert.ToByte(condition.Condition.Param1B).ToString(), delegate (string val) { condition.Condition.Param1B = byte.Parse(val) > 0; condition.Update(); DrawNode(condition); }, NumberBox.NumberMode.Byte);

                    if (!condition.Condition.Param1B)
                        CreateInput("Distance", condition.Condition.Param1F.ToString(), delegate (string val) { condition.Condition.Param1F = Utils.InvariantParse(val); condition.Update(); }, NumberBox.NumberMode.Float);
                    else
                    {
                        CreateInput("Minimum Distance", condition.Condition.Param1F.ToString(), delegate (string val) { condition.Condition.Param1F = Utils.InvariantParse(val); condition.Update(); }, NumberBox.NumberMode.Float);
                        CreateInput("Maximum Distance", condition.Condition.Param2F.ToString(), delegate (string val) { condition.Condition.Param2F = Utils.InvariantParse(val); condition.Update(); }, NumberBox.NumberMode.Float);
                    }
                    break;
                case HeatActionConditionType.Down:
                    CreateInput("Is Face Down", Convert.ToByte(condition.Condition.Param1B).ToString(), delegate (string val) { condition.Condition.Param1B = byte.Parse(val) > 0; }, NumberBox.NumberMode.Byte);
                    break;
                case HeatActionConditionType.Job:
                    CreateComboBox("Job", (int)condition.Condition.Param1U32, Enum.GetNames<RPGJobID>(), delegate (int idx) { condition.Condition.Param1U32 = (uint)idx; condition.Update(); });
                    break;
                case HeatActionConditionType.CharacterLevel:
                    CreateInput("Character Level", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); condition.Update(); }, NumberBox.NumberMode.UInt);
                    break;
                case HeatActionConditionType.StageID:
                    CreateInput("Stage ID", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); condition.Update(); }, NumberBox.NumberMode.UInt);
                    break;
                case HeatActionConditionType.AssetID:
                    CreateInput("Asset ID", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); condition.Update(); }, NumberBox.NumberMode.UInt);
                    break;
                case HeatActionConditionType.WeaponType:
                    CreateComboBox("Weapon Type", (int)condition.Condition.Param1U32, Enum.GetNames<AssetArmsCategoryID>(), delegate (int idx) { condition.Condition.Param1U32 = (uint)idx; condition.Update(); });
                    break;
                case HeatActionConditionType.WeaponSubtype:
                    CreateInput("Subtype", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); condition.Update(); }, NumberBox.NumberMode.UInt);
                    break;
                case HeatActionConditionType.FacingTarget:
                    CreateComboBox("Target", (int)condition.Condition.Param1U32, Enum.GetNames<HeatActionActorType>(), delegate (int idx) { condition.Condition.Param1U32 = (uint)idx; condition.Update(); });
                    break;
                case HeatActionConditionType.DistanceToNearestAsset:
                    CreateInput("Distance", condition.Condition.Param1F.ToString(), delegate (string val) { condition.Condition.Param1F = Utils.InvariantParse(val); condition.Update(); }, NumberBox.NumberMode.Float);
                    break;
                case HeatActionConditionType.NearestAssetSpecialType:
                    CreateComboBox("Special Type", (int)condition.Condition.Param1U32, new string[] {"None", "Car", "NeedleHolder"}, delegate (int idx) { condition.Condition.Param1U32 = (uint)idx; condition.Update(); });
                    break;
                case HeatActionConditionType.DistanceToRange:
                    CreateInput("Distance", condition.Condition.Param1F.ToString(), delegate (string val) { condition.Condition.Param1F = Utils.InvariantParse(val); condition.Update(); }, NumberBox.NumberMode.Float);
                    break;

                case HeatActionConditionType.FacingRange:
                    CreateComboBox("Range", (int)condition.Condition.Param1U32, Enum.GetNames<HeatActionRangeType>(), delegate (int idx) { condition.Condition.Param1U32 = (uint)idx; condition.Update(); });
                    break;
                case HeatActionConditionType.NearestAssetFlag:
                    CreateComboBox("Flag", (int)condition.Condition.Param1U32, Enum.GetNames<NearestAssetFlagType>(), delegate (int idx) { condition.Condition.Param1U32 = (uint)idx; condition.Update(); DrawNode(condition); });

                    switch ((NearestAssetFlagType)condition.Condition.Param1U32)
                    {
                        case NearestAssetFlagType.AssetID:
                            CreateInput("Asset ID", condition.Condition.Param2U32.ToString(), delegate (string val) { condition.Condition.Param2U32 = uint.Parse(val); condition.Update(); }, NumberBox.NumberMode.UInt);
                            break;
                        case NearestAssetFlagType.EntityUID:
                            CreateInput("Asset UID", condition.Condition.Param1U64.ToString(), delegate (string val) { condition.Condition.Param1U64 = ulong.Parse(val); condition.Update(); }, NumberBox.NumberMode.Long);
                            break;
                        case NearestAssetFlagType.Type:
                            CreateComboBox("Asset Type", (int)condition.Condition.Param2U32, Enum.GetNames<AssetArmsCategoryID>(), delegate (int idx) { condition.Condition.Param2U32 = (uint)idx; condition.Update(); });
                            break;
                        case NearestAssetFlagType.Subtype:
                            CreateComboBox("Asset Subtype", (int)condition.Condition.Param2U32, Enum.GetNames<AssetArmsCategoryID>(), delegate (int idx) { condition.Condition.Param2U32 = (uint)idx; condition.Update(); });
                            break;
                        case NearestAssetFlagType.Distance:
                            CreateInput("Distance", condition.Condition.Param1F.ToString(), delegate (string val) { condition.Condition.Param1F = Utils.InvariantParse(val); condition.Update(); }, NumberBox.NumberMode.Float);
                            break;
                    }
                    break;

                case HeatActionConditionType.InRange:
                    CreateComboBox("Range", (int)condition.Condition.Param1U32, Enum.GetNames<HeatActionRangeType>(), delegate (int idx) { condition.Condition.Param1U32 = (uint)idx; condition.Update(); });
                    break;

                case HeatActionConditionType.Health:
                    CreateInput("Is Percentage", Convert.ToByte(condition.Condition.Param1B).ToString(), delegate (string val) { condition.Condition.Param1B = byte.Parse(val) > 0; condition.Update();  DrawNode(condition); }, NumberBox.NumberMode.Byte);

                    if ((condition.Condition.Param1B))
                        CreateInput("Percentage", condition.Condition.Param1F.ToString(), delegate (string val) { condition.Condition.Param1F = Utils.InvariantParse(val); condition.Update(); }, NumberBox.NumberMode.Float);
                    else
                        CreateInput("Value", condition.Condition.Param1U64.ToString(), delegate (string val) { condition.Condition.Param1U64 = ulong.Parse(val); condition.Update(); }, NumberBox.NumberMode.Long);
                    break;

                case HeatActionConditionType.Grabbing:
                    CreateComboBox("Grab Direction", condition.Condition.Param1I32, new string[] {"Doesn't Matter", "Back", "Front"}, delegate (int idx) { condition.Condition.Param1I32 = idx; condition.Update(); });
                    CreateComboBox("Grab Category", condition.Condition.Param2I32, Enum.GetNames<AttackSyncCategory>(), delegate (int idx) { condition.Condition.Param2I32 = idx; condition.Update(); });
                    break;

                case HeatActionConditionType.CtrlType:
                    CreateInput("Ctrl Type", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); condition.Update(); }, NumberBox.NumberMode.UInt);
                    break;

                case HeatActionConditionType.SoldierID:
                    CreateInput("Soldier ID", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); condition.Update(); }, NumberBox.NumberMode.UInt);
                    break;

                case HeatActionConditionType.BattleConfigID:
                    CreateInput("Battle Config ID", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); condition.Update(); }, NumberBox.NumberMode.UInt);
                    break;

                case HeatActionConditionType.WouldDieInHAct:
                    CreateInput("Damage", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); condition.Update(); }, NumberBox.NumberMode.UInt);
                    CreateInput("Player", condition.Condition.Param1B.ToString(), delegate (string val) { condition.Condition.Param1B = byte.Parse(val) > 0; }, NumberBox.NumberMode.Byte);
                    break;

                case HeatActionConditionType.MotionID:
                    CreateInput("Motion ID", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); }, NumberBox.NumberMode.UInt);
                    break;

                case HeatActionConditionType.InBepElementRange:
                    CreateInput("Element ID", condition.Condition.Param1U32.ToString(), delegate (string val) { condition.Condition.Param1U32 = uint.Parse(val); }, NumberBox.NumberMode.UInt);
                    break;

                case HeatActionConditionType.CanAttackGeneric:
                    CreateInput("Allow Execute Mid Attack", condition.Condition.Param1B.ToString(), delegate (string val) { condition.Condition.Param1B = byte.Parse(val) > 0; }, NumberBox.NumberMode.Byte);
                    break;

            }

        }

        //Used for context menus
        private void attacksTree_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_rightClickedNode = attacksTree.GetNodeAt(e.X, e.Y);
            else if (e.Button == MouseButtons.Left)
                m_selectedNode = attacksTree.GetNodeAt(e.X, e.Y);
        }

        private void attacksTree_MouseDown(object sender, MouseEventArgs e)
        {
        }

        //File > New
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        //File > Open
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;

            DialogResult res = dialog.ShowDialog();

            if (res != DialogResult.OK)
                return;

            m_isYFC = dialog.FileName.EndsWith(".yfc");


            Clean();
            CleanVarPanel();

            if (m_isYFC)
            {
                YFC yfc = YFC.Read(dialog.FileName);
                m_filePath = dialog.FileName;

                foreach (AttackGroup group in yfc.Groups)
                {
                    TreeNodeAttackGroup groupNode = new TreeNodeAttackGroup(group);
                    attacksTree.Nodes.Add(groupNode);

                    foreach (Attack attack in group.Attacks)
                    {
                        TreeNodeAttack attackNode = new TreeNodeAttack(attack);
                        groupNode.Nodes.Add(attackNode);

                        foreach (AttackCondition cond in attack.Conditions)
                        {
                            TreeNodeCondition condNode = new TreeNodeCondition(cond);
                            attackNode.Nodes[0].Nodes.Add(condNode);
                        }
                    }
                }
            }
            else
            {
                YHC yhc = YHC.Read(dialog.FileName);
                m_filePath = dialog.FileName;

                foreach (HeatActionAttack attack in yhc.Attacks)
                {
                    TreeNodeHeatActionAttack attackNode = new TreeNodeHeatActionAttack(attack);
                    attacksTree.Nodes.Add(attackNode);

                    if (attack.StartupAttack != null)
                        attackNode.Nodes.Add(new TreeNodeAttack(attack.StartupAttack));

                    foreach (HeatActionActor actor in attack.Actors)
                    {
                        TreeNodeHeatActionActor actorNode = new TreeNodeHeatActionActor(actor);
                        attackNode.Nodes.Add(actorNode);

                        foreach (HeatActionCondition cond in actor.Conditions)
                        {
                            TreeNodeHeatActionCondition condNode = new TreeNodeHeatActionCondition(cond);
                            actorNode.Nodes.Add(condNode);
                        }
                    }
                }
            }
        }

        //Add > Attack (RPG)
        private void attackRPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeAttack(new AttackRPG()));
        }

        //Add > Attack (CFC)
        private void attackCFCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeAttack(new AttackCFC()));
        }

        private void attackGMTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeAttack(new AttackGMT()));
        }

        private void attackQuickstepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeAttack(new AttackQuickstep()));
        }

        private void attackSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeAttack(new AttackSync()));
        }

        private void attackEmptyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeAttack(new AttackEmpty()));
        }

        private void attackRangeCFCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeAttack(new AttackCFCRange()));
        }

        //Add > Attack Group (YFC)
        private void attackGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attacksTree.Nodes.Add(new TreeNodeAttackGroup(new AttackGroup()));
        }

        //Add > Attack (YHC)
        private void attackYHCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attacksTree.Nodes.Add(new TreeNodeHeatActionAttack(new HeatActionAttack()));
        }

        //Add Condition > Flag
        private void flagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeCondition(new AttackCondition()));
        }


        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.MoveUp();
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.MoveDown();
        }

        private void moveUpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.MoveUp();
        }

        private void moveDownToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.MoveDown();
        }

        private void addActorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeHeatActionActor(new HeatActionActor()));
        }

        private void flagToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.Nodes.Add(new TreeNodeHeatActionCondition(new HeatActionCondition()));
        }

        private void moveUpToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.MoveUp();
        }

        private void moveDownToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.MoveDown();
        }

        private void moveUpToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.MoveUp();
        }

        private void moveDownToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            m_rightClickedNode.MoveDown();
        }

        public void CreateHeader(string label, float spacing = 0)
        {
            Label label2 = new Label();
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Arial", 16F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(42, 5);
            label2.Size = new Size(195, 10);
            label2.TabIndex = 0;
            label2.Text = label;

            if (spacing > 0)
            {
                varPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, spacing));
                rowCount++;
                varPanel.RowCount = rowCount;

                varPanel.Controls.Add(CreateText(""), 0, varPanel.RowCount - 2);
                varPanel.Controls.Add(CreateText(""), 1, varPanel.RowCount - 2);
            }
            varPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            rowCount++;
            varPanel.RowCount = rowCount;

            varPanel.Controls.Add(label2, 0, varPanel.RowCount - 2);
            varPanel.Controls.Add(CreateText(""), 1, varPanel.RowCount - 2);
            varPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        }

        public Control CreateText(string label, bool left = false)
        {
            Label text = new Label();

            if (!left)
                text.Anchor = AnchorStyles.Right;
            else
                text.Anchor = AnchorStyles.Left;

            text.AutoSize = true;
            text.Size = new Size(58, 15);
            text.TabIndex = 1;
            text.Text = label;

            return text;
        }

        public TextBox CreateInput(string label, string defaultValue, Action<string> editedCallback, NumberBox.NumberMode mode = NumberBox.NumberMode.Text, bool readOnly = false)
        {
            varPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            rowCount++;
            varPanel.RowCount = rowCount;

            NumberBox input = new NumberBox(mode, editedCallback);
            input.Text = defaultValue;
            input.Size = new Size(200, 15);
            input.ReadOnly = readOnly;

            varPanel.Controls.Add(CreateText(label), 0, varPanel.RowCount - 2);
            varPanel.Controls.Add(input, 1, varPanel.RowCount - 2);
            varPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

            return input;
        }

        public void CreateComboBox(string label, int defaultIndex, string[] items, Action<int> editedCallback)
        {
            if (defaultIndex < 0 || defaultIndex >= items.Length)
            {
#if DEBUG
                //  throw new Exception("Combobox index error");
#endif
                //  defaultIndex = 0;

                if (defaultIndex > 0)
                {
                    List<string> itemsList = items.ToList();

                    while (itemsList.Count - 1 != defaultIndex)
                        itemsList.Add("Unknown");

                    items = itemsList.ToArray();
                }
            }

            varPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            rowCount++;
            varPanel.RowCount = rowCount;

            ComboBox input = new ComboBox();
            input.Items.AddRange(items);
            input.SelectedIndex = defaultIndex;
            input.Size = new Size(200, 15);

            input.SelectedIndexChanged += delegate { editedCallback?.Invoke(input.SelectedIndex); };

            varPanel.Controls.Add(CreateText(label), 0, varPanel.RowCount - 2);
            varPanel.Controls.Add(input, 1, varPanel.RowCount - 2);
            varPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        }

        public void CreateSpace(float space)
        {
            varPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, space));
            rowCount++;
            varPanel.RowCount = rowCount;

            varPanel.Controls.Add(CreateText(""), 0, varPanel.RowCount - 2);
            varPanel.Controls.Add(CreateText(""), 1, varPanel.RowCount - 2);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(m_filePath))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.ShowDialog();

                if (string.IsNullOrEmpty(dialog.FileName))
                    return;

                m_filePath = dialog.FileName;
            }

            if (m_isYFC)
            {
                YFC yfc = new YFC();

                IEnumerable<TreeNodeAttackGroup> attackGroups = attacksTree.Nodes.Cast<TreeNodeAttackGroup>();

                foreach (TreeNodeAttackGroup attackGroup in attackGroups)
                {
                    List<TreeNodeAttack> attackNodes = attackGroup.Nodes.Cast<TreeNodeAttack>().ToList();

                    AttackGroup group = new AttackGroup();
                    group.Name = attackGroup.Group.Name;

                    foreach (TreeNodeAttack attack in attackNodes)
                    {
                        Attack attackClone = attack.Attack.Copy();
                        attackClone.Conditions = attack.Nodes[0].Nodes.Cast<TreeNodeCondition>().Select(x => x.Condition).ToList();
                        group.Attacks.Add(attackClone);
                    }

                    yfc.Groups.Add(group);
                }

                YFC.Write(yfc, m_filePath);
            }
            else
            {
                YHC yhc = new YHC();

                IEnumerable<TreeNodeHeatActionAttack> attacks = attacksTree.Nodes.Cast<TreeNodeHeatActionAttack>();

                foreach (TreeNodeHeatActionAttack attack in attacks)
                {
                    HeatActionAttack atk = new HeatActionAttack();
                    atk.Name = attack.Attack.Name;
                    atk.HactID = attack.Attack.HactID;
                    atk.Flags = attack.Attack.Flags;
                    atk.Position = attack.Attack.Position;
                    atk.RotationY = attack.Attack.RotationY;
                    atk.PreferHActPosition = attack.Attack.PreferHActPosition;
                    atk.SpecialType = attack.Attack.SpecialType;
                    atk.Range = attack.Attack.Range;
                    atk.UseMatrix = attack.Attack.UseMatrix;
                    atk.Mtx = attack.Attack.Mtx;
                    atk.DontAllowMovementOnLinkOut = attack.Attack.DontAllowMovementOnLinkOut;
                    atk.IsFollowupOnly = attack.Attack.IsFollowupOnly;

                    TreeNodeAttack followUpAtkNode = (TreeNodeAttack)attack.Nodes.Cast<TreeNode>().FirstOrDefault(x => x is TreeNodeAttack);

                    if (followUpAtkNode != null)
                        atk.StartupAttack = followUpAtkNode.Attack;

                    List<TreeNodeHeatActionActor> actorNodes =
                        attack.Nodes.Cast<TreeNode>().Where(x => x is TreeNodeHeatActionActor).Cast<TreeNodeHeatActionActor>().ToList();

                    foreach (TreeNodeHeatActionActor actor in actorNodes)
                    {
                        HeatActionActor actorClone = actor.Actor.Copy();
                        actorClone.Conditions = actor.Nodes.Cast<TreeNodeHeatActionCondition>().Select(x => x.Condition).ToList();
                        atk.Actors.Add(actorClone);
                    }

                    yhc.Attacks.Add(atk);
                }

                YHC.Write(yhc, m_filePath);
            }
        }

        private void attacksTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && m_selectedNode != null)
            {

                if (m_selectedNode is TreeNodeAttack || m_selectedNode is TreeNodeCondition || m_selectedNode is TreeNodeAttackGroup)
                    m_selectedNode.Remove();

                else if (m_selectedNode is TreeNodeHeatActionAttack || m_selectedNode is TreeNodeHeatActionCondition || m_selectedNode is TreeNodeHeatActionActor)
                    m_selectedNode.Remove();

            }
            else if (ModifierKeys.HasFlag(Keys.Control))
            {
                if (e.KeyCode == Keys.C || e.KeyCode == Keys.V)
                {
                    if (e.KeyCode == Keys.C)
                        m_copiedNode = m_selectedNode;
                    else
                    {
                        if (m_copiedNode != null && (m_copiedNode is TreeNodeYHC || m_copiedNode is TreeNodeYFC))
                        {
                            TreeNode node = (TreeNode)m_copiedNode.Clone();

                            if (m_selectedNode != null)
                                m_selectedNode.Nodes.Add(node);
                            else
                                attacksTree.Nodes.Add(node);
                        }
                    }
                }
            }
        }
    }
}
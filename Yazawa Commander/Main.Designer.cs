namespace Yazawa_Commander
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.attacksTree = new System.Windows.Forms.TreeView();
            this.icons = new System.Windows.Forms.ImageList(this.components);
            this.appToolstrip = new System.Windows.Forms.ToolStrip();
            this.fileTab = new System.Windows.Forms.ToolStripDropDownButton();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newYHCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDropdown = new System.Windows.Forms.ToolStripDropDownButton();
            this.attackGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attackYHCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.conditionsRootContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addConditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attackGroupContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attackCFCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attackRPGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attackGMTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attackQuickstepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attackSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attackEmptyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.varPanel = new System.Windows.Forms.TableLayoutPanel();
            this.attackContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.conditionContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.heatActionAttackContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.addActorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heatActionActorContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.flagToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.heatActionConditionContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.attackRangeCFCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appToolstrip.SuspendLayout();
            this.conditionsRootContext.SuspendLayout();
            this.attackGroupContext.SuspendLayout();
            this.attackContext.SuspendLayout();
            this.conditionContext.SuspendLayout();
            this.heatActionAttackContext.SuspendLayout();
            this.heatActionActorContext.SuspendLayout();
            this.heatActionConditionContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // attacksTree
            // 
            this.attacksTree.ImageIndex = 0;
            this.attacksTree.ImageList = this.icons;
            this.attacksTree.Location = new System.Drawing.Point(15, 28);
            this.attacksTree.Name = "attacksTree";
            this.attacksTree.SelectedImageIndex = 0;
            this.attacksTree.Size = new System.Drawing.Size(279, 410);
            this.attacksTree.TabIndex = 0;
            this.attacksTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.attacksTree_AfterSelect);
            this.attacksTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.attacksTree_KeyDown);
            this.attacksTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.attacksTree_MouseDown);
            this.attacksTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.attacksTree_MouseUp);
            // 
            // icons
            // 
            this.icons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("icons.ImageStream")));
            this.icons.TransparentColor = System.Drawing.Color.Transparent;
            this.icons.Images.SetKeyName(0, "none.png");
            this.icons.Images.SetKeyName(1, "square.png");
            this.icons.Images.SetKeyName(2, "triangle.png");
            this.icons.Images.SetKeyName(3, "cross.png");
            this.icons.Images.SetKeyName(4, "circle.png");
            this.icons.Images.SetKeyName(5, "attack.png");
            this.icons.Images.SetKeyName(6, "condition.png");
            this.icons.Images.SetKeyName(7, "condition2.png");
            // 
            // appToolstrip
            // 
            this.appToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileTab,
            this.addDropdown});
            this.appToolstrip.Location = new System.Drawing.Point(0, 0);
            this.appToolstrip.Name = "appToolstrip";
            this.appToolstrip.Size = new System.Drawing.Size(754, 25);
            this.appToolstrip.TabIndex = 1;
            this.appToolstrip.Text = "appToolstrip";
            // 
            // fileTab
            // 
            this.fileTab.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileTab.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.newYHCToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem});
            this.fileTab.Image = ((System.Drawing.Image)(resources.GetObject("fileTab.Image")));
            this.fileTab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileTab.Name = "fileTab";
            this.fileTab.Size = new System.Drawing.Size(38, 22);
            this.fileTab.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.newToolStripMenuItem.Text = "New YFC";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // newYHCToolStripMenuItem
            // 
            this.newYHCToolStripMenuItem.Name = "newYHCToolStripMenuItem";
            this.newYHCToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.newYHCToolStripMenuItem.Text = "New YHC";
            this.newYHCToolStripMenuItem.Click += new System.EventHandler(this.newYHCToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // addDropdown
            // 
            this.addDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addDropdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.attackGroupToolStripMenuItem,
            this.attackYHCToolStripMenuItem});
            this.addDropdown.Image = ((System.Drawing.Image)(resources.GetObject("addDropdown.Image")));
            this.addDropdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addDropdown.Name = "addDropdown";
            this.addDropdown.Size = new System.Drawing.Size(42, 22);
            this.addDropdown.Text = "Add";
            // 
            // attackGroupToolStripMenuItem
            // 
            this.attackGroupToolStripMenuItem.Name = "attackGroupToolStripMenuItem";
            this.attackGroupToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.attackGroupToolStripMenuItem.Text = "Attack Group (YFC)";
            this.attackGroupToolStripMenuItem.Click += new System.EventHandler(this.attackGroupToolStripMenuItem_Click);
            // 
            // attackYHCToolStripMenuItem
            // 
            this.attackYHCToolStripMenuItem.Name = "attackYHCToolStripMenuItem";
            this.attackYHCToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.attackYHCToolStripMenuItem.Text = "Attack (YHC)";
            this.attackYHCToolStripMenuItem.Click += new System.EventHandler(this.attackYHCToolStripMenuItem_Click);
            // 
            // conditionsRootContext
            // 
            this.conditionsRootContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addConditionToolStripMenuItem});
            this.conditionsRootContext.Name = "attackContextMenu";
            this.conditionsRootContext.Size = new System.Drawing.Size(97, 26);
            // 
            // addConditionToolStripMenuItem
            // 
            this.addConditionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flagToolStripMenuItem,
            this.distanceToolStripMenuItem});
            this.addConditionToolStripMenuItem.Name = "addConditionToolStripMenuItem";
            this.addConditionToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.addConditionToolStripMenuItem.Text = "Add";
            // 
            // flagToolStripMenuItem
            // 
            this.flagToolStripMenuItem.Name = "flagToolStripMenuItem";
            this.flagToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.flagToolStripMenuItem.Text = "Flag";
            this.flagToolStripMenuItem.Click += new System.EventHandler(this.flagToolStripMenuItem_Click);
            // 
            // distanceToolStripMenuItem
            // 
            this.distanceToolStripMenuItem.Name = "distanceToolStripMenuItem";
            this.distanceToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.distanceToolStripMenuItem.Text = "Distance";
            // 
            // attackGroupContext
            // 
            this.attackGroupContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.attackGroupContext.Name = "attackGroupContext";
            this.attackGroupContext.Size = new System.Drawing.Size(181, 48);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.attackCFCToolStripMenuItem,
            this.attackRPGToolStripMenuItem,
            this.attackGMTToolStripMenuItem,
            this.attackQuickstepToolStripMenuItem,
            this.attackSyncToolStripMenuItem,
            this.attackEmptyToolStripMenuItem,
            this.attackRangeCFCToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // attackCFCToolStripMenuItem
            // 
            this.attackCFCToolStripMenuItem.Name = "attackCFCToolStripMenuItem";
            this.attackCFCToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.attackCFCToolStripMenuItem.Text = "Attack (CFC)";
            this.attackCFCToolStripMenuItem.Click += new System.EventHandler(this.attackCFCToolStripMenuItem_Click);
            // 
            // attackRPGToolStripMenuItem
            // 
            this.attackRPGToolStripMenuItem.Name = "attackRPGToolStripMenuItem";
            this.attackRPGToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.attackRPGToolStripMenuItem.Text = "Attack (RPG)";
            this.attackRPGToolStripMenuItem.Click += new System.EventHandler(this.attackRPGToolStripMenuItem_Click);
            // 
            // attackGMTToolStripMenuItem
            // 
            this.attackGMTToolStripMenuItem.Name = "attackGMTToolStripMenuItem";
            this.attackGMTToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.attackGMTToolStripMenuItem.Text = "Attack (GMT)";
            this.attackGMTToolStripMenuItem.Click += new System.EventHandler(this.attackGMTToolStripMenuItem_Click);
            // 
            // attackQuickstepToolStripMenuItem
            // 
            this.attackQuickstepToolStripMenuItem.Name = "attackQuickstepToolStripMenuItem";
            this.attackQuickstepToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.attackQuickstepToolStripMenuItem.Text = "Attack (Quickstep)";
            this.attackQuickstepToolStripMenuItem.Click += new System.EventHandler(this.attackQuickstepToolStripMenuItem_Click);
            // 
            // attackSyncToolStripMenuItem
            // 
            this.attackSyncToolStripMenuItem.Name = "attackSyncToolStripMenuItem";
            this.attackSyncToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.attackSyncToolStripMenuItem.Text = "Attack (Sync)";
            this.attackSyncToolStripMenuItem.Click += new System.EventHandler(this.attackSyncToolStripMenuItem_Click);
            // 
            // attackEmptyToolStripMenuItem
            // 
            this.attackEmptyToolStripMenuItem.Name = "attackEmptyToolStripMenuItem";
            this.attackEmptyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.attackEmptyToolStripMenuItem.Text = "Attack (Empty)";
            this.attackEmptyToolStripMenuItem.Click += new System.EventHandler(this.attackEmptyToolStripMenuItem_Click);
            // 
            // varPanel
            // 
            this.varPanel.AutoScroll = true;
            this.varPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.varPanel.ColumnCount = 2;
            this.varPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.varPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 317F));
            this.varPanel.Location = new System.Drawing.Point(300, 31);
            this.varPanel.Name = "varPanel";
            this.varPanel.RowCount = 3;
            this.varPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.varPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.varPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.varPanel.Size = new System.Drawing.Size(442, 407);
            this.varPanel.TabIndex = 2;
            // 
            // attackContext
            // 
            this.attackContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.attackContext.Name = "attackContext";
            this.attackContext.Size = new System.Drawing.Size(139, 48);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // conditionContext
            // 
            this.conditionContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpToolStripMenuItem1,
            this.moveDownToolStripMenuItem1});
            this.conditionContext.Name = "conditionContext";
            this.conditionContext.Size = new System.Drawing.Size(139, 48);
            // 
            // moveUpToolStripMenuItem1
            // 
            this.moveUpToolStripMenuItem1.Name = "moveUpToolStripMenuItem1";
            this.moveUpToolStripMenuItem1.Size = new System.Drawing.Size(138, 22);
            this.moveUpToolStripMenuItem1.Text = "Move Up";
            this.moveUpToolStripMenuItem1.Click += new System.EventHandler(this.moveUpToolStripMenuItem1_Click);
            // 
            // moveDownToolStripMenuItem1
            // 
            this.moveDownToolStripMenuItem1.Name = "moveDownToolStripMenuItem1";
            this.moveDownToolStripMenuItem1.Size = new System.Drawing.Size(138, 22);
            this.moveDownToolStripMenuItem1.Text = "Move Down";
            this.moveDownToolStripMenuItem1.Click += new System.EventHandler(this.moveDownToolStripMenuItem1_Click);
            // 
            // heatActionAttackContext
            // 
            this.heatActionAttackContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpToolStripMenuItem2,
            this.moveDownToolStripMenuItem2,
            this.addActorToolStripMenuItem});
            this.heatActionAttackContext.Name = "heatActionAttackContext";
            this.heatActionAttackContext.Size = new System.Drawing.Size(139, 70);
            // 
            // moveUpToolStripMenuItem2
            // 
            this.moveUpToolStripMenuItem2.Name = "moveUpToolStripMenuItem2";
            this.moveUpToolStripMenuItem2.Size = new System.Drawing.Size(138, 22);
            this.moveUpToolStripMenuItem2.Text = "Move Up";
            this.moveUpToolStripMenuItem2.Click += new System.EventHandler(this.moveUpToolStripMenuItem2_Click);
            // 
            // moveDownToolStripMenuItem2
            // 
            this.moveDownToolStripMenuItem2.Name = "moveDownToolStripMenuItem2";
            this.moveDownToolStripMenuItem2.Size = new System.Drawing.Size(138, 22);
            this.moveDownToolStripMenuItem2.Text = "Move Down";
            this.moveDownToolStripMenuItem2.Click += new System.EventHandler(this.moveDownToolStripMenuItem2_Click);
            // 
            // addActorToolStripMenuItem
            // 
            this.addActorToolStripMenuItem.Name = "addActorToolStripMenuItem";
            this.addActorToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.addActorToolStripMenuItem.Text = "Add Actor";
            this.addActorToolStripMenuItem.Click += new System.EventHandler(this.addActorToolStripMenuItem_Click);
            // 
            // heatActionActorContext
            // 
            this.heatActionActorContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem1});
            this.heatActionActorContext.Name = "heatActionActorContext";
            this.heatActionActorContext.Size = new System.Drawing.Size(97, 26);
            // 
            // addToolStripMenuItem1
            // 
            this.addToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flagToolStripMenuItem1});
            this.addToolStripMenuItem1.Name = "addToolStripMenuItem1";
            this.addToolStripMenuItem1.Size = new System.Drawing.Size(96, 22);
            this.addToolStripMenuItem1.Text = "Add";
            // 
            // flagToolStripMenuItem1
            // 
            this.flagToolStripMenuItem1.Name = "flagToolStripMenuItem1";
            this.flagToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.flagToolStripMenuItem1.Text = "Condition";
            this.flagToolStripMenuItem1.Click += new System.EventHandler(this.flagToolStripMenuItem1_Click);
            // 
            // heatActionConditionContext
            // 
            this.heatActionConditionContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpToolStripMenuItem3,
            this.moveDownToolStripMenuItem3});
            this.heatActionConditionContext.Name = "heatActionCondition";
            this.heatActionConditionContext.Size = new System.Drawing.Size(139, 48);
            // 
            // moveUpToolStripMenuItem3
            // 
            this.moveUpToolStripMenuItem3.Name = "moveUpToolStripMenuItem3";
            this.moveUpToolStripMenuItem3.Size = new System.Drawing.Size(138, 22);
            this.moveUpToolStripMenuItem3.Text = "Move Up";
            this.moveUpToolStripMenuItem3.Click += new System.EventHandler(this.moveUpToolStripMenuItem3_Click);
            // 
            // moveDownToolStripMenuItem3
            // 
            this.moveDownToolStripMenuItem3.Name = "moveDownToolStripMenuItem3";
            this.moveDownToolStripMenuItem3.Size = new System.Drawing.Size(138, 22);
            this.moveDownToolStripMenuItem3.Text = "Move Down";
            this.moveDownToolStripMenuItem3.Click += new System.EventHandler(this.moveDownToolStripMenuItem3_Click);
            // 
            // attackRangeCFCToolStripMenuItem
            // 
            this.attackRangeCFCToolStripMenuItem.Name = "attackRangeCFCToolStripMenuItem";
            this.attackRangeCFCToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.attackRangeCFCToolStripMenuItem.Text = "Attack (Range CFC)";
            this.attackRangeCFCToolStripMenuItem.Click += new System.EventHandler(this.attackRangeCFCToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 450);
            this.Controls.Add(this.varPanel);
            this.Controls.Add(this.appToolstrip);
            this.Controls.Add(this.attacksTree);
            this.Name = "Main";
            this.Text = "Yazawa Commander";
            this.appToolstrip.ResumeLayout(false);
            this.appToolstrip.PerformLayout();
            this.conditionsRootContext.ResumeLayout(false);
            this.attackGroupContext.ResumeLayout(false);
            this.attackContext.ResumeLayout(false);
            this.conditionContext.ResumeLayout(false);
            this.heatActionAttackContext.ResumeLayout(false);
            this.heatActionActorContext.ResumeLayout(false);
            this.heatActionConditionContext.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TreeView attacksTree;
        private ToolStrip appToolstrip;
        private ToolStripDropDownButton fileTab;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem addConditionToolStripMenuItem;
        private ToolStripMenuItem flagToolStripMenuItem;
        private ToolStripMenuItem distanceToolStripMenuItem;
        public ContextMenuStrip conditionsRootContext;
        private ToolStripDropDownButton addDropdown;
        private ToolStripMenuItem attackGroupToolStripMenuItem;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem attackCFCToolStripMenuItem;
        private ToolStripMenuItem attackRPGToolStripMenuItem;
        public ContextMenuStrip attackGroupContext;
        private TableLayoutPanel varPanel;
        private ToolStripMenuItem openToolStripMenuItem;
        private ImageList icons;
        private ToolStripMenuItem attackGMTToolStripMenuItem;
        private ToolStripMenuItem attackQuickstepToolStripMenuItem;
        private ToolStripMenuItem attackSyncToolStripMenuItem;
        private ToolStripMenuItem moveUpToolStripMenuItem;
        private ToolStripMenuItem moveDownToolStripMenuItem;
        public ContextMenuStrip attackContext;
        public ContextMenuStrip conditionContext;
        private ToolStripMenuItem moveUpToolStripMenuItem1;
        private ToolStripMenuItem moveDownToolStripMenuItem1;
        private ToolStripMenuItem newYHCToolStripMenuItem;
        private ToolStripMenuItem addActorToolStripMenuItem;
        private ToolStripMenuItem attackYHCToolStripMenuItem;
        public ContextMenuStrip heatActionAttackContext;
        public ContextMenuStrip heatActionActorContext;
        private ToolStripMenuItem addToolStripMenuItem1;
        private ToolStripMenuItem flagToolStripMenuItem1;
        private ToolStripMenuItem moveUpToolStripMenuItem2;
        private ToolStripMenuItem moveDownToolStripMenuItem2;
        private ToolStripMenuItem moveUpToolStripMenuItem3;
        private ToolStripMenuItem moveDownToolStripMenuItem3;
        public ContextMenuStrip heatActionConditionContext;
        private ToolStripMenuItem attackEmptyToolStripMenuItem;
        private ToolStripMenuItem attackRangeCFCToolStripMenuItem;
    }
}
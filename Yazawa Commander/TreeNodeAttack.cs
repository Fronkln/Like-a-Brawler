using System;
using System.Windows.Forms;
using YazawaCommand;

namespace Yazawa_Commander
{
    public class TreeNodeAttack : TreeNodeYFC
    {
        public Attack Attack = null;

        public TreeNode ConditionsRoot = null;

        public TreeNodeAttack()
        {

        }

        public override object Clone()
        {
            TreeNodeAttack atk = (TreeNodeAttack)base.Clone();
            atk.Attack = Attack.Copy();

            return atk;
        }

        public TreeNodeAttack(Attack attack)
        {
            Attack = attack;
            Text = Attack.Name;

            ConditionsRoot = new TreeNode("Conditions");
            ConditionsRoot.ContextMenuStrip = Main.Instance.conditionsRootContext;
            ConditionsRoot.ImageIndex = 6;
            ConditionsRoot.SelectedImageIndex = 6;
            Nodes.Add(ConditionsRoot);

            ImageIndex = 5;
            SelectedImageIndex = 5;

            ContextMenuStrip = Main.Instance.attackContext;
        }

        public override void Update()
        {
            base.Update();

            Text = Attack.Name;
        }
    }
}

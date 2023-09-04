using System.Runtime.CompilerServices;
using System.Windows.Forms;
using YazawaCommand;

namespace Yazawa_Commander
{
    internal class TreeNodeAttackGroup : TreeNodeYFC
    {
        public AttackGroup Group;

        public TreeNodeAttackGroup()
        {

        }

        public TreeNodeAttackGroup(AttackGroup group)
        {
            Group = group;
            Text = group.Name;
            ContextMenuStrip = Main.Instance.attackGroupContext;
        }

        public override object Clone()
        {
            TreeNodeAttackGroup cloned = (TreeNodeAttackGroup)base.Clone();
            cloned.Group = Group.Copy();

            return cloned;
        }

        public override void Update()
        {
            base.Update();

            Text = Group.Name;
        }
    }
}

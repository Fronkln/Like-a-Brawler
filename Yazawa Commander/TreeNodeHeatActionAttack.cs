using YazawaCommand;

namespace Yazawa_Commander
{
    internal class TreeNodeHeatActionAttack : TreeNodeYHC
    {
        public HeatActionAttack Attack;

        public TreeNodeHeatActionAttack()
        {

        }

        public TreeNodeHeatActionAttack(HeatActionAttack attack)
        {
            Attack = attack;

            ContextMenuStrip = Main.Instance.heatActionAttackContext;
            Update();
        }

        public override object Clone()
        {
            TreeNodeHeatActionAttack cloned = (TreeNodeHeatActionAttack)base.Clone();
            cloned.Attack = Attack.Copy();

            return cloned;
        }

        public override void Update()
        {
            base.Update();

            Text = Attack.Name;
        }
    }
}

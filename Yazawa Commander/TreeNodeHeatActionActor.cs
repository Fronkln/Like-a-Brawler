using System;
using YazawaCommand;

namespace Yazawa_Commander
{
    internal class TreeNodeHeatActionActor : TreeNodeYHC
    {
        public HeatActionActor Actor;

        public TreeNodeHeatActionActor()
        {

        }

        public TreeNodeHeatActionActor(HeatActionActor actor)
        {
            Actor = actor;
            ContextMenuStrip = Main.Instance.heatActionActorContext;
            Update();
        }

        public override object Clone()
        {
            TreeNodeHeatActionActor cloned = (TreeNodeHeatActionActor)base.Clone();
            cloned.Actor = Actor.Copy();

            return cloned;
        }

        public override void Update()
        {
            base.Update();

            Text = Actor.Type.ToString();
        }
    }
}

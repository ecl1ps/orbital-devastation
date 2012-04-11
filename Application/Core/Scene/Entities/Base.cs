using System;
using Orbit.Core.Player;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities
{
    public class Base : SceneObject, ICollidable
    {
        public PlayerPosition BasePosition { get; set; }
        public Color Color { get; set; }
        public int Integrity { get; set; }
        public Size Size { get; set; }

        public override bool IsOnScreen(Size screenSize)
        {
            return Integrity > 0;
        }

        public bool CollideWith(ICollidable other)
        {
            if (other is Sphere)
                return CollisionHelper.intersectsCircleAndSquare((other as Sphere).GetPosition(), (other as Sphere).Radius, GetPosition(), Size);

            return false;
        }

        public void DoCollideWith(ICollidable other)
        {
            if (other is Sphere)
            {
                Integrity -= (other as Sphere).Radius / 2;
                if (Integrity < 0)
                    Integrity = 0;
            }

            SceneMgr.GetInstance().GetUIDispatcher().BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode(SceneMgr.GetInstance().GetCanvas(),
                    BasePosition == PlayerPosition.LEFT ? "lblIntegrityLeft" : "lblIntegrityRight");
                if (lbl != null)
                    lbl.Content = (float)Integrity / (float)SharedDef.BASE_MAX_INGERITY * 100.0f + "%";
            }));
        }

        public override void UpdateGeometric()
        {
            
        }
    }

}

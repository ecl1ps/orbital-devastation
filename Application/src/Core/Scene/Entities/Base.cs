using System;
using Orbit.Core.Players;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Entities
{
    public class Base : Square
    {
        public PlayerPosition BasePosition { get; set; }
        public Color Color { get; set; }
        public int Integrity { get; set; }

        public override bool IsOnScreen(Size screenSize)
        {
            return Integrity > 0;
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is Meteor)
            {
                Integrity -= (other as Meteor).Radius / 2;
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

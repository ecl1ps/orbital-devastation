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
        
        public Base(SceneMgr mgr) : base(mgr)
        {
        }

        public override bool IsOnScreen(Size screenSize)
        {
            return Integrity > 0;
        }

        public override void DoCollideWith(ICollidable other)
        {
            if (other is Asteroid)
            {
                Integrity -= (other as Asteroid).Radius / 2;
                if (Integrity < 0)
                    Integrity = 0;

                (other as Asteroid).DoRemoveMe();

            }

            SceneMgr.BeginInvoke(new Action(() =>
            {
                Label lbl = (Label)LogicalTreeHelper.FindLogicalNode((SceneMgr as SceneMgr).GetCanvas(),
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

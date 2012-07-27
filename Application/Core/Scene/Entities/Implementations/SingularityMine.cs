using System;
using System.Windows;
using Orbit.Core.Scene.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using Lidgren.Network;
using System.Collections.Generic;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Entities.Implementations
{

    public class SingularityMine : Sphere, ISendable, IProjectile
    {
        public bool IsVisible { get; set; } // neposilan - pouzivano pouze controlem SingularityControl
        public Player Owner { get; set; } // neposilan
        public Brush BorderBrush { get; set; } // neposilan
        public Brush FillBrush { get; set; } // neposilan

        private bool exploded = false; // neposilan
        
        public SingularityMine(SceneMgr mgr) : base(mgr)
        {
            BorderBrush = Brushes.Black;
            FillBrush = Brushes.Black;
        }

        public override void DoCollideWith(ICollidable other, float tpf)
        {
            if (other is IMovable)
            {
                if (!exploded)
                {
                    SoundManager.Instance.StartPlayingOnce(SharedDef.MUSIC_EXPLOSION);
                    exploded = true;
                }

                DroppingSingularityControl c = GetControlOfType(typeof(DroppingSingularityControl)) as DroppingSingularityControl;
                if (c != null)
                    c.CollidedWith(other as IMovable);
            }
        }

        protected override void UpdateGeometricState()
        {
            ((geometryElement as Path).Data as EllipseGeometry).RadiusX = Radius;
            ((geometryElement as Path).Data as EllipseGeometry).RadiusY = Radius;
            (geometryElement as Path).Stroke = BorderBrush;
            (geometryElement as Path).Fill = FillBrush;
        }

        public void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write((int)PacketType.NEW_SINGULARITY_MINE);
            msg.WriteObjectSingularityMine(this);
            msg.WriteControls(GetControlsCopy());
        }

        public void ReadObject(NetIncomingMessage msg)
        {
            msg.ReadObjectSingularityMine(this);
            IList<IControl> controls = msg.ReadControls();
            foreach (Control c in controls)
                AddControl(c);
        }
    }
}

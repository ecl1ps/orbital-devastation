using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using System.Windows.Media;
using System.Windows.Controls;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class HighlightingControl : Control
    {
        private bool enabled;
        public override bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                if (!enabled)
                {
                    if (highlighter != null)
                        highlighter.Visible = false;
                }
                else
                {
                    if (me != null)
                    {
                        if (highlighter == null)
                            CreateHighlighter();
                        highlighter.Visible = true;
                    }
                }
            }
        }

        private ISceneObject highlighter;

        protected override void InitControl(ISceneObject me)
        {
            if (!(me is Sphere))
                throw new NotSupportedException("Highlighting control supports only Sphere parent object yet");

            enabled = false;
        }

        private void CreateHighlighter()
        {
            if (highlighter == null)
            {
                if (me is Sphere)
                {
                    highlighter = new HighlightingSphere(me.SceneMgr, IdMgr.GetNewId(me.SceneMgr.GetCurrentPlayer().GetId()));
                    highlighter.Position = me.Position;
                    (highlighter as Sphere).Radius = (me as Sphere).Radius + 10;
                    (highlighter as Sphere).Color = (me as Sphere).Color;
                    Color border = (me as Sphere).Color;
                    Color center = (me as Sphere).Color;
                    border.A = 0x0;
                    center.A = 0xAA;

                    highlighter.SetGeometry(SceneGeometryFactory.CreateRadialGradientEllipseGeometry(
                        me.SceneMgr, (highlighter as Sphere).Radius, border, center, Colors.Transparent, me.Position));

                    // nastavit ho pod rodicovsky objekt
                    me.SceneMgr.BeginInvoke(new Action(() =>
                    {
                        Canvas.SetZIndex(highlighter.GetGeometry(), Canvas.GetZIndex(me.GetGeometry()) - 1);
                    }));

                    PositionCloneControl pcc = new PositionCloneControl(me);
                    highlighter.AddControl(pcc);

                    highlighter.Visible = false;

                    me.SceneMgr.DelayedAttachToScene(highlighter);
                }
                else
                    throw new NotSupportedException("Highlighting control supports only Sphere parent object yet");
            }
        }
    }
}

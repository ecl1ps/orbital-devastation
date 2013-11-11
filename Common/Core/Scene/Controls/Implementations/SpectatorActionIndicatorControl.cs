using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class SpectatorActionIndicatorControl : Control
    {
        public ISpectatorAction Action { get; set; }
        public ISceneObject Indicator { get; set; }
        public ISceneObject ExactIndicator { get; set; }

        protected override void InitControl(ISceneObject me)
        {
            Indicator.Visible = false;
            ExactIndicator.Visible = false;
        }

        protected override void UpdateControl(float tpf)
        {
            Indicator.Visible = Action.IsReady();
            ExactIndicator.Visible = Action.IsReady() && Action.IsExact();
        }

        public override void OnRemove()
        {
            Indicator.DoRemoveMe();
            ExactIndicator.DoRemoveMe();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.SpecialActions;
using Orbit.Core.SpecialActions.Spectator;

namespace Orbit.Gui.ActionControllers
{
    class SpectatorActionController : ActionControllerImpl
    {
        private ISpectatorAction spectatorAction;

        public SpectatorActionController(SceneMgr mgr, ISpecialAction action)
            : base(mgr, action)
        {
            if (action is ISpectatorAction)
                this.spectatorAction = action as ISpectatorAction;
            else
                throw new Exception("Spectator action controller must be initated with Spectator action");
        }

        public override void Update(ActionUC window, float tpf)
        {
            base.Update(window, tpf);
            if (!window.IsSpectatorMode())
                window.SpectatorMode();
            UpdateCount(window);
        }

        private void UpdateCount(ActionUC window)
        {
            int count = spectatorAction.RangeCount;
            if (count > 0)
            {
                window.SetCountText(String.Format(Strings.Culture, Strings.char_plus_and_val, count));
            }
            else
            {
                window.SetCountText(count.ToString(Strings.Culture));
            }

            //window.Highlight(count == 0);
        }
    }
}

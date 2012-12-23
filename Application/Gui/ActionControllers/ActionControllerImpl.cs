using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.SpecialActions;

namespace Orbit.Gui.ActionControllers
{
    class ActionControllerImpl : ActionController<ActionUC>
    {
        public ActionControllerImpl(SceneMgr mgr, ISpecialAction action)
            : base(action, mgr)
        {
        }

        public override void ActionClicked(ActionUC window)
        {
            if (Action.IsReady())
                Action.StartAction();

            window.Refresh();
        }

        public override void CreateHeaderText(ActionUC window)
        {
            window.SetHeaderText(Action.Name);
        }

        public override void CreatePriceText(ActionUC window)
        {
            window.SetPriceText("Costs " + Action.Cost + " credits");
        }

        public override void CreateImageUriString(ActionUC window)
        {
            if(Action.ImageSource != null)
                window.SetImageUri(Action.ImageSource);
        }

        public override void Update(ActionUC window, float tpf)
        {
            if (Action.IsOnCooldown())
                computeCooldown(window);
        }

        private void computeCooldown(ActionUC window)
        {
            float perc = (Action.RemainingCooldown / Action.Cooldown);
            window.SetShadeWidth(30 * perc);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.SpecialActions;

namespace Orbit.Gui.ActionControllers
{
    class ActionControllerImpl : ActionController
    {
        public ActionControllerImpl(SceneMgr mgr, ISpecialAction action)
            : base(action, mgr)
        {
        }

        public override void ActionClicked(BuyActionUC window)
        {
            if (Action.IsReady())
                Action.StartAction();

            window.Refresh();
        }

        public override void CreateHeaderText(BuyActionUC window)
        {
            window.SetHeaderText(Action.Name);
        }

        public override void CreatePriceText(BuyActionUC window)
        {
            window.SetPriceText("Costs " + Action.Cost + " credits");
        }

        public override void CreateImageUriString(BuyActionUC window)
        {
            if(Action.ImageSource != null)
                window.SetImageUri(Action.ImageSource);
        }
    }
}

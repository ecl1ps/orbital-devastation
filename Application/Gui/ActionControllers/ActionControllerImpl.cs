﻿using System;
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
            if (sceneMgr.UserActionsDisabled)
                return;

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
            if (Action.Cost != 0)
                window.SetPriceText(String.Format(Strings.Culture, Strings.ui_action_cost, Action.Cost));
            else
                window.SetPriceText(String.Empty);
        }

        public override void CreateImageUriString(ActionUC window)
        {
            if (Action.ImageSource != null)
                window.SetImageUri(Action.ImageSource);
        }

        public override void CreateBackgroundColor(ActionUC window)
        {
            window.SetBackgroundColor(Action.BackgroundColor);
        }

        public override void Update(ActionUC window, float tpf)
        {
            float time = 0;
            if (Action.IsOnCooldown())
            {
                ComputeCooldown(window);
                time = Action.RemainingCooldown;
            }

            window.SetCooldownTime(time);
        }

        private void ComputeCooldown(ActionUC window)
        {
            float perc = (Action.RemainingCooldown / Action.Cooldown);
            window.SetShadeWidth(42 * perc);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Controls.Implementations;
using System.Windows;
using Orbit.Core.Scene.Entities.Implementations;
using System.Windows.Media;

namespace Orbit.Core.SpecialActions.Spectator
{
    class Shielding : SpecialAction
    {
        private ISceneObject toFollow;

        public Shielding(ISceneObject toFollow,SceneMgr mgr, Player plr) : 
            base(mgr, plr) {

                Name = "Static Shield";
                ImageSource = "pack://application:,,,/resources/images/icons/shield-icon.png";
                Cost = 1500;
                this.toFollow = toFollow;
        }

        public override void StartAction()
        {
            base.StartAction();

            PercentageArc arc = (Owner.Device.GetControlOfType(typeof(HpBarControl)) as HpBarControl).Bar;
            arc.Color = Colors.RoyalBlue;

            LimitedReverseDamageControl c = new LimitedReverseDamageControl(SharedDef.SPECTATOR_SHIELDING_TIME);
            c.addControlDestroyAction(new Action(() => { arc.Color = Owner.GetPlayerColor(); }));
            toFollow.AddControl(c);
        }

        public override bool IsReady()
        {
            return Owner.Data.Gold >= Cost && (toFollow.GetControlOfType(typeof(LimitedReverseDamageControl)) == null);
        }
    }
}

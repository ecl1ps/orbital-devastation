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
using Lidgren.Network;
using Orbit.Core.Scene.Controls.Health.Implementations;

namespace Orbit.Core.SpecialActions.Spectator
{
    class Shielding : SpectatorAction
    {
        private ISceneObject toFollow;

        public Shielding(ISceneObject toFollow,SceneMgr mgr, Player plr) : base(mgr, plr) {
            Name = "Static Shield";
            ImageSource = "pack://application:,,,/resources/images/icons/shield-icon.png";
            Cost = 750;
            this.toFollow = toFollow;
        }

        public override void StartAction()
        {
            if (!control.Enabled)
                return;

            base.StartAction();

            IHpBar bar = Owner.Device.GetControlOfType<HpBarControl>().Bar;
            bar.Color = Colors.RoyalBlue;

            LimitedReverseDamageControl c = new LimitedReverseDamageControl(SharedDef.SPECTATOR_SHIELDING_TIME);
            c.AddControlDestroyAction(new Action(() => { bar.Color = Owner.GetPlayerColor(); }));
            toFollow.AddControl(c);

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.MODULE_COLOR_CHANGE);
            msg.Write(Owner.GetId());
            msg.Write(bar.Color);

            SceneMgr.SendMessage(msg);
        }

        public override bool IsReady()
        {
            return Owner.Data.Gold >= Cost && toFollow.GetControlOfType<LimitedReverseDamageControl>() == null;
        }
    }
}

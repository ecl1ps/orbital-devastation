﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Client.GameStates;
using System.Windows.Media;
using Orbit.Core.Scene.Entities;
using Lidgren.Network;

namespace Orbit.Core.SpecialActions.Spectator
{
    public abstract class SpectatorAction : SpecialAction, ISpectatorAction
    {
        protected MiningModuleControl control;

        public RangeGroup Range { get; set; }

        public float CastingTime { get; set; }

        public SpectatorAction(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            this.control = owner.Device.GetControlOfType<MiningModuleControl>();
            BackgroundColor = Colors.Bisque;
        }

        public override bool IsReady()
        {
            return !IsOnCooldown();
        }


        public override void StartAction()
        {
            if (!control.Enabled)
                return;

            base.StartAction();

            StartAction(Range.GetValidGroup(control.CurrentlyMining));
            SendActionStart();
            
        }

        private void SendActionStart()
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.SPECTATOR_ACTION_START);

            msg.Write(Owner.GetId());
            msg.Write(Name);

            SceneMgr.SendMessage(msg);
        }

        protected abstract void StartAction(List<Asteroid> afflicted);
   
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Client.GameStates;
using Lidgren.Network;

namespace Orbit.Core.SpecialActions.Spectator
{
    class AsteroidGrowth : SpectatorAction
    {
        public AsteroidGrowth(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            Name = "Asteroid growth";
            Type = SpecialActionType.ASTEROID_GROWTH;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-growth-icon.png";

            //nastavime parametry
            this.Cooldown = 3; //sekundy
            this.Range = new RangeGroup(new Range(AsteroidType.NORMAL, 4), new Range(AsteroidType.GOLDEN, 2));
        }

        protected override void  StartAction(List<Asteroid> afflicted)
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.OBJECTS_HEAL_AMOUNT);

            msg.Write(Owner.GetId());
            msg.Write(afflicted.Count);
            msg.Write(SharedDef.SPECTATOR_HEAL);

            afflicted.ForEach(aff => { growAsteroid(aff); msg.Write(aff.Id); });

            SceneMgr.SendMessage(msg);
        }

        private void growAsteroid(Asteroid a)
        {
            int val = SharedDef.SPECTATOR_HEAL;
            a.Radius += val;
            SceneMgr.FloatingTextMgr.AddFloatingText("+" + val, a.Position, FloatingTextManager.TIME_LENGTH_3, Client.GameStates.FloatingTextType.HEAL);
        }
    }
}

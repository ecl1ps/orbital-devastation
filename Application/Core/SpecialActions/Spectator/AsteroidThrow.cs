﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene.Entities.Implementations;
using Lidgren.Network;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Controls;
using System.Windows.Media;

namespace Orbit.Core.SpecialActions.Spectator
{
    public class AsteroidThrow : SpectatorAction
    {
        public AsteroidThrow(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            Name = "Asteroid throw";
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-throw-icon.png";
            Type = SpecialActionType.ASTEROID_THROW;

            //nastavime parametry
            this.Cooldown = 4; //sec
            this.CastingTime = 0.5f; //sec;
            this.CastingColor = Colors.Orange;
            this.Range = new Range(5);
        }

        public override void StartAction(List<Asteroid> afflicted)
        {
            int count = 0;

            Vector v;
            foreach (Asteroid ast in afflicted)
            {
                v = Owner.Device.Position - ast.Position;
                v = v.NormalizeV();
                IMovementControl mc = ast.GetControlOfType<IMovementControl>();
                if (mc != null)
                    mc.Speed = SharedDef.SPECTATOR_ASTEROID_THROW_SPEED;

                ast.Direction = v;
                count++;
            }

            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.ASTEROIDS_DIRECTIONS_CHANGE);
            msg.Write(count);

            foreach (Asteroid ast in afflicted)
            {
                msg.Write(ast.Id);
                msg.Write(ast.Direction);
            }

            SceneMgr.SendMessage(msg);
        }
    }
}

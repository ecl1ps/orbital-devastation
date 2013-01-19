using System;
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
        protected float exactBonus;

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
            this.exactBonus = 1.5f;
        }

        public override void StartAction(List<Asteroid> afflicted, bool exact)
        {
            int count = 0;

            Vector v;
            float speed = exact ? SharedDef.SPECTATOR_ASTEROID_THROW_SPEED * exactBonus : SharedDef.SPECTATOR_ASTEROID_THROW_SPEED;
            foreach (Asteroid ast in afflicted)
            {
                v = Owner.Device.Position - ast.Position;
                v = v.NormalizeV();
                IMovementControl mc = ast.GetControlOfType<IMovementControl>();
                if (mc != null)
                    mc.Speed = speed;

                ast.Direction = v;
                count++;
            }
        }
    }
}

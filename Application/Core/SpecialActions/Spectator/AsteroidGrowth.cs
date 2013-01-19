using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Client.GameStates;
using Lidgren.Network;
using System.Windows.Media;

namespace Orbit.Core.SpecialActions.Spectator
{
    class AsteroidGrowth : SpectatorAction
    {
        protected float exactBonus;

        public AsteroidGrowth(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            Name = "Asteroid growth";
            Type = SpecialActionType.ASTEROID_GROWTH;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-growth-icon.png";

            //nastavime parametry
            this.Cooldown = 3; //sekundy
            this.CastingTime = 0.5f;
            this.CastingColor = Colors.Green;
            this.Range = new Range(4);
            this.exactBonus = 1.2f;
        }

        public override void  StartAction(List<Asteroid> afflicted, bool exact)
        {
            NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
            msg.Write((int)PacketType.OBJECTS_HEAL_AMOUNT);

            msg.Write(Owner.GetId());
            msg.Write(afflicted.Count);
            msg.Write(SharedDef.SPECTATOR_GROWTH);

            afflicted.ForEach(aff => { GrowAsteroid(aff, exact); msg.Write(aff.Id); });

            SceneMgr.SendMessage(msg);
        }

        private void GrowAsteroid(Asteroid a, bool exact)
        {
            int val = (int) (exact ? SharedDef.SPECTATOR_GROWTH * exactBonus : SharedDef.SPECTATOR_GROWTH);
            a.Radius += val;
            if (a.Radius > SharedDef.ASTEROID_MAX_GROWN_RADIUS)
                a.Radius = SharedDef.ASTEROID_MAX_GROWN_RADIUS;
            SceneMgr.FloatingTextMgr.AddFloatingText("+" + val, a.Position, FloatingTextManager.TIME_LENGTH_3, Client.GameStates.FloatingTextType.HEAL);
        }
    }
}

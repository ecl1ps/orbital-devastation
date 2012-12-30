using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.SpecialActions.Spectator
{
    class AsteroidGrowth : SpectatorAction
    {
        public AsteroidGrowth(SceneMgr mgr, Players.Player owner, params ISpectatorAction[] actions)
            : base(mgr, owner, actions)
        {
            Name = "Asteroid growth";
            Type = SpecialActionType.ASTEROID_GROWTH;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-damage-icon.png";

            //nastavime parametry
            this.Cooldown = 3; //sekundy
            this.Range = new RangeGroup(new Range(AsteroidType.NORMAL, 4), new Range(AsteroidType.GOLDEN, 2));
        }

        protected override void  StartAction(List<Asteroid> afflicted)
        {
            afflicted.ForEach(aff => growAsteroid(aff));
        }

        private void growAsteroid(Asteroid a)
        {
            int val = SharedDef.SPECTATOR_HEAL;
            a.Radius += val;
            SceneMgr.FloatingTextMgr.AddFloatingText("+" + val, a.Position, FloatingTextManager.TIME_LENGTH_3, Client.GameStates.FloatingTextType.HEAL);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls;
using Orbit.Core.Scene.Controls.Implementations;

namespace Orbit.Core.SpecialActions.Spectator
{
    class AsteroidSlow : SpectatorAction
    {
        public AsteroidSlow(SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            Name = "Asteroid slow";
            Type = SpecialActionType.ASTEROID_SLOW;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-slow-icon.png";

            //nastavime parametry
            this.Cooldown = 8; //sekundy
            this.Range = new RangeGroup(new Range(AsteroidType.NORMAL, 2), new Range(AsteroidType.GOLDEN, 2));
        }

        private void Slow(Asteroid ast)
        {
            IMovementControl c = ast.GetControlOfType<IMovementControl>();
            if(c == null)
                return;

            TemporaryControlRemovalControl removal = new TemporaryControlRemovalControl();
            removal.ToRemove = c;
            removal.Time = 3;

            ast.AddControl(removal);
        }

        protected override void StartAction(List<Asteroid> afflicted)
        {
            afflicted.ForEach(a => Slow(a));
        }
    }
}

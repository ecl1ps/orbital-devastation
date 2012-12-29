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
            this.Normal = new RangeGroup(AsteroidType.NORMAL, new Range(2));
            this.Gold = new RangeGroup(AsteroidType.GOLDEN, new Range());
        }

        public override void StartAction()
        {
            base.StartAction();

            foreach (MiningObject locked in lockedObjects)
            {
                if (locked.Obj is Asteroid)
                    growAsteroid(locked.Obj as Asteroid);
            }
        }

        private void growAsteroid(Asteroid a)
        {
            int val = SharedDef.SPECTATOR_HEAL;
            a.Radius += val;
            SceneMgr.FloatingTextMgr.AddFloatingText("+" + val, a.Position, FloatingTextManager.TIME_LENGTH_3, Client.GameStates.FloatingTextType.HEAL);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.SpecialActions.Spectator
{
    class AsteroidDamage : SpecialAction
    {
        private MiningModuleControl control;

        public AsteroidDamage(MiningModuleControl control, SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            Name = "Asteroid damage";
            Type = SpecialActionType.ASTEROID_THROW;
            ImageSource = "pack://application:,,,/resources/images/icons/asteroid-damage-icon.png";
            this.control = control;
            Cost = 250;
        }

        public override void StartAction()
        {
            base.StartAction();

            foreach (MiningObject afflicted in control.currentlyMining)
            {
                if (afflicted.Obj is IDestroyable)
                {
                    (afflicted.Obj as IDestroyable).TakeDamage(SharedDef.SPECTATOR_DAMAGE, null);
                    SceneMgr.FloatingTextMgr.AddFloatingText(SharedDef.SPECTATOR_DAMAGE, afflicted.Obj.Position, FloatingTextManager.TIME_LENGTH_3, FloatingTextType.DAMAGE); 
                }
            }
        }

        public override bool IsReady()
        {
            return Cost <= Owner.Data.Gold && control.currentlyMining.Count != 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.SpecialActions
{
    public class AsteroidThrow : SpecialAction
    {
        protected MiningModuleControl Control;

        public AsteroidThrow(MiningModuleControl control, SceneMgr mgr, Players.Player owner)
            : base(mgr, owner)
        {
            Name = "Asteroid throw";
            Type = SpecialActionType.ASTEROID_THROW;
            Control = control;
        }

        public override void StartAction()
        {
            Vector v = new Vector();
            v = Control.Position - new Vector(Control.Position.X, SharedDef.CANVAS_SIZE.Height);
            v = v.NormalizeV();

            foreach (MiningObject afflicted in Control.currentlyMining)
            {
                if (afflicted.Obj is Asteroid)
                    (afflicted.Obj as Asteroid).Direction = v;
            }
        }

        public override bool IsReady()
        {
            return true;
        }
    }
}

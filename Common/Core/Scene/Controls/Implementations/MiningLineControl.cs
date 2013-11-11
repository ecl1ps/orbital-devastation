using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using System.Windows;
using Orbit.Core.Helpers;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class MiningEllipse
    {
        public Sphere Obj { get; set; }
        public float RunningTime { get; set; }

        public MiningEllipse(Sphere ellipse)
        {
            Obj = ellipse;
            RunningTime = 0;
        }
    }

    public class MiningLineControl : Control
    {
        private Line line;

        protected override void InitControl(Entities.ISceneObject me)
        {
            if (me is Line)
                line = me as Line;
            else
                throw new Exception("MiningLineControl must be attached to SolidLine object");

            events.AddEvent(1, new Event(SharedDef.SPECTATOR_ORBITS_SPAWN_TIME, EventType.REPEATABLE, new Action(() => { SpawnNewEllipse(); })));
        }

        private void SpawnNewEllipse()
        {
            Sphere ellipse = SceneObjectFactory.CreateOrbitEllipse(me.SceneMgr, me.Position, 3, Color.Black);

            MiningEllipseControl control = new MiningEllipseControl();
            control.LineToFollow = line;

            ellipse.AddControl(control);

            me.SceneMgr.DelayedAttachToScene(ellipse);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using System.Windows;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class MiningEllipse
    {
        public OrbitEllipse Obj { get; set; }
        public float RunningTime { get; set; }

        public MiningEllipse(OrbitEllipse ellipse)
        {
            Obj = ellipse;
            RunningTime = 0;
        }
    }

    class MiningLineControl : Control
    {
        private Line line;
        private float spawnTime = 0;

        protected override void InitControl(Entities.ISceneObject me)
        {
            if (me is Line)
                line = me as Line;
            else
                throw new Exception("MiningLineControl must be attached to SolidLine object");

        }

        protected override void UpdateControl(float tpf)
        {
            spawnTime -= tpf;
            if (spawnTime < 0)
                SpawnNewEllipse();
        }

        private void SpawnNewEllipse()
        {
            spawnTime = SharedDef.SPECTATOR_ORBITS_SPAWN_TIME;

            OrbitEllipse ellipse = SceneObjectFactory.CreateOrbitEllipse(me.SceneMgr, line.End, 2.5f, 2.5f);

            MiningEllipseControl control = new MiningEllipseControl();
            control.LineToFollow = line;

            ellipse.AddControl(control);

            me.SceneMgr.DelayedAttachToScene(ellipse);
        }
    }
}

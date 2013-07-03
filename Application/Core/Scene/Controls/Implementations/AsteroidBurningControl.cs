using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Xna.Framework;
using Orbit.Core.Helpers;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class AsteroidBurningControl : Control
    {
        private Asteroid asteroid;
        private double maxHeatDistance;
        private double minHeatDistance;
        private float lastDistProcessed;
        private int step;
        private float stepSize;

        private float tpf;

        public AsteroidBurningControl(Asteroid parent)
        {
            asteroid = parent;
            maxHeatDistance = PlayerBaseLocation.GetBaseLocation(PlayerPosition.LEFT).Top - 150;
            minHeatDistance = SharedDef.ORBIT_AREA.Bottom;
            lastDistProcessed = -0.1f;
            step = 0;
            stepSize = 0.3f;
        }

        protected override void InitControl(ISceneObject me)
        {
            if (asteroid.Center.Y < minHeatDistance)
                return;

            float distPct = Math.Min((float)((asteroid.Center.Y - minHeatDistance) / (maxHeatDistance - minHeatDistance)), 1);
            while (distPct >= lastDistProcessed + stepSize && step <= 3)
            {
                step++;
                lastDistProcessed += stepSize;
                ProcessStep();
            }
        }

        protected override void UpdateControl(float tpf)
        {
            this.tpf = tpf;
            if (asteroid.Center.Y < minHeatDistance)
                return;

            float distPct = Math.Min((float)((asteroid.Center.Y - minHeatDistance) / (maxHeatDistance - minHeatDistance)), 1);
            if (distPct < lastDistProcessed + stepSize)
                return;

            step++;
            lastDistProcessed = distPct;

            ProcessStep();
        }

        private void ProcessStep()
        {
            switch (step)
            {
                case 1:
                    {
                        /*
                        ParticleEmmitor fireEmmitor = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(40, 255, 60, 0));
                        fireEmmitor.Amount = 20;
                        fireEmmitor.MinLife = 0.3f;
                        fireEmmitor.MaxLife = 0.4f;
                        fireEmmitor.MinSize = asteroid.Radius / 5.0f;
                        fireEmmitor.MaxSize = asteroid.Radius / 6.0f;
                        fireEmmitor.Infinite = true;
                        */
                    }
                    break;
                case 2:
                    {
                        asteroid.AddControl(new ShakingControl(1, true, 0.05f));
                    }
                    break;
                case 3: 
                    break;
            }
        }

        protected int GetMaxSmoke()
        {
            if (asteroid.Radius < 12)
                return 1;
            if (asteroid.Radius < 20)
                return 2;

            return 3;
        }
    }
}

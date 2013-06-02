using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Orbit.Core.Helpers;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Particles.Implementations;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class AsteroidBurningControl : Control
    {
        private Asteroid asteroid;
        private ParticleNode meNode;
        private double maxHeatDistance;
        private double minHeatDistance;
        private float lastDistProcessed;
        private int step;
        private float stepSize;

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
            if (!(me is ParticleNode))
                throw new ArgumentException("AsteroidBurningControl must by attached to an ParticleNode object");

            meNode = me as ParticleNode;
        }

        protected override void UpdateControl(float tpf)
        {
            if (asteroid.Center.Y < minHeatDistance || asteroid.Center.Y > maxHeatDistance)
                return;

            float distPct = Math.Min((float)((asteroid.Center.Y - minHeatDistance) / (maxHeatDistance - minHeatDistance)), 1);
            if (distPct < lastDistProcessed + stepSize)
                return;

            step++;
            lastDistProcessed = distPct;

            switch (step)
            {
                case 1:
                    {
                        ParticleEmmitor smokeEmmitor = new ParticleEmmitor(me.SceneMgr, IdMgr.GetNewId(me.SceneMgr.GetCurrentPlayer().GetId()));
                        smokeEmmitor.Amount = 50;
                        smokeEmmitor.MinLife = 2f;
                        smokeEmmitor.MaxLife = 3f;
                        smokeEmmitor.MinSize = (float)asteroid.Radius / 20.0f;
                        smokeEmmitor.MaxSize = (float)asteroid.Radius / 25.0f;
                        smokeEmmitor.Infinite = true;
                        ParticleImageFactory f = new ParticleImageFactory();
                        f.Color = Color.FromArgb(80, 0, 0, 0);
                        f.RenderSize = 200;
                        f.Source = new Uri("pack://application:,,,/resources/images/particles/fire-particle.png");
                        smokeEmmitor.Factory = f;

                        ParticleEmmitor fireEmmitor = new ParticleEmmitor(me.SceneMgr, IdMgr.GetNewId(me.SceneMgr.GetCurrentPlayer().GetId()));
                        fireEmmitor.Amount = 30;
                        fireEmmitor.MinLife = 0.5f;
                        fireEmmitor.MaxLife = 1.2f;
                        fireEmmitor.MinSize = asteroid.Radius / 4;
                        fireEmmitor.MaxSize = asteroid.Radius / 6;
                        fireEmmitor.Infinite = true;
                        ParticleImageFactory ff = new ParticleImageFactory();
                        ff.Color = Color.FromArgb(50, 255, 100, 0);
                        ff.RenderSize = 200;
                        ff.Source = new Uri("pack://application:,,,/resources/images/particles/fire-particle.png");
                        fireEmmitor.Factory = ff;

                        meNode.AddEmmitor(fireEmmitor, new Vector(0, 0), false);

                        meNode.AddEmmitor(smokeEmmitor, new Vector(-5, 0), false);
                    }
                    break;
            }
        }
    }
}

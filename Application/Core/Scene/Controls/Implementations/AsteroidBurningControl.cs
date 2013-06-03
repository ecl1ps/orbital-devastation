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

            events.AddEvent(1, new Event(0.5f, EventType.REPEATABLE, new Action(() => CheckSparkSpawn())));
        }

        protected override void UpdateControl(float tpf)
        {
            if (asteroid.Center.Y < minHeatDistance)
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
                        ParticleEmmitor smokeEmmitor = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(60, 0, 0, 0));
                        smokeEmmitor.Amount = 50;
                        smokeEmmitor.MinLife = 1f;
                        smokeEmmitor.MaxLife = 2f;
                        smokeEmmitor.MinSize = asteroid.Radius / 10.0f;
                        smokeEmmitor.MaxSize = asteroid.Radius / 15.0f;
                        smokeEmmitor.SpawnRadius = asteroid.Radius / 4.0f; 
                        smokeEmmitor.Infinite = true;

                        ParticleEmmitor fireEmmitor = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(40, 255, 60, 0));
                        fireEmmitor.Amount = 20;
                        fireEmmitor.MinLife = 0.3f;
                        fireEmmitor.MaxLife = 0.4f;
                        fireEmmitor.MinSize = asteroid.Radius / 5.0f;
                        fireEmmitor.MaxSize = asteroid.Radius / 6.0f;
                        fireEmmitor.Infinite = true;

                        meNode.AddEmmitor(fireEmmitor, new Vector(0, 0), false);

                        meNode.AddEmmitor(smokeEmmitor, new Vector(asteroid.Radius * -0.7, 0), false);
                    }
                    break;
                case 2:
                    {
                        ParticleEmmitor smokeEmmitor1 = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(40, 10, 10, 10));
                        smokeEmmitor1.Amount = 40;
                        smokeEmmitor1.MinLife = 1f;
                        smokeEmmitor1.MaxLife = 1.5f;
                        smokeEmmitor1.MinSize = asteroid.Radius / 20.0f;
                        smokeEmmitor1.MaxSize = asteroid.Radius / 30.0f;
                        smokeEmmitor1.SpawnRadius = asteroid.Radius / 4.0f;
                        smokeEmmitor1.Infinite = true;
                        ParticleEmmitor smokeEmmitor2 = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(40, 10, 10, 10));
                        smokeEmmitor2.Amount = 40;
                        smokeEmmitor2.MinLife = 1f;
                        smokeEmmitor2.MaxLife = 1.5f;
                        smokeEmmitor2.MinSize = asteroid.Radius / 20.0f;
                        smokeEmmitor2.MaxSize = asteroid.Radius / 30.0f;
                        smokeEmmitor2.SpawnRadius = asteroid.Radius / 4.0f;
                        smokeEmmitor2.Infinite = true;

                        meNode.AddEmmitor(smokeEmmitor1, new Vector(asteroid.Radius * -0.7, asteroid.Radius * 0.5), false);
                        meNode.AddEmmitor(smokeEmmitor2, new Vector(asteroid.Radius * -0.7, asteroid.Radius * -0.5), false);

                        asteroid.AddControl(new ShakingControl(1, true, 0.05f));
                    }
                    break;
                case 4:
                    {
                        ParticleEmmitor fireEmmitor = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(50, 255, 200, 0));
                        fireEmmitor.Amount = 20;
                        fireEmmitor.MinLife = 0.1f;
                        fireEmmitor.MaxLife = 0.2f;
                        fireEmmitor.MinSize = asteroid.Radius / 7.0f;
                        fireEmmitor.MaxSize = asteroid.Radius / 9.0f;
                        fireEmmitor.Infinite = true;

                        meNode.AddEmmitor(fireEmmitor, new Vector(asteroid.Radius, 0), false);
                    }
                    break;
            }
        }

        private void CheckSparkSpawn()
        {
            if (step < 1)
                return;

            if (meNode.SceneMgr.GetRandomGenerator().Next(100) < 20)
                CreateSpark();
        }

        private void CreateSpark()
        {
            ParticleNode node = new ParticleNode(me.SceneMgr, IdMgr.GetNewId(me.SceneMgr.GetCurrentPlayer().GetId()));
            node.Position = (meNode.Center - (meNode.Direction * (asteroid.Radius * 0.7f)));
            node.Direction = meNode.Direction;

            float minSize = (float)FastMath.LinearInterpolate(0.2, 0.8, me.SceneMgr.GetRandomGenerator().NextDouble());
            float maxSize = minSize * 1.2f;

            ParticleEmmitor smokeEmmitor = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(80, 0, 0, 0));
            smokeEmmitor.Amount = 20;
            smokeEmmitor.MinLife = 0.3f;
            smokeEmmitor.MaxLife = 0.4f;
            smokeEmmitor.MinSize = minSize * 1.3f;
            smokeEmmitor.MaxSize = maxSize * 1.3f;
            smokeEmmitor.SpawnRadius = 4f;
            smokeEmmitor.Infinite = true;

            ParticleEmmitor fireEmmitor = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(150, 255, 100, 0));
            fireEmmitor.Amount = 20;
            fireEmmitor.MinLife = 0.1f;
            fireEmmitor.MaxLife = 0.2f;
            fireEmmitor.MinSize = minSize * 1.1f;
            fireEmmitor.MaxSize = maxSize * 1.1f;
            fireEmmitor.Infinite = true;

            ParticleEmmitor fireEmmitor2 = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(200, 255, 200, 0));
            fireEmmitor2.Amount = 20;
            fireEmmitor2.MinLife = 0.1f;
            fireEmmitor2.MaxLife = 0.1f;
            fireEmmitor2.MinSize = minSize;
            fireEmmitor2.MaxSize = maxSize;
            fireEmmitor2.Infinite = true;

            EmmitorGroup grp = new EmmitorGroup();
            grp.Add(smokeEmmitor);
            grp.Add(fireEmmitor);
            grp.Add(fireEmmitor2);

            node.AddEmmitorGroup(grp, new Vector());

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.Speed = asteroid.GetControlOfType<IMovementControl>().Speed / 2.0f;
            node.AddControl(nmc);
            node.AddControl(new LimitedLifeControl((float)FastMath.LinearInterpolate(0.5, 2, me.SceneMgr.GetRandomGenerator().NextDouble())));

            me.SceneMgr.DelayedAttachToScene(node);
        }
    }
}

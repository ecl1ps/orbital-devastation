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
        private List<ParticleEmmitor> smokeEmmitors = new List<ParticleEmmitor>(3);

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
            if (!(me is ParticleNode))
                throw new ArgumentException("AsteroidBurningControl must by attached to an ParticleNode object");

            meNode = me as ParticleNode;

            events.AddEvent(1, new Event(0.5f, EventType.REPEATABLE, new Action(() => CheckSparkSpawn())));
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
            if (step > 0)
                UpdateSmokes(tpf);

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
                        ParticleEmmitor fireEmmitor = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(40, 255, 60, 0));
                        fireEmmitor.Amount = 20;
                        fireEmmitor.MinLife = 0.3f;
                        fireEmmitor.MaxLife = 0.4f;
                        fireEmmitor.MinSize = asteroid.Radius / 5.0f;
                        fireEmmitor.MaxSize = asteroid.Radius / 6.0f;
                        fireEmmitor.Infinite = true;

                        meNode.AddEmmitor(fireEmmitor, new Vector(0, 0), false);
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
        protected void UpdateSmokes(float tpf)
        {
            NewtonianMovementControl control = asteroid.GetControlOfType<NewtonianMovementControl>();
            if (control == null)
                return;

            double speed = control.RealSpeed / tpf;

            if (speed < 80 && smokeEmmitors.Count > 0)
            {
                foreach (ParticleEmmitor e in smokeEmmitors)
                    e.DelayedStop();

                smokeEmmitors.Clear();
                return;
            }

            int maxSmoke = GetMaxSmoke();
            if (smokeEmmitors.Count == maxSmoke)
                return;

            int num = (int) ((speed - 80) / 20);
            if (num > maxSmoke)
                num = maxSmoke;

            int diff = num - smokeEmmitors.Count;
            if (diff == 0)
                return;

            if (diff < 0)
            {
                for (int i = 1; i <= diff; i++)
                {
                    smokeEmmitors[smokeEmmitors.Count - i].DelayedStop();
                    smokeEmmitors.RemoveAt(smokeEmmitors.Count - i);
                }
            }
            else
            {
                for (int i = 0; i < diff; i++)
                {
                    smokeEmmitors.Add(CreateAndAddSmokeEmmitor(smokeEmmitors.Count));
                }
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

        private void CheckSparkSpawn()
        {
            if (step < 1)
                return;

            if (meNode.SceneMgr.GetRandomGenerator().Next(100) < 25)
                CreateSpark();
        }

        private void CreateSpark()
        {
            ParticleNode node = new ParticleNode(me.SceneMgr, IdMgr.GetNewId(me.SceneMgr.GetCurrentPlayer().GetId()));

            // TODO spawn in a whole volume
            bool left = me.SceneMgr.GetRandomGenerator().Next(2) == 0 ? false : true;
            if (left)
            {
                Vector position = (meNode.Center - (meNode.Direction.Rotate(Math.PI / 4) * (asteroid.Radius)));
                position += meNode.Direction * (asteroid.Radius * 1.5);
                node.Position = position;
                node.Direction = meNode.Direction.Rotate(FastMath.DegToRad(-10));
            }
            else
            {
                Vector position = (meNode.Center - (meNode.Direction.Rotate(-Math.PI / 4) * (asteroid.Radius)));
                position += meNode.Direction * (asteroid.Radius * 1.5);
                node.Position = position;
                node.Direction = meNode.Direction.Rotate(FastMath.DegToRad(10));
            }

            float minSize = (float)FastMath.LinearInterpolate(0.2, 0.5, me.SceneMgr.GetRandomGenerator().NextDouble());
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

            node.AddEmmitorGroup(grp, new Vector(), false);

            NewtonianMovementControl nmc = new NewtonianMovementControl();
            nmc.Speed = (asteroid.GetControlOfType<IMovementControl>().RealSpeed / tpf) * 0.75f;
            node.AddControl(nmc);
            node.AddControl(new LimitedLifeControl((float)FastMath.LinearInterpolate(0.5, 2, me.SceneMgr.GetRandomGenerator().NextDouble())));

            me.SceneMgr.DelayedAttachToScene(node);
	    }


        protected ParticleEmmitor CreateAndAddSmokeEmmitor(int num)
        {
            ParticleEmmitor smallSmokeEmmitor1 = ParticleEmmitorFactory.CreateSmokeParticleEmmitor(me.SceneMgr, asteroid.Position);
            smallSmokeEmmitor1.EmmitingDirection = asteroid.Direction.Rotate(Math.PI);
            smallSmokeEmmitor1.Infinite = true;
            smallSmokeEmmitor1.MinAngle = (float)FastMath.DegToRad(15);
            smallSmokeEmmitor1.MaxAngle = (float)FastMath.DegToRad(-15);
            smallSmokeEmmitor1.MinLife = 1.2f;
            smallSmokeEmmitor1.MaxLife = 1.5f;
            smallSmokeEmmitor1.Amount = 30;
            smallSmokeEmmitor1.MinSize *= 1.2f;
            smallSmokeEmmitor1.MaxSize *= 1.2f;

            EmmitorDirectionCloneControl control = new EmmitorDirectionCloneControl(me);
            control.DirectionOfsetRotation = (float)Math.PI;
            smallSmokeEmmitor1.AddControl(control);

            IMovementControl c = asteroid.GetControlOfType<IMovementControl>();
            if (num == 0)
                meNode.AddEmmitor(smallSmokeEmmitor1, new Vector(asteroid.Radius * 0.85, 0), false);
            else if (num == 1)
                meNode.AddEmmitor(smallSmokeEmmitor1, new Vector(asteroid.Radius * 0.85, asteroid.Radius * 0.2).Rotate((Math.PI / 4)), false);
            else if (num == 2)
                meNode.AddEmmitor(smallSmokeEmmitor1, new Vector(asteroid.Radius * 0.85, asteroid.Radius * 0.2).Rotate(-(Math.PI / 4)), false);

            return smallSmokeEmmitor1;
        }
    }
}

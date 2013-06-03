﻿using System;
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
            if (step > 0)
                UpdateSmokes(tpf);

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
                        ParticleEmmitor fireEmmitor = ParticleEmmitorFactory.CreateBasicFire(me.SceneMgr, Color.FromArgb(40, 255, 60, 0));
                        fireEmmitor.Amount = 20;
                        fireEmmitor.MinLife = 0.3f;
                        fireEmmitor.MaxLife = 0.4f;
                        fireEmmitor.MinSize = asteroid.Radius / 5.0f;
                        fireEmmitor.MaxSize = asteroid.Radius / 6.0f;
                        fireEmmitor.Infinite = true;

                        meNode.AddEmmitor(fireEmmitor, new Vector(0, 0), false);
                        
                        //CreateAndAddSmokeEmmitor(0);
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
                        //CreateAndAddSmokeEmmitor(1);
                    }
                    break;
                case 3:
                    {
                        //CreateAndAddSmokeEmmitor(2);
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

        protected void UpdateSmokes(float tpf)
        {
            NewtonianMovementControl control = asteroid.GetControlOfType<NewtonianMovementControl>();
            if(control == null)
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

        protected ParticleEmmitor CreateAndAddSmokeEmmitor(int num)
        {
            ParticleEmmitor smallSmokeEmmitor1 = ParticleEmmitorFactory.CreateSmokeParticleEmmitor(me.SceneMgr, asteroid.Position);
            smallSmokeEmmitor1.EmmitingDirection = asteroid.Direction.Rotate(Math.PI);
            smallSmokeEmmitor1.Infinite = true;
            smallSmokeEmmitor1.MinAngle = (float)FastMath.DegToRad(15);
            smallSmokeEmmitor1.MaxAngle = (float)FastMath.DegToRad(-15);
            smallSmokeEmmitor1.MinLife = 2;
            smallSmokeEmmitor1.MaxLife = 3;
            smallSmokeEmmitor1.Amount = 60;
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

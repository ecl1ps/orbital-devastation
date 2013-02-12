using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Particles.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Orbit.Core.Helpers
{
    class ParticleEmmitorFactory
    {
        public static ParticleEmmitor CreateExplodingSphereEmmitor(SceneMgr mgr, Vector position, Color color) 
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));
            e.EmitingTime = 1f;
            e.EmmitingDirection = new Vector(1, 0);
            e.MinAngle = (float)-Math.PI;
            e.MaxAngle = (float)Math.PI;
            e.MinForce = 10;
            e.MaxForce = 15;
            e.MinLife = 3f;
            e.MaxLife = 3.5f;
            e.Position = position;
            e.MinSize = 0.2f;
            e.MaxSize = 0.8f;
            e.Amount = 100;
            e.SizeMultiplier = 0;
            e.Infinite = false;
            e.FireAll = true;
            e.Enabled = true;

            ParticleSphereFactory f = new ParticleSphereFactory();
            f.Color = color;
            e.Factory = f;

            return e;
        }

        public static ParticleEmmitor CreateMovingSphereEmmitor(SceneMgr mgr, Vector position, Vector direction, Color color, float speed)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));
            e.Direction = direction;
            e.EmmitingDirection = direction.Rotate(180);
            e.MinAngle = (float)(-Math.PI);
            e.MaxAngle = (float)(Math.PI);
            e.MinForce = 1;
            e.MaxForce = 2;
            e.MinLife = 2f;
            e.MaxLife = 2.5f;
            e.Position = position;
            e.MinSize = 0.2f;
            e.MaxSize = 1;
            e.Amount = 250;
            e.SizeMultiplier = 0;
            e.Infinite = true;
            e.Enabled = true;

            ParticleSphereFactory f = new ParticleSphereFactory();
            f.Color = Color.FromArgb(125, 255, 20, 0);
            e.Factory = f;

            LinearMovementControl c = new LinearMovementControl();
            c.Speed = speed;
            e.AddControl(c);

            return e;
        }

        public static ParticleEmmitor CreateParticleSmokeEmmitor(SceneMgr mgr, Vector position)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));
            e.EmitingTime = 1f;
            e.EmmitingDirection = new Vector(0, -1);
            e.MinAngle = (float)(-Math.PI / 30);
            e.MaxAngle = (float)(Math.PI / 30);
            e.MinForce = 2;
            e.MaxForce = 4;
            e.MinLife = 3f;
            e.MaxLife = 3.5f;
            e.Position = position;
            e.MinSize = 1;
            e.MaxSize = 1.5f;
            e.Amount = 80;
            e.SizeMultiplier = 1;
            e.Infinite = true;
            e.Enabled = false;

            e.Factory = new ParticleSmokeFactory();
            return e;

        }

        public static ParticleEmmitor CrateFlashParticleEmmitor(SceneMgr mgr, Vector position, Color color)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));
            e.EmitingTime = 2f;
            e.EmmitingDirection = new Vector(0, 0);
            e.MinLife = 3f;
            e.MaxLife = 3.5f;
            e.Position = position;
            e.MinSize = 1;
            e.MaxSize = 1.5f;
            e.Amount = 80;
            e.SizeMultiplier = 0;
            e.Infinite = false;
            e.Enabled = true;

            ParticleImageFactory f = new ParticleImageFactory();
            f.Color = color;
            f.RenderSize = 100;
            f.Source = new Uri("pack://application:,,,/resources/images/particles/flash-particle.png");
            e.Factory = f;

            return e;

        }

        public static ParticleEmmitor CreateFireParticleEmmitor(SceneMgr mgr, Vector position, Color color)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));
            e.EmitingTime = 2f;
            e.EmmitingDirection = new Vector(0, -1);
            e.MinAngle = (float) -Math.PI;
            e.MaxAngle = (float) Math.PI;
            e.MinForce = 2f;
            e.MaxForce = 2.5f;
            e.MinLife = 3f;
            e.MaxLife = 3.5f;
            e.Position = position;
            e.MinSize = 1;
            e.MaxSize = 1.5f;
            e.Amount = 80;
            e.SizeMultiplier = 0;
            e.Infinite = false;
            e.Enabled = true;

            ParticleImageFactory f = new ParticleImageFactory();
            f.Color = color;
            f.RenderSize = 200;
            f.Source = new Uri("pack://application:,,,/resources/images/particles/fire-particle.png");
            e.Factory = f;

            return e;
        }

        public static ParticleEmmitor CreateShockWaveParticleEmmitor(SceneMgr mgr, Vector position, Color color)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));
            e.MinLife = 0.5f;
            e.MaxLife = 0.5f;
            e.Position = position;
            e.MinSize = 2;
            e.MaxSize = 2;
            e.Amount = 1;
            e.SizeMultiplier = 20;
            e.Infinite = false;
            e.FireAll = true;
            e.Enabled = true;

            ParticleImageFactory f = new ParticleImageFactory();
            f.Color = color;
            //f.Color = Colors.White;
            f.RenderSize = 100;
            f.Source = new Uri("pack://application:,,,/resources/images/particles/explosion-particle.png");
            e.Factory = f;

            return e;
        }

        public static List<ParticleEmmitor> CreateExplosionEmmitors(SceneMgr mgr, Vector position)
        {
            List<ParticleEmmitor> emmitors = new List<ParticleEmmitor>();

            ParticleEmmitor smokeEmmitor = CreateFireParticleEmmitor(mgr, position, Color.FromRgb(135, 135, 135));
            smokeEmmitor.EmitingTime = 1;
            smokeEmmitor.Amount = 20;
            smokeEmmitor.MinLife = 1.5f;
            smokeEmmitor.MaxLife = 2f;
            smokeEmmitor.MinSize = 5;
            smokeEmmitor.MaxSize = 6f;
            smokeEmmitor.MinForce = 3;
            smokeEmmitor.MaxForce = 4;
            smokeEmmitor.FireAll = true;

            ParticleEmmitor fireEmmitor = CreateFireParticleEmmitor(mgr, position, Color.FromArgb(255, 171, 0, 0));
            fireEmmitor.EmitingTime = 0.25f;
            fireEmmitor.Amount = 30;
            fireEmmitor.MinLife = 0.25f;
            fireEmmitor.MaxLife = 0.5f;
            fireEmmitor.MinSize = 2;
            fireEmmitor.MaxSize = 3;
            fireEmmitor.SizeMultiplier = 0.5f;

            ParticleEmmitor explosionSphere = CreateExplodingSphereEmmitor(mgr, position, Color.FromRgb(190, 120, 70));
            explosionSphere.Amount = 300;
            explosionSphere.MinSize = 0.5f;
            explosionSphere.MaxSize = 0.75f;
            explosionSphere.SizeMultiplier = 0;
            explosionSphere.MinLife = 1f;
            explosionSphere.MaxLife = 2.5f;

            ParticleEmmitor flashEmmitor = CrateFlashParticleEmmitor(mgr, position, Color.FromRgb(250, 250, 150));
            flashEmmitor.Amount = 1;
            flashEmmitor.MinSize = 2;
            flashEmmitor.MaxSize = 5;
            flashEmmitor.MinLife = 0.1f;
            flashEmmitor.MaxLife = 0.2f;
            flashEmmitor.SizeMultiplier = 1;
            flashEmmitor.FireAll = true;

            ParticleEmmitor shockWaveEmmitor = CreateShockWaveParticleEmmitor(mgr, position, Color.FromRgb(171, 0, 0)); 

            emmitors.Add(smokeEmmitor);
            //emmitors.Add(fireEmmitor);
            emmitors.Add(explosionSphere);
            emmitors.Add(flashEmmitor);
            emmitors.Add(shockWaveEmmitor);

            return emmitors;
        }

    }
}

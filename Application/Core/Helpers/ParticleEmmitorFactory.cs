using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Entities.Implementations;
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
            ParticleEmmitor e = CreateBasicSphere(mgr, color);
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

            return e;
        }

        public static ParticleEmmitor CreateMovingSphereEmmitor(SceneMgr mgr, Vector position, Vector direction, Color color, float speed)
        {
            ParticleEmmitor e = CreateBasicSphere(mgr, color);
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

            LinearMovementControl c = new LinearMovementControl();
            c.Speed = speed;
            e.AddControl(c);

            return e;
        }

        public static ParticleEmmitor CreateSmokeParticleEmmitor(SceneMgr mgr, Vector position)
        {
            ParticleEmmitor e = CreateBasicSmoke(mgr);
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

            return e;
        }

        public static ParticleEmmitor CreateFlashParticleEmmitor(SceneMgr mgr, Vector position, Color color)
        {
            ParticleEmmitor e = CreateBasicFlash(mgr, color);
            e.Amount = 1;
            e.MinSize = 5;
            e.MaxSize = 7;
            e.MinLife = 0.1f;
            e.MaxLife = 0.2f;
            e.SizeMultiplier = 1;
            e.MinStartingRotation = (float)-Math.PI;
            e.MaxStartingRotation = (float)Math.PI;
            e.FireAll = true;
            e.Position = position;

            return e;
        }

        public static ParticleEmmitor CreateFireParticleEmmitor(SceneMgr mgr, Vector position, Color color)
        {
            ParticleEmmitor e = CreateBasicFire(mgr, color);
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

            return e;
        }

        public static ParticleEmmitor CreateShockWaveParticleEmmitor(SceneMgr mgr, Vector position, Color color)
        {
            ParticleEmmitor e = CreateBasicShockwave(mgr, color);
            e.MinLife = 0.35f;
            e.MaxLife = 0.35f;
            e.Position = position;
            e.MinSize = 0.5f;
            e.MaxSize = 0.6f;
            e.Amount = 1;
            e.SizeMultiplier = 20;
            e.Infinite = false;
            e.FireAll = true;
            e.Enabled = true;

            return e;
        }

        public static ParticleEmmitor CreateBaseDebrisEmmitor(SceneMgr mgr, Vector position, Vector direction, Color color) 
        {
            ParticleEmmitor e = CreateBasicDebris(mgr, color);
            e.Position = position;
            e.Amount = 3;
            e.MaxLife = 1f;
            e.MinLife = 0.25f;
            e.MaxSize = 0.75f;
            e.MinSize = 0.75f;
            e.EmmitingDirection = direction;
            e.MinAngle = (float)-Math.PI / 4;
            e.MaxAngle = (float)Math.PI / 4;
            e.MaxStartingRotation = (float)Math.PI;
            e.MinStartingRotation = (float)-Math.PI;
            e.MinRotation = 0;
            e.MaxRotation = (float)Math.PI * 2;
            e.MinForce = 8;
            e.MaxForce = 10;
            e.FireAll = true;
            e.Enabled = true;

            return e;
        }

        public static EmmitorGroup CreateBulletImplosionEmmitor(SceneMgr mgr, Vector position, Color color)
        {
            ParticleEmmitor e1 = CreateShockWaveParticleEmmitor(mgr, position, color);
            e1.SizeMultiplier = 20;
            e1.MinLife = 0.25f;
            e1.MaxLife = 0.25f;
            e1.MinSize = 0.2f;
            e1.MaxSize = 0.2f;

            EmmitorGroup g = new EmmitorGroup();
            g.Add(e1);
            
            return g;
        }

        public static EmmitorGroup CreateExplosionEmmitors(SceneMgr mgr, Vector position)
        {
            ParticleEmmitor smokeEmmitor = CreateFireParticleEmmitor(mgr, position, Color.FromArgb(135, 0, 0, 0));
            smokeEmmitor.EmitingTime = 0.5f;
            smokeEmmitor.Amount = 20;
            smokeEmmitor.MinLife = 1.0f;
            smokeEmmitor.MaxLife = 1.2f;
            smokeEmmitor.MinSize = 5;
            smokeEmmitor.MaxSize = 6f;
            smokeEmmitor.MinForce = 3;
            smokeEmmitor.MaxForce = 4;
            smokeEmmitor.Delay = 0.2f;
            smokeEmmitor.FireAll = true;

            ParticleEmmitor fireEmmitor = CreateFireParticleEmmitor(mgr, position, Color.FromArgb(120, 255, 120, 0));
            fireEmmitor.EmitingTime = 0.5f;
            fireEmmitor.Amount = 20;
            fireEmmitor.MinLife = 0.8f;
            fireEmmitor.MaxLife = 1.0f;
            fireEmmitor.MinSize = 5;
            fireEmmitor.MaxSize = 6f;
            fireEmmitor.MinForce = 3;
            fireEmmitor.MaxForce = 4;
            fireEmmitor.Delay = 0.2f;
            fireEmmitor.FireAll = true;

            ParticleEmmitor flashEmmitor = CreateFlashParticleEmmitor(mgr, position, Color.FromRgb(250, 250, 150));

            ParticleEmmitor shockWaveEmmitor = CreateShockWaveParticleEmmitor(mgr, position, Color.FromArgb(150, 255, 200, 0));

            EmmitorGroup g = new EmmitorGroup();
            g.Position = position;
            g.Add(smokeEmmitor);
            g.Add(fireEmmitor);
            g.Add(flashEmmitor);
            g.Add(shockWaveEmmitor);

            return g;
        }

        public static EmmitorGroup CreateAsteroidExplosionEmmitors(SceneMgr mgr, Vector position)
        {
            ParticleEmmitor smokeEmmitor = CreateFireParticleEmmitor(mgr, position, Color.FromArgb(135, 0, 0, 0));
            smokeEmmitor.EmitingTime = 0.5f;
            smokeEmmitor.Amount = 20;
            smokeEmmitor.MinLife = 1.0f;
            smokeEmmitor.MaxLife = 1.2f;
            smokeEmmitor.MinSize = 5;
            smokeEmmitor.MaxSize = 6f;
            smokeEmmitor.MinForce = 3;
            smokeEmmitor.MaxForce = 4;
            smokeEmmitor.FireAll = true;

            ParticleEmmitor fireEmmitor = CreateFireParticleEmmitor(mgr, position, Color.FromArgb(120, 255, 120, 0));
            fireEmmitor.EmitingTime = 0.5f;
            fireEmmitor.Amount = 20;
            fireEmmitor.MinLife = 0.8f;
            fireEmmitor.MaxLife = 1.0f;
            fireEmmitor.MinSize = 5;
            fireEmmitor.MaxSize = 6f;
            fireEmmitor.MinForce = 3;
            fireEmmitor.MaxForce = 4;
            fireEmmitor.FireAll = true;

            EmmitorGroup g = new EmmitorGroup();
            g.Position = position;
            g.Add(smokeEmmitor);
            g.Add(fireEmmitor);

            return g;
        }

        public static EmmitorGroup CreateFireEmmitors(SceneMgr mgr, Vector position)
        {
            ParticleEmmitor smokeEmmitor = CreateFireParticleEmmitor(mgr, position, Color.FromArgb(80, 0, 0, 0));
            smokeEmmitor.Amount = 80;
            smokeEmmitor.MinLife = 6f;
            smokeEmmitor.MaxLife = 7f;
            smokeEmmitor.MinSize = 0.7f;
            smokeEmmitor.MaxSize = 1.2f;
            smokeEmmitor.MinForce = 2;
            smokeEmmitor.MaxForce = 3;
            smokeEmmitor.Direction = new Vector(0, -1);
            smokeEmmitor.MinAngle = (float)-Math.PI / 30;
            smokeEmmitor.MaxAngle = (float)Math.PI / 30;
            smokeEmmitor.Infinite = true;

            ParticleEmmitor fireEmmitor = CreateFireParticleEmmitor(mgr, position, Color.FromArgb(120, 255, 155, 0));
            fireEmmitor.Amount = 60;
            fireEmmitor.MinLife = 0.5f;
            fireEmmitor.MaxLife = 1.2f;
            fireEmmitor.MinSize = 1f;
            fireEmmitor.MaxSize = 1.5f;
            fireEmmitor.MinForce = 5;
            fireEmmitor.MaxForce = 6;
            fireEmmitor.Direction = new Vector(0, -1);
            fireEmmitor.MinAngle = (float)-Math.PI / 20;
            fireEmmitor.MaxAngle = (float)Math.PI / 20;
            fireEmmitor.Infinite = true;


            EmmitorGroup g = new EmmitorGroup();
            g.Position = position;
            g.Add(smokeEmmitor);
            g.Add(fireEmmitor);

            return g;
        }

        public static EmmitorGroup CreateBaseExplosionEmmitors(Base baze, Vector collision, Vector direction)
        {
            ParticleEmmitor f = CreateFlashParticleEmmitor(baze.SceneMgr, collision, Color.FromRgb(250, 250, 150));
            f.MinSize = 2;
            f.MaxSize = 3;

            ParticleEmmitor d = CreateBaseDebrisEmmitor(baze.SceneMgr, collision, direction, baze.Color);

            EmmitorGroup g = new EmmitorGroup();
            g.Add(f);
            g.Add(d);

            return g;
        }

        public static ParticleEmmitor CreateBasicFire(SceneMgr mgr, Color color)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));

            ParticleImageFactory f = new ParticleImageFactory();
            f.Color = color;
            f.RenderSize = 200;
            f.Source = new Uri("pack://application:,,,/resources/images/particles/fire-particle.png");
            e.Factory = f;

            return e;
        }

        public static ParticleEmmitor CreateBasicFlash(SceneMgr mgr, Color color)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));

            ParticleImageFactory f = new ParticleImageFactory();
            f.Color = color;
            f.RenderSize = 100;
            f.Source = new Uri("pack://application:,,,/resources/images/particles/flash-particle.png");
            e.Factory = f;

            return e;
        }

        public static ParticleEmmitor CreateBasicShockwave(SceneMgr mgr, Color color)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));

            ParticleImageFactory f = new ParticleImageFactory();
            f.Color = color;
            f.RenderSize = 100;
            f.Source = new Uri("pack://application:,,,/resources/images/particles/explosion-particle.png");
            e.Factory = f;

            return e;
        }

        public static ParticleEmmitor CreateBasicDebris(SceneMgr mgr, Color color)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));

            BaseParticleFactory f = new BaseParticleFactory();
            f.Color = color;
            e.Factory = f;

            return e;
        }

        public static ParticleEmmitor CreateBasicSmoke(SceneMgr mgr)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));

            e.Factory = new ParticleSmokeFactory();

            return e;
        }

        public static ParticleEmmitor CreateBasicSphere(SceneMgr mgr, Color color)
        {
            ParticleEmmitor e = new ParticleEmmitor(mgr, IdMgr.GetNewId(0));

            ParticleSphereFactory f = new ParticleSphereFactory();
            f.Color = color;
            e.Factory = f;

            return e;
        }
    }
}

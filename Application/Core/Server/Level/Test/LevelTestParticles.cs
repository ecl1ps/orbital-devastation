using Lidgren.Network;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Particles.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Orbit.Core.Server.Level.Test
{
    class LevelTestParticles : AbstractGameLevel
    {
        public static readonly LevelInfo Info = new LevelInfo(true, "[TEST] Particles");

        protected Random rand;
        private Color[] colors = { Colors.Red, Colors.Green, Colors.Blue, Colors.Brown, Colors.Pink };

        private enum Events
        {
            MOVING_PARTICLE_ADD,
            EXPLODING_EMITTOR_ADD
        }

        public LevelTestParticles(ServerMgr serverMgr)
            : base(serverMgr)
        {
            rand = mgr.GetRandomGenerator();
        }

        private void CreateMovingParticleEmmitor(Vector position, Vector direction, float speed, float time)
        {
            ParticleEmmitor e = new ParticleEmmitor(null, IdMgr.GetNewId(0));
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
            e.Enabled = false;

            ParticleSphereFactory f = new ParticleSphereFactory();
            f.Color = Color.FromArgb(125, 255, 20, 0);
            e.Factory = f;

            LinearMovementControl c = new LinearMovementControl();
            c.Speed = speed;
            e.AddControl(c);

            GameLevelManager.SendNewObject(mgr, e);
        }

        private ParticleEmmitor createConstantEmmitor(Vector position)
        {
            ParticleEmmitor e = new ParticleEmmitor(null, IdMgr.GetNewId(0));
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

        private void CreateConstantParticleEmmitor(Vector position)
        {
            ParticleEmmitor e = createConstantEmmitor(position);

            ParticleSphereFactory f = new ParticleSphereFactory();
            f.Color = Color.FromArgb(20, 0, 0, 0);
            e.Factory = f;

            GameLevelManager.SendNewObject(mgr, e);
        }

        private void CreateConstantSmokeEmmitor(Vector position)
        {
            ParticleEmmitor e = createConstantEmmitor(position);

            e.Factory = new ParticleSmokeFactory();

            GameLevelManager.SendNewObject(mgr, e);
        }

        private void CreateExplodingParticleEmmitor(Vector position)
        {
            ParticleEmmitor e = new ParticleEmmitor(null, IdMgr.GetNewId(0));
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

            byte r = (byte)rand.Next(0, 255);
            byte g = (byte)rand.Next(0, 255);
            byte b = (byte)rand.Next(0, 255);

            ParticleSphereFactory f = new ParticleSphereFactory();
            f.Color = colors[rand.Next(colors.Count())];
            e.Factory = f;

            GameLevelManager.SendNewObject(mgr, e);
        }

        private void CreateShockWaveParticleEmmitor(Vector position)
        {
            ParticleEmmitor e = new ParticleEmmitor(null, IdMgr.GetNewId(0));
            e.MinLife = 10f;
            e.MaxLife = 10f;
            e.Position = position;
            e.MinSize = 20;
            e.MaxSize = 20;
            e.Amount = 1;
            //e.SizeMultiplier = 20;
            e.Infinite = false;
            e.FireAll = true;
            e.Enabled = true;

            ParticleImageFactory f = new ParticleImageFactory();
            f.Color = Color.FromArgb(50, 227, 144, 61);
            //f.Color = Colors.White;
            f.Source = new Uri("pack://application:,,,/resources/images/particles/explosion-particle.png");
            e.Factory = f;

            GameLevelManager.SendNewObject(mgr, e);
        }


        public override void OnStart()
        {
            CreateConstantParticleEmmitor(new Vector(100, 500));
            CreateConstantSmokeEmmitor(new Vector(400, 500));
            CreateConstantParticleEmmitor(new Vector(800, 500));

            CreateMovingParticleEmmitor(new Vector(0, rand.NextDouble() * SharedDef.VIEW_PORT_SIZE.Height), new Vector(1, 0), 100, 5);
            events.AddEvent((int)Events.MOVING_PARTICLE_ADD, new Event(5, EventType.REPEATABLE, new Action(() =>
            {
                CreateMovingParticleEmmitor(new Vector(0, rand.NextDouble() * SharedDef.VIEW_PORT_SIZE.Height), new Vector(1, 0), 100, 5);
            })));
           
            events.AddEvent((int)Events.EXPLODING_EMITTOR_ADD, new Event(1, EventType.REPEATABLE, new Action(() =>
            {
                CreateExplodingParticleEmmitor(new Vector(rand.NextDouble() * SharedDef.VIEW_PORT_SIZE.Width, rand.NextDouble() * SharedDef.VIEW_PORT_SIZE.Height));
            })));
        }
    }
}

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

        private enum Events
        {
            MOVING_PARTICLE_ADD
        }

        public LevelTestParticles(ServerMgr serverMgr) : base(serverMgr)
        {
            rand = mgr.GetRandomGenerator();
        }

        private void CreateMovingParticleEmmitor(Vector position, Vector direction, float speed, float time)
        {
            ParticleEmmitor e = new ParticleEmmitor(null, IdMgr.GetNewId(0));
            e.Direction = direction;
            e.EmitingTime = 1f;
            e.EmmitingDirection = direction.Rotate(180);
            e.MinAngle = (float)(-Math.PI);
            e.MaxAngle = (float)(Math.PI);
            e.MinForce = 1;
            e.MaxForce = 2;
            e.MinLife = 2f;
            e.MaxLife = 2.5f;
            e.Position = position;
            e.MinSize = 1;
            e.MaxSize = 2;
            e.Amount = 150;
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

        private void CreateConstantParticleEmmitor(Vector position)
        {
            ParticleEmmitor e = new ParticleEmmitor(null, IdMgr.GetNewId(0));
            e.EmitingTime = 2f;
            e.EmmitingDirection = new Vector(0, -1);
            e.MinAngle = (float)(-Math.PI / 30);
            e.MaxAngle = (float)(Math.PI / 30);
            e.MinForce = 2;
            e.MaxForce = 4;
            e.MinLife = 3f;
            e.MaxLife = 3.5f;
            e.Position = position;
            e.MinSize = 3;
            e.MaxSize = 4;
            e.Amount = 80;
            e.Infinite = true;
            e.Enabled = false;

            ParticleSphereFactory f = new ParticleSphereFactory();
            f.Color = Color.FromArgb(20, 0, 0, 0);
            e.Factory = f;

            GameLevelManager.SendNewObject(mgr, e);
        }


        public override void OnStart()
        {
            CreateConstantParticleEmmitor(new Vector(100, 500));
            CreateConstantParticleEmmitor(new Vector(400, 500));
            CreateConstantParticleEmmitor(new Vector(800, 500));

            events.AddEvent((int)Events.MOVING_PARTICLE_ADD, new Event(3, EventType.REPEATABLE, new Action(() =>
            {
                CreateMovingParticleEmmitor(new Vector(0, rand.NextDouble() * SharedDef.VIEW_PORT_SIZE.Height), new Vector(1, 0), 100, 5);
            })));
        }
    }
}

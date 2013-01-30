using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Controls;

namespace Orbit.Core.Scene.Particles.Implementations
{
    public class ParticleEmmitor : SceneObject
    {
        private Random rand;
        private float timeLap = 0;
        private float time = 0;
        private List<IMovable> deadObjects;
        private List<IMovable> livingObjects;
        private int amount = 0;

        private IParticleFactory factory;
        public IParticleFactory Factory { get { return factory; } set { value.Init(this); factory = value; } }
        public Vector EmmitingDirection { get; set; }
        public float EmitingTime { get; set; }
        public float MinAngle { get; set; }
        public float MaxAngle { get; set; }
        public float MaxForce { get; set; }
        public float MinForce { get; set; }
        public float MaxLife { get; set; }
        public float MinLife { get; set; }
        public float MaxSize { get; set; }
        public float MinSize { get; set; }
        public float MaxAlpha { get; set; }
        public float MinAlpha { get; set; }
        public int Amount { get; set; }
        public bool FireAll { get; set; }
        public bool Infinite { get; set; }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                if (value)
                    amount = Amount;
                base.Enabled = value;
            }
        }

        public ParticleEmmitor(SceneMgr mgr, long id) : base(mgr, id)
        {
            MinAngle = 0;
            MaxAngle = 0;
            MaxForce = 0;
            MinForce = 0;
            MaxSize = 0;
            MinSize = 0;
            MaxAlpha = 0;
            MinAlpha = 0;
            base.Enabled = false;

            rand = mgr.GetRandomGenerator();
            livingObjects = new List<IMovable>();
            deadObjects = new List<IMovable>();
        }

        public override System.Windows.Vector Center
        {
            get { return Position; }
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            return Position.X > 0 && Position.Y > 0 && Position.X < screenSize.Width && Position.Y < screenSize.Height;
        }

        public override void UpdateGeometric()
        {
            //i dont have geometry
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);
            if (FireAll)
            {
                for (int i = 0; i < Amount; i++)
                    SpawnParticle();

                Enabled = false;
                return;
            }

            if (timeLap == 0)
            {
                timeLap = EmitingTime / Amount;
                time = timeLap;
            }

            if (time <= 0)
            {
                SpawnParticle();
                amount--;
                time = timeLap;
            }

            if (amount <= 0 && !Infinite)
            {
                Enabled = false;
                return;
            }

            time -= tpf;
        }

        public void PrepareParticles(int num)
        {
            for (int i = 0; i < num; i++)
            {
                double size = FastMath.LinearInterpolate(MinSize, MaxSize, rand.NextDouble());
                IMovable obj = Factory.CreateParticle((int)size);
                deadObjects.Add(obj);
            }
        }

        protected void SpawnParticle()
        {
            if (!SpawnDeadParticle())
            {
                List<IMovable> temp = livingObjects.FindAll(obj => obj.Dead);
                if (temp.Count > 0)
                {
                    foreach (IMovable obj in temp) {
                        deadObjects.Add(obj);
                        livingObjects.Remove(obj);
                    }
                }

                if (!SpawnDeadParticle())
                    CreateArticle();
            }

        }

        protected bool SpawnDeadParticle()
        {
            if (deadObjects.Count == 0)
                return false;

            IMovable obj = deadObjects[0];
            AttachControls(obj);

            SceneMgr.DelayedAttachToScene(obj);
            deadObjects.RemoveAt(0);

            return true;
        }

        protected void AttachControls(IMovable obj)
        {
            obj.RemoveControlsOfType<IControl>();
            double angle = FastMath.LinearInterpolate(MinAngle, MaxAngle, rand.NextDouble());
            double force = FastMath.LinearInterpolate(MinForce, MaxForce, rand.NextDouble());
            double life = FastMath.LinearInterpolate(MinLife, MaxLife, rand.NextDouble());

            obj.Position = Position;
            obj.Direction = EmmitingDirection.Rotate(angle);
            LinearMovementControl mc = new LinearMovementControl();
            mc.Speed = (float)force;

            obj.AddControl(mc);
            obj.AddControl(new LimitedLifeControl((float)life));
        }

        protected void CreateArticle()
        {
            double size = FastMath.LinearInterpolate(MinSize, MaxSize, rand.NextDouble());
            IMovable obj = Factory.CreateParticle((int)size);
            livingObjects.Add(obj);

            SceneMgr.DelayedAttachToScene(obj);
            AttachControls(obj);
        }
    }
}

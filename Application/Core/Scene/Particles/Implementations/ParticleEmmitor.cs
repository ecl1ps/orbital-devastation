using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.Scene.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using Petzold.Media3D;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Orbit.Gui;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Particles.Implementations
{
    class Particle
    {
        public Point3D Position { get; set; }
        public Vector Direction { get; set; }
        public double Force { get; set; }
        public double Life { get; set; }
        public double MaxLife { get; set; }
        public double Size { get; set; }
        public double StartingSize { get; set; }

        public double Rotation { get; set; }
        public double StartingRotation { get; set; }
    }

    public class ParticleEmmitor : SceneObject, ISendable, IEmpty
    {
        private Random rand;
        private float timeLap = 0;
        private float time = 0;
        private List<Particle> particles;
        private int amount = 0;
        private Viewport3D viewPort;
        private GeometryModel3D model;
        private bool started;
        private bool ending;
        private float delayTime;

        private Vector positionToSet;
        private Vector currentPosition;
        private ParticleArea particleArea;

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
        public float SizeMultiplier { get; set; }
        public float MinStartingRotation { get; set; }
        public float MaxStartingRotation { get; set; }
        public float MinRotation { get; set; }
        public float MaxRotation { get; set; }
        private float delay;
        public float Delay { get { return delay; } set { delay = value; delayTime = value; } }        
        public int Amount { get; set; }
        public bool FireAll { get; set; }
        public bool Infinite { get; set; }

        private Point3D position;
        public override Vector Position
        {
            get
            {
                return started ? currentPosition : positionToSet;
            }
            set
            {
                this.positionToSet = value;
            }
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                if (value)
                {
                    amount = Amount;
                    ending = false;
                }
                base.Enabled = value;
                
            }
        }

        public ParticleEmmitor(SceneMgr mgr, long id)
            : base(mgr, id)
        {
            MinAngle = 0;
            MaxAngle = 0;
            MaxForce = 0;
            MinForce = 0;
            MaxSize = 0;
            MinSize = 0;
            MinRotation = 0;
            MaxRotation = 0;
            MinStartingRotation = 0;
            MaxStartingRotation = 0;
            Delay = 0;
            EmitingTime = 1;
            SizeMultiplier = 1;
            positionToSet = new Vector(0, 0);
            started = false;
            ending = false;
            //base.Enabled = false;

            particles = new List<Particle>();
            if (mgr != null)
                rand = mgr.GetRandomGenerator();
        }

        public void Init(ParticleArea a)
        {
            a.BeginInvoke(new Action(() =>
            {
                model = new GeometryModel3D();
                model.Geometry = new MeshGeometry3D();

                DiffuseMaterial material = new DiffuseMaterial(Factory.CreateParticle());

                model.Material = material;

                particleArea = a;
                this.viewPort = a.ViewPort;
                a.WorldModels.Children.Add(model);
            }));
        }

        public override System.Windows.Vector Center
        {
            get { return Position; }
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            return !started || ending || Position.X >= 0 && Position.X <= screenSize.Width && Position.Y >= 0 && Position.Y <= screenSize.Height;
        }

        public override void UpdateGeometric()
        {
            if (model == null)
                return;

            Point3DCollection positions = new Point3DCollection();
            Int32Collection indices = new Int32Collection();
            PointCollection texcoords = new PointCollection();

            for (int i = 0; i < particles.Count; ++i)
            {
                int positionIndex = i * 4;
                int indexIndex = i * 6;
                Particle p = particles[i];

                ComputePositions(p, positions);

                System.Windows.Point t1 = new System.Windows.Point(0.0, 0.0);
                System.Windows.Point t2 = new System.Windows.Point(0.0, 1.0);
                System.Windows.Point t3 = new System.Windows.Point(1.0, 1.0);
                System.Windows.Point t4 = new System.Windows.Point(1.0, 0.0);

                texcoords.Add(t1);
                texcoords.Add(t2);
                texcoords.Add(t3);
                texcoords.Add(t4);

                indices.Add(positionIndex);
                indices.Add(positionIndex + 2);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 2);
            }

            ((MeshGeometry3D)model.Geometry).Positions = positions;
            ((MeshGeometry3D)model.Geometry).TriangleIndices = indices;
            ((MeshGeometry3D)model.Geometry).TextureCoordinates = texcoords;
        }

        private void ComputePositions(Particle p, Point3DCollection list)
        {
            double size = p.Size;
            Vector center = new Vector(p.Position.X, p.Position.Y);
            
            //najdeme body
            Vector p1 = new Vector(p.Position.X - size, p.Position.Y - size);
            Vector p2 = new Vector(p.Position.X - size, p.Position.Y + size);
            Vector p3 = new Vector(p.Position.X + size, p.Position.Y + size);
            Vector p4 = new Vector(p.Position.X + size, p.Position.Y - size);

            //pokud ma particle rotaci orotujeme
            if (p.Rotation > 0)
            {
                p1 = p1.Rotate(p.Rotation, center);
                p2 = p2.Rotate(p.Rotation, center);
                p3 = p3.Rotate(p.Rotation, center);
                p4 = p4.Rotate(p.Rotation, center);
            }

            double z = p.Position.Z;
            list.Add(new Point3D(p1.X, p1.Y, z));
            list.Add(new Point3D(p2.X, p2.Y, z));
            list.Add(new Point3D(p3.X, p3.Y, z));
            list.Add(new Point3D(p4.X, p4.Y, z));
        }

        public Point3D To3DPoint(Vector point)
        {
            LineRange r = new LineRange();
            ViewportInfo.Point2DtoPoint3D(viewPort, point.ToPoint(), out r);

            return r.PointFromZ(-100);
        }

        public Vector To2DPoint(Point3D point)
        {
            return viewPort == null ? new Vector() : ViewportInfo.Point3DtoPoint2D(viewPort, point).ToVector();
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);
            if (viewPort == null)
                return;

            position = To3DPoint(positionToSet);
            currentPosition = To2DPoint(position);

            started = true;
            if (Delay > 0)
            {
                Delay -= tpf;
                return;
            }

            if (FireAll && !ending)
            {
                for (int i = 0; i < Amount; i++)
                    SpawnParticle();

                Amount = 0;
                DelayedStop();
            }

            if (timeLap == 0)
            {
                timeLap = EmitingTime / Amount;
                time = timeLap;
            }

            if (time <= 0)
            {
                SpawnParticles(tpf);
                time = timeLap;
            }

            if (amount <= 0 && !Infinite)
            {
                DelayedStop();
            }

            if (ending && particles.Count == 0)
            {
                DoRemoveMe();
                return;
            }

            time -= tpf;
            UpdateParticles(tpf);
        }

        private void UpdateParticles(float tpf)
        {
            Particle p = null;
            for (int i = 0; i < particles.Count; i++)
            {
                p = particles[i];

                if (p.Life <= 0)
                {
                    particles.RemoveAt(i);
                    continue;
                }

                Vector v = (p.Direction * p.Force * tpf);
                p.Position += new Vector3D(v.X, v.Y, 0);
                p.Life -= tpf;

                p.Size = FastMath.LinearInterpolate(p.StartingSize * SizeMultiplier, p.StartingSize , p.Life / p.MaxLife);
                p.Rotation = p.StartingRotation + FastMath.LinearInterpolate(MinRotation, MaxRotation, p.Life / p.MaxLife);
            }
        }

        protected void SpawnParticles(float tpf)
        {
            int count = (int) (tpf / timeLap);
            if (count < 1)
                count = 1;

            for (int i = 0; i < count; i++)
                SpawnParticle();
        }

        protected void SpawnParticle()
        {
            if (ending)
                return;

            double angle = FastMath.LinearInterpolate(MinAngle, MaxAngle, rand.NextDouble());
            double force = FastMath.LinearInterpolate(MinForce, MaxForce, rand.NextDouble());
            double life = FastMath.LinearInterpolate(MinLife, MaxLife, rand.NextDouble());
            double size = FastMath.LinearInterpolate(MinSize, MaxSize, rand.NextDouble());
            double rotation = FastMath.LinearInterpolate(MinStartingRotation, MaxStartingRotation, rand.NextDouble());

            Particle p = new Particle();
            p.Life = life;
            p.Force = force;
            p.MaxLife = life;

            p.Position = position;
            p.Direction = EmmitingDirection.Rotate(angle);
            p.StartingRotation = rotation;

            Point3D point = To3DPoint(new Vector(Position.X + size, Position.Y));
            p.Size = size;
            p.StartingSize = p.Size;

            particles.Add(p);
            amount--;
        }

        private double get3dSize(Particle p)
        {
            Point3D p1 = To3DPoint(new Vector(0, 0));
            Point3D p2 = To3DPoint(new Vector(p.Size, 0));
            return p2.X - p1.X;
        }

        public void DelayedStop()
        {
            ending = true;
        }

        public void Start(bool send = false)
        {
            particles.Clear();
            ending = false;
            amount = Amount;
            delayTime = Delay;
            timeLap = 0;
            time = 0;

            if (Dead)
            {
                Dead = false;
                SceneMgr.DelayedAttachToScene(this);
            }

            if (send)
            {
                Lidgren.Network.NetOutgoingMessage msg = SceneMgr.CreateNetMessage();

                msg.Write((int)PacketType.PARTICLE_EMMITOR_CREATE);
                WriteMe(msg);

                SceneMgr.SendMessage(msg);
            }
        }

        public override void DoRemove(ISceneObject obj)
        {
            base.DoRemove(obj);

            particleArea.BeginInvoke(new Action(() => particleArea.WorldModels.Children.Remove(model)));
        }

        public void WriteObject(Lidgren.Network.NetOutgoingMessage msg)
        {
            msg.Write((int) PacketType.PARTICLE_EMMITOR_CREATE);
            WriteMe(msg);
           
        }

        private void WriteMe(Lidgren.Network.NetOutgoingMessage msg)
        {
            msg.Write(Id);
            msg.Write(EmmitingDirection);
            msg.Write(EmitingTime);
            msg.Write(MinAngle);
            msg.Write(MaxAngle);
            msg.Write(MaxForce);
            msg.Write(MinForce);
            msg.Write(MaxLife);
            msg.Write(MinLife);
            msg.Write(MaxSize);
            msg.Write(MinSize);
            msg.Write(MaxRotation);
            msg.Write(MinRotation);
            msg.Write(MaxStartingRotation);
            msg.Write(MinStartingRotation);
            msg.Write(Delay);
            msg.Write(SizeMultiplier);
            msg.Write(Amount);
            msg.Write(FireAll);
            msg.Write(Infinite);
            msg.Write(Position);
            msg.Write(Direction);

            NetDataHelper.WriteParticleFactory(msg, factory);
            NetDataHelper.WriteControls(msg, GetControlsCopy());
        }

        public void ReadMe(Lidgren.Network.NetIncomingMessage msg)
        {
            EmmitingDirection = msg.ReadVector();
            EmitingTime = msg.ReadFloat();
            MinAngle = msg.ReadFloat();
            MaxAngle = msg.ReadFloat();
            MaxForce = msg.ReadFloat();
            MinForce = msg.ReadFloat();
            MaxLife = msg.ReadFloat();
            MinLife = msg.ReadFloat();
            MaxSize = msg.ReadFloat();
            MinSize = msg.ReadFloat();
            MaxRotation = msg.ReadFloat();
            MinRotation = msg.ReadFloat();
            MaxStartingRotation = msg.ReadFloat();
            MinStartingRotation = msg.ReadFloat();
            Delay = msg.ReadFloat();
            SizeMultiplier = msg.ReadFloat();
            Amount = msg.ReadInt32();
            FireAll = msg.ReadBoolean();
            Infinite = msg.ReadBoolean();
            Position = msg.ReadVector();
            Direction = msg.ReadVector();

            Factory = NetDataHelper.ReadParticleFactory(msg);
            IList<IControl> controls = NetDataHelper.ReadControls(msg);

            foreach (IControl control in controls)
                AddControl(control);
        }

        public void ReadObject(Lidgren.Network.NetIncomingMessage msg)
        {
            Id = msg.ReadInt64();
            ReadMe(msg);
        }
    }
}

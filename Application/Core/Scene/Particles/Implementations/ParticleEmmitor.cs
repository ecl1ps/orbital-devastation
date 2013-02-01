﻿using System;
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

namespace Orbit.Core.Scene.Particles.Implementations
{
    class Particle
    {
        public Point3D Position { get; set; }
        public Vector Direction { get; set; }
        public double Force { get; set; }
        public double Life { get; set; }
        public double Size { get; set; }
    }

    public class ParticleEmmitor : SceneObject
    {
        private Random rand;
        private float timeLap = 0;
        private float time = 0;
        private Nullable<Vector> toConvert = null;
        private List<Particle> particles;
        private int amount = 0;
        private Viewport3D viewPort;
        private GeometryModel3D model;

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

        private Point3D position;
        public override Vector Position
        {
            get
            {
                return To2DPoint(position);
            }
            set
            {
                this.position = To3DPoint(value);
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
                    amount = Amount;
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
            MaxAlpha = 0;
            MinAlpha = 0;
            base.Enabled = false;

            rand = mgr.GetRandomGenerator();
            particles = new List<Particle>();
        }

        public void Init(ParticleArea a)
        {
            SceneMgr.BeginInvoke(new Action(() =>
            {
                model = new GeometryModel3D();
                model.Geometry = new MeshGeometry3D();

                UIElement elem = factory.CreateParticle(10);

                System.Windows.Media.Brush brush = null;

                RenderTargetBitmap renderTarget = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
                renderTarget.Render(elem);
                renderTarget.Freeze();
                brush = new ImageBrush(renderTarget);

                DiffuseMaterial material = new DiffuseMaterial(brush);

                model.Material = material;

                this.viewPort = a.ViewPort;
                a.WorldModels.Children.Add(model);

                if (toConvert != null)
                    Position = toConvert.Value;
            }));
        }

        public override System.Windows.Vector Center
        {
            get { return Position; }
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            return true;
        }

        public override void UpdateGeometric()
        {
            Point3DCollection positions = new Point3DCollection();
            Int32Collection indices = new Int32Collection();
            PointCollection texcoords = new PointCollection();

            for (int i = 0; i < particles.Count; ++i)
            {
                int positionIndex = i * 4;
                int indexIndex = i * 6;
                Particle p = particles[i];

                Point3D p1 = new Point3D(p.Position.X, p.Position.Y, p.Position.Z);
                Point3D p2 = new Point3D(p.Position.X, p.Position.Y + p.Size, p.Position.Z);
                Point3D p3 = new Point3D(p.Position.X + p.Size, p.Position.Y + p.Size, p.Position.Z);
                Point3D p4 = new Point3D(p.Position.X + p.Size, p.Position.Y, p.Position.Z);

                positions.Add(p1);
                positions.Add(p2);
                positions.Add(p3);
                positions.Add(p4);

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

        public Point3D To3DPoint(Vector point)
        {
            if (viewPort == null)
            {
                toConvert = point;
                return new Point3D();
            }

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
            }
        }

        protected void SpawnParticle()
        {
            double angle = FastMath.LinearInterpolate(MinAngle, MaxAngle, rand.NextDouble());
            double force = FastMath.LinearInterpolate(MinForce, MaxForce, rand.NextDouble());
            double life = FastMath.LinearInterpolate(MinLife, MaxLife, rand.NextDouble());

            Particle p = new Particle();
            p.Life = life;
            p.Force = force;
            p.Position = position;
            
            p.Direction = EmmitingDirection.Rotate(angle);
            p.Size = 4;

            particles.Add(p);
        }
    }
}

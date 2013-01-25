using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using Orbit.Gui.Visuals;
using Petzold.Media3D;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ParticleTest.xaml
    /// </summary>
    public partial class ParticleTest : UserControl
    {
        System.Windows.Threading.DispatcherTimer frameTimer;

        private Point3D spawnPoint;
        private double elapsed;
        private double totalElapsed;
        private int lastTick;
        private int currentTick;

        private int frameCount;
        private double frameCountTime;
        private int frameRate;

        private ParticleSystemManager pm;

        private Random rand;

        public ParticleTest()
        {
            InitializeComponent();

            frameTimer = new System.Windows.Threading.DispatcherTimer();
            frameTimer.Tick += OnFrame;
            frameTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            frameTimer.Start();

            this.spawnPoint = new Point3D(0.0, 0.0, 0.0);
            this.lastTick = Environment.TickCount;

            pm = new ParticleSystemManager();

            this.WorldModels.Children.Add(pm.CreateParticleSystem(1000, Colors.Gray));
            this.WorldModels.Children.Add(pm.CreateParticleSystem(1000, Colors.Red));
            this.WorldModels.Children.Add(pm.CreateParticleSystem(1000, Colors.Silver));
            this.WorldModels.Children.Add(pm.CreateParticleSystem(1000, Colors.Orange));
            this.WorldModels.Children.Add(pm.CreateParticleSystem(1000, Colors.Yellow));

            rand = new Random(this.GetHashCode());
        }

        private void OnFrame(object sender, EventArgs e)
        {
            // Calculate frame time;
            this.currentTick = Environment.TickCount;
            this.elapsed = (double)(this.currentTick - this.lastTick) / 1000.0;
            this.totalElapsed += this.elapsed;
            this.lastTick = this.currentTick;

            frameCount++;
            frameCountTime += elapsed;
            if (frameCountTime >= 1.0)
            {
                frameCountTime -= 1.0;
                frameRate = frameCount;
                frameCount = 0;
                fps.Text = frameRate.ToString();
            }

            pm.Update((float)elapsed);
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Red, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Orange, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Silver, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Gray, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Red, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Orange, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Silver, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Gray, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Red, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Yellow, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Silver, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Yellow, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Red, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Orange, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Silver, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Gray, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Red, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Orange, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Silver, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Gray, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Red, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Yellow, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Silver, rand.NextDouble(), 2.5 * rand.NextDouble());
            pm.SpawnParticle(this.spawnPoint, 10.0, Colors.Yellow, rand.NextDouble(), 2.5 * rand.NextDouble());

            double c = Math.Cos(this.totalElapsed);
            double s = Math.Sin(this.totalElapsed);

            Point3D p = To3DPoint(new Point(500, 200), World);
            Console.WriteLine(p.X + " " + p.Y + " " + p.Z);
            this.spawnPoint = p;
        }


        public Point3D To3DPoint(Point point, Viewport3D viewPort)
        {
            LineRange r = new LineRange();
            ViewportInfo.Point2DtoPoint3D(World, point, out r);

            return r.PointFromZ(-100);            
        }
       
    }
}

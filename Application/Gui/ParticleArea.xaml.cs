using Orbit.Core.Client.Interfaces;
using Orbit.Core.Scene.Particles.Implementations;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for ParticleArea.xaml
    /// </summary>
    public partial class ParticleArea : UserControl, Invoker
    {
        private int currentTick;
        private int lastTick;
        private int totalTicks;

        private static DispatcherTimer frameTimer;
        public static DispatcherTimer FrameTimer { get { return frameTimer; } }

        private List<ParticleEmmitor> toAdd;
        private List<ParticleEmmitor> emmitors;
        private List<ParticleEmmitor> toRemove;

        public Viewport3D ViewPort { get { return World; } }
        public Model3DGroup Models { get { return WorldModels; } }

        public ParticleArea()
        {
            InitializeComponent();

            frameTimer = new DispatcherTimer();
            FrameTimer.Tick += OnFrame;
            FrameTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            FrameTimer.Start();

            toAdd = new List<ParticleEmmitor>();
            emmitors = new List<ParticleEmmitor>();
            toRemove = new List<ParticleEmmitor>();

            lastTick = Environment.TickCount;
            totalTicks = 0;
        }


        private void OnFrame(object sender, EventArgs e)
        {
            // Calculate frame time;
            currentTick = Environment.TickCount;
            totalTicks++;
            double diff = (double)(this.currentTick - this.lastTick) / 1000.0;
            
            double tpf = diff / totalTicks;
            Update((float) tpf);
        }

        private void Update(float tpf)
        {
            AddEmmitors();

            emmitors.ForEach(e => e.Update(tpf));
            emmitors.ForEach(e => e.UpdateGeometric());

            RemoveEmmitors();
        }

        private void AddEmmitors()
        {
            foreach (ParticleEmmitor e in toAdd)
            {
                emmitors.Add(e);
                e.Init(this);
                e.OnAttach();
            }

            toAdd.Clear();
        }

        private void RemoveEmmitors()
        {
            foreach (ParticleEmmitor e in toRemove)
            {
                e.OnRemove();
                emmitors.Remove(e);
            }

            toRemove.Clear();
        }

        public void AddEmmitor(ParticleEmmitor emmitor)
        {
            toAdd.Add(emmitor);
        }

        public void RemoveEmmitor(ParticleEmmitor emmitor)
        {
            toRemove.Add(emmitor);
        }

        public void ClearAll()
        {
            toAdd.Clear();
            emmitors.Clear();
            toRemove.Clear();
        }

        public void Invoke(Action a)
        {
            FrameTimer.Dispatcher.Invoke(a);
        }

        public void BeginInvoke(Action a)
        {
            FrameTimer.Dispatcher.BeginInvoke(a);
        }

    }
}

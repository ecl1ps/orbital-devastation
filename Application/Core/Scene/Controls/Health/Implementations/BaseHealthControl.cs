using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Collisions;
using Orbit.Core.Scene.Controls.Health;
using Lidgren.Network;
using Orbit.Core.Client;
using Orbit.Core.Scene.Particles.Implementations;
using System.Windows;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Controls.Health.Implementations
{
    public class BaseHealthControl : Control, IHpControl
    {
        private Base baze;

        public int Hp { get { return baze.Owner.GetBaseIntegrity(); } set { baze.Owner.SetBaseIntegrity(value); } }
        public int MaxHp { get { return baze.Owner.Data.MaxBaseIntegrity; } set { baze.Owner.Data.MaxBaseIntegrity = value; } }

        private Stack<EmmitorGroup> emmitors;
        private Queue<EmmitorGroup> dead;

        private Random rand;

        protected override void InitControl(Entities.ISceneObject me)
        {
            if (me is Base)
                baze = me as Base;

            this.emmitors = new Stack<EmmitorGroup>();
            this.dead = new Queue<EmmitorGroup>();
            this.rand = me.SceneMgr.GetRandomGenerator();
        }

        public void RefillHp()
        {
        }

        protected override void UpdateControl(float tpf)
        {
            base.UpdateControl(tpf);

            if (MaxHp < 5)
                MaxHp = 5;

            int count = (int) ((MaxHp - Hp) / (MaxHp / 5));
            int size = 0;
            if (count > emmitors.Count)
            {
                size = count - emmitors.Count;
                for (int i = 0; i < size; i++)
                    SpawnEmmitor();
            }
            else if (count < emmitors.Count)
            {
                size = emmitors.Count - count;
                for (int i = 0; i < size; i++)
                    DespawnEmmitor();
            }
        }

        private void SpawnEmmitor()
        {
            Vector v = new Vector(1, 0);
            v = v.Rotate(FastMath.LinearInterpolate(0, -Math.PI, rand.NextDouble()));
            //v *= FastMath.LinearInterpolate(0, baze.Size.Height, rand.NextDouble());
            v *= baze.Size.Height;

            v.X += baze.Position.X + baze.Size.Width / 2;
            v.Y += baze.Position.Y + baze.Size.Height;

            if (dead.Count > 0)
            {
                EmmitorGroup g = dead.Dequeue();
                g.Position = v;

                g.Start();
                emmitors.Push(g);
            }
            else
                CreateEmmitor(v);
        }

        private void CreateEmmitor(Vector position)
        {
            EmmitorGroup g = ParticleEmmitorFactory.CreateFireEmmitors(me.SceneMgr, position);
            emmitors.Push(g);

            g.Attach(me.SceneMgr, false);
        }

        private void DespawnEmmitor()
        {
            if (emmitors.Count == 0)
                return;

            EmmitorGroup g = emmitors.Pop();
            g.Stop();

            dead.Enqueue(g);
        }
    }
}

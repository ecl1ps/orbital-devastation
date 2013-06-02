using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.Scene.Particles.Implementations
{
    public class ParticleNode : SceneObject, IEmpty
    {
        private class ParticleEmmitorTuple
        {
            public ParticleEmmitorTuple(EmmitorGroup group, Vector offset)
            {
                Emmitor = group;
                Offset = offset;
            }

            public EmmitorGroup Emmitor { get; set; }
            public Vector Offset { get; set; }
        }

        public override Vector Center { get { return Position; } }

        private List<ParticleEmmitorTuple> emmitors = new List<ParticleEmmitorTuple>();

        public ParticleNode(SceneMgr mgr, long id) : base(mgr, id)
        {
        }

        public override bool IsOnScreen(System.Windows.Size screenSize)
        {
            return true;
        }

        public override void UpdateGeometric()
        {            
        }

        public override void Update(float tpf)
        {
            base.Update(tpf);

            foreach (ParticleEmmitorTuple t in emmitors)
            {
                // TODO: radius
                t.Emmitor.Position = Position + t.Offset.Rotate(Rotation);
            }
        }

        public override void OnAttach()
        {
            foreach (ParticleEmmitorTuple t in emmitors)
                t.Emmitor.Start();
        }

        public override void OnRemove()
        {
            foreach (ParticleEmmitorTuple t in emmitors)
                t.Emmitor.Stop();
        }

        public void AddEmmitor(ParticleEmmitor emmitor, Vector offset, bool send = true)
        {
            EmmitorGroup grp = new EmmitorGroup();
            grp.Add(emmitor);
            AddEmmitorGroup(grp, offset, send);
        }

        public void AddEmmitorGroup(EmmitorGroup group, Vector offset, bool send = true)
        {
            if (offset == null)
                offset = new Vector(0, 0);

            emmitors.Add(new ParticleEmmitorTuple(group, offset));
            group.Attach(SceneMgr, send);
        }
    }
}

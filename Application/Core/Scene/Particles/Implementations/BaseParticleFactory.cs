using Orbit.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Orbit.Core.Scene.Particles.Implementations
{
    class BaseParticleFactory : AbstractParticleFactory
    {
        public Color Color { get; set; }

        public override Brush CreateParticle()
        {
            return ParticleGeometryFactory.CreateBaseImageParticle(Color);
        }

        public override void WriteObject(Lidgren.Network.NetOutgoingMessage msg)
        {
            msg.Write(Color);
        }

        public override void ReadObject(Lidgren.Network.NetIncomingMessage msg)
        {
            Color = msg.ReadColor();
        }
    }
}

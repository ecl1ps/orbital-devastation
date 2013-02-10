using Orbit.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Orbit.Core.Scene.Particles.Implementations
{
    class ParticleImageFactory : AbstractParticleFactory
    {
        public Color Color { get; set; }
        public Uri Source { get; set; }
        public int RenderSize { get; set; }

        public override Brush CreateParticle()
        {
            return ParticleGeometryFactory.CreateImageParticle(Color, Source, RenderSize);
        }

        public override void WriteObject(Lidgren.Network.NetOutgoingMessage msg)
        {
            msg.Write(Color);
            msg.Write(RenderSize);
            msg.Write(Source.OriginalString);
        }

        public override void ReadObject(Lidgren.Network.NetIncomingMessage msg)
        {
            Color = msg.ReadColor();
            RenderSize = msg.ReadInt32();
            Source = new Uri(msg.ReadString());
        }
    }
}

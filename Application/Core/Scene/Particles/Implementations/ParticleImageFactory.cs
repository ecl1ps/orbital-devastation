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

        public override System.Windows.UIElement CreateParticle(double size)
        {
            return HeavyweightGeometryFactory.CreateImageParticle(size, Color, Source);
        }

        public override void WriteObject(Lidgren.Network.NetOutgoingMessage msg)
        {
            msg.Write(Color);
            msg.Write(Source.OriginalString);
        }

        public override void ReadObject(Lidgren.Network.NetIncomingMessage msg)
        {
            Color = msg.ReadColor();
            Source = new Uri(msg.ReadString());
        }
    }
}

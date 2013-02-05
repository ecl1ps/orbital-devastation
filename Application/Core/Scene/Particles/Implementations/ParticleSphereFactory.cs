﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Helpers;
using System.Windows.Media;

namespace Orbit.Core.Scene.Particles.Implementations
{
    class ParticleSphereFactory : AbstractParticleFactory
    {
        public Color Color { get; set; }

        public override System.Windows.UIElement CreateParticle()
        {
            return HeavyweightGeometryFactory.CreateConstantColorCircleGeometry(20, Color);
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

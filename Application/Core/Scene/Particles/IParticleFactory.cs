using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Particles.Implementations;
using System.Windows;
using System.Windows.Media;

namespace Orbit.Core.Scene.Particles
{
    public interface IParticleFactory : ISendable
    {
        Brush CreateParticle();

        void Init(ParticleEmmitor emmitor);
    }
}

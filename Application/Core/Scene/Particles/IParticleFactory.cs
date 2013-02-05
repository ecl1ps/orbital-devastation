using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Particles.Implementations;
using System.Windows;

namespace Orbit.Core.Scene.Particles
{
    public interface IParticleFactory : ISendable
    {
        UIElement CreateParticle();

        void Init(ParticleEmmitor emmitor);
    }
}

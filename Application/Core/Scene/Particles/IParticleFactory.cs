using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Particles.Implementations;

namespace Orbit.Core.Scene.Particles
{
    public interface IParticleFactory
    {
        IMovable CreateParticle(int size);

        void Init(ParticleEmmitor emmitor);
    }
}

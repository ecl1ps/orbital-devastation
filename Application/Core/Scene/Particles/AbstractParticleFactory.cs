using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;

namespace Orbit.Core.Scene.Particles
{
    public abstract class AbstractParticleFactory : IParticleFactory
    {
        protected SceneMgr SceneMgr;

        public virtual void Init(Implementations.ParticleEmmitor emmitor)
        {
            SceneMgr = emmitor.SceneMgr;
        }

        public abstract Entities.IMovable CreateParticle(int size);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows;

namespace Orbit.Core.Scene.Particles
{
    public abstract class AbstractParticleFactory : IParticleFactory
    {
        protected SceneMgr SceneMgr;

        public virtual void Init(Implementations.ParticleEmmitor emmitor)
        {
            SceneMgr = emmitor.SceneMgr;
        }

        public abstract UIElement CreateParticle(int size);
    }
}

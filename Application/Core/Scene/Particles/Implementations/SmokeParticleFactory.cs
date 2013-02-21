using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Orbit.Core.Scene.Particles.Implementations
{
    class ParticleSmokeFactory : ParticleImageFactory
    {
        public ParticleSmokeFactory()
        {
            Color = Color.FromArgb(80, 0, 0, 0);
            Source = new Uri("pack://application:,,,/resources/images/particles/particle_cloud.png");
            RenderSize = 512;
        }
    }
}

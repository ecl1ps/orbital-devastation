﻿using System;
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
            Color = Colors.Black;
            Source = new Uri("pack://application:,,,/resources/images/particles/particle_cloud.png");
        }
    }
}

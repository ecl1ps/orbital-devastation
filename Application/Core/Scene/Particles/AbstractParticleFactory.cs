﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using System.Windows;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Particles
{
    public abstract class AbstractParticleFactory : IParticleFactory
    {
        protected SceneMgr SceneMgr;

        public virtual void Init(Implementations.ParticleEmmitor emmitor)
        {
            SceneMgr = emmitor.SceneMgr;
        }

        public abstract UIElement CreateParticle(double size);

        public abstract void WriteObject(Lidgren.Network.NetOutgoingMessage msg);

        public abstract void ReadObject(Lidgren.Network.NetIncomingMessage msg);

    }
}

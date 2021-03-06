﻿using Lidgren.Network;
using Orbit.Core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orbit.Core.Scene.Particles.Implementations
{
    public class EmmitorGroup
    {
        private Vector position;
        public Vector Position { get { return position; } set { list.ForEach(e => e.Position = value); position = value; } }
        private List<ParticleEmmitor> list = new List<ParticleEmmitor>();

        public void Add(ParticleEmmitor e)
        {
            list.Add(e);
        }

        public void Attach(SceneMgr mgr, bool send = true)
        {
            mgr.GetParticleArea().BeginInvoke(new Action(() =>
            {
                list.ForEach(e =>
                {
                    e.Enabled = true;
                    mgr.DelayedAttachToScene(e);

                    if (send)
                    {
                        NetOutgoingMessage msg = mgr.CreateNetMessage();
                        e.WriteObject(msg);

                        mgr.SendMessage(msg);
                    }
                });
            }));
        }

        public List<ParticleEmmitor> GetList()
        {
            return list;
        }

        public void Stop()
        {
            list.ForEach(e => e.DelayedStop());
        }

        public void Start()
        {
            list.ForEach(e => e.Start());
        }

    }
}

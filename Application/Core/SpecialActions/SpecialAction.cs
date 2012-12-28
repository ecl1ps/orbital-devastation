﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Lidgren.Network;
using System.Windows.Media;

namespace Orbit.Core.SpecialActions
{
    public abstract class SpecialAction : ISpecialAction
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public String Name { get; set; }
        public String ImageSource { get; set; }
        public SpecialActionType Type { get; set; }
        public float Cost { get; set; }
        private float timer = 0;
        public float RemainingCooldown { get { return timer; } set { timer = value; } }
        public float Cooldown { get; set; }
        public Color BackgroundColor { get; set; }

        public SpecialAction(SceneMgr mgr, Player owner)
        {
            Owner = owner;
            SceneMgr = mgr;
        }

        public virtual void StartAction()
        {
            Owner.AddGoldAndShow((int) -Cost);
            timer = Cooldown;
        }

        public abstract bool IsReady();

        public virtual void Update(float tpf)
        {
            if (timer > 0)
            {
                timer -= tpf;
                if (timer < 0)
                    timer = 0;
            }
        }

        public bool IsOnCooldown()
        {
            return timer != 0;
        }
    }
}

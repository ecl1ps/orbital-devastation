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

        private List<ISpecialAction> shared;

        public SpecialAction(SceneMgr mgr, Player owner, params ISpecialAction[] sharedActions)
        {
            Owner = owner;
            SceneMgr = mgr;

            shared = new List<ISpecialAction>(sharedActions);
        }

        public virtual void StartAction()
        {
            Owner.AddGoldAndShow((int) -Cost);
            StartCoolDown();
            shared.ForEach(a => a.StartCoolDown());
            SceneMgr.StatisticsMgr.Actions.Add(this);
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


        public void StartCoolDown()
        {
            timer = Cooldown;
        }

        public void AddSharedAction(ISpecialAction a)
        {
            shared.Add(a);
        }

        public void removeSharedAction(ISpecialAction a)
        {
            shared.Remove(a);
        }
    }
}

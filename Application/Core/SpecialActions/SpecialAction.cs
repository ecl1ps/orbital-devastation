using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Lidgren.Network;
using System.Windows.Media;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Helpers;

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
            Owner.Statistics.Actions.Add(this);
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

        public virtual void WriteObject(NetOutgoingMessage msg)
        {
            msg.Write(Name);
            msg.Write(ImageSource);
            msg.Write((int)Type);
            msg.Write(Cost);
            msg.Write(Cooldown);
            msg.Write(BackgroundColor);
        }

        public virtual void ReadObject(NetIncomingMessage msg)
        {
            Name = msg.ReadString();
            ImageSource = msg.ReadString();
            Type = (SpecialActionType)msg.ReadInt32();
            Cost = msg.ReadFloat();
            Cooldown = msg.ReadFloat();
            BackgroundColor = msg.ReadColor();
        }
    }
}

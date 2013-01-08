using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Gui;
using System.Windows;
using Orbit.Core.Helpers;
using Lidgren.Network;

namespace Orbit.Core.Client.GameStates
{
    public class AlertMessage
    {
        public String Text { get; set; }
        public float Time { get; set; }

        public AlertMessage(String text, float time)
        {
            Text = text;
            Time = time;
        }
    }

    public class AlertMessageManager : IGameState
    {
        public const float TIME_FASTER = 1;
        public const float TIME_FAST = 2;
        public const float TIME_NORMAL = 3;
        public const float TIME_LONG = 4;
        public const float TIME_LONGER = 5;

        private const float STARTING_ENDING_TIME = 0.25f;

        public float AnimationTime { get; set; }

        private float timer = 0;
        private AlertBox element = null;

        private bool slippingIn = false;
        private bool starting = false;
        private bool opening = false;
        private bool closing = false;
        private bool showing = false;
        private bool ending = false;
        private bool slippingOut = false;
        private bool ready = true;

        private SceneMgr mgr;
        private List<AlertMessage> messages;

        public AlertMessageManager(SceneMgr mgr, float animationTime)
        {
            this.mgr = mgr;
            this.messages = new List<AlertMessage>();
            AnimationTime = animationTime;
        }

        public void InitElement()
        {
            mgr.BeginInvoke(new Action(() =>
            {
                element = (AlertBox)LogicalTreeHelper.FindLogicalNode(mgr.GetCanvas(), "AlertBoxUC");
                if (element == null)
                {
                    Size size = SharedDef.VIEW_PORT_SIZE;
                    element = GuiObjectFactory.CreateAndAddAlertBox(mgr, new Vector(0, 0));
                }
            }));
        }

        public void Update(float tpf)
        {
            if ((ready && messages.Count > 0) || slippingIn)
                SlipIn(tpf);
            else if (starting)
                Start(tpf);
            else if (opening)
                Open(tpf);
            else if (showing)
                Display(tpf);
            else if (closing)
                Close(tpf);
            else if (ending)
                Ending(tpf);
            else if (slippingOut)
                SlipOut(tpf);
        }

        private void SlipIn(float tpf)
        {
            if (!slippingIn)
            {
                timer = AnimationTime;
                slippingIn = true;
                ready = false;
            }

            if (timer <= 0)
            {
                slippingIn = false;
                Start(tpf);
                return;
            }

            timer -= tpf;
            mgr.BeginInvoke(new Action(() => { element.Slip(timer / AnimationTime); }));
        }

        private void Start(float tpf)
        {
            if (!starting)
            {
                timer = STARTING_ENDING_TIME;
                starting = true;
                ready = false;
            }

            if (timer <= 0)
            {
                starting = false;
                Open(tpf);
                return;
            }

            timer -= tpf;
        }

        private void Open(float tpf)
        {
            if (!opening)
            {
                timer = AnimationTime;
                opening = true;
                mgr.BeginInvoke(new Action(() =>
                {
                    element.SetText(messages[0].Text);
                }));
            }

            if (timer <= 0)
            {
                opening = false;
                Display(tpf);
                return;
            }

            timer -= tpf;
            Animate(true);
        }

        private void Display(float tpf)
        {
            if (!showing)
            {
                showing = true;
                timer = getFirstItem().Time;
            }

            if (timer <= 0)
            {
                showing = false;
                Close(tpf);
                return;
            }

            timer -= tpf;
        }

        private void Close(float tpf)
        {
            if (!closing)
            {
                closing = true;
                timer = AnimationTime;
            }

            if (timer <= 0)
            {
                closing = false;
                Ending(tpf);
                return;
            }

            timer -= tpf;
            Animate(false);
        }

        private void Ending(float tpf)
        {
            if (!ending)
            {
                ending = true;
                timer = STARTING_ENDING_TIME;
            }

            if (timer <= 0)
            {
                ending = false;
                SlipOut(tpf);
                return;
            }

            timer -= tpf;
        }

        private void SlipOut(float tpf)
        {
            if (!slippingOut)
            {
                timer = AnimationTime;
                slippingOut = true;
            }

            if (timer <= 0)
            {
                slippingOut = false;
                ready = true;
                mgr.BeginInvoke(new Action(() => { element.Slip(1); }));
                return;
            }

            timer -= tpf;
            mgr.BeginInvoke(new Action(() => { element.Slip(1 - (timer / AnimationTime)); }));
        }

        private void Animate(bool open)
        {
            mgr.BeginInvoke(new Action(() =>
            {
                if (open)
                    element.OpenDoors(timer / AnimationTime);
                else
                    element.OpenDoors(1 - (timer / AnimationTime));
            }));
        }

        private AlertMessage getFirstItem()
        {
            if (messages.Count == 0)
                return null;

            AlertMessage temp = messages[0];
            messages.Remove(temp);

            return temp;
        }

        public void Show(String text, float time)
        {
            messages.Add(new AlertMessage(text, time));
        }

        public NetOutgoingMessage CreateShowMessage(String text, float time)
        {
            NetOutgoingMessage msg = mgr.CreateNetMessage();
            msg.Write((int)PacketType.SHOW_ALLERT_MESSAGE);
            msg.Write(text);
            msg.Write(time);

            return msg;
        }

        public void ReceiveShowMessage(NetIncomingMessage msg)
        {
            Show(msg.ReadString(), msg.ReadFloat());
        }
    }
}

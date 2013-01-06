using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Gui;
using System.Windows;
using Orbit.Core.Helpers;

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
        public float AnimationTime { get; set; }

        private float timer = 0;
        private AlertBox element = null;

        private bool opening = false;
        private bool closing = false;
        private bool showing = false;
        private bool ready = true;

        private SceneMgr mgr;
        private List<AlertMessage> messages;

        public AlertMessageManager(SceneMgr mgr, float animationTime)
        {
            this.mgr = mgr;
            this.messages = new List<AlertMessage>();
            AnimationTime = animationTime;

            InitElement();
        }

        private void InitElement()
        {
            mgr.BeginInvoke(new Action(() =>
            {
                element = (AlertBox)LogicalTreeHelper.FindLogicalNode(mgr.GetCanvas(), "AlertBoxUC");
                if (element == null)
                {
                    Size size = SharedDef.VIEW_PORT_SIZE;
                    GuiObjectFactory.CreateAndAddAlertBox(mgr, new Vector(size.Width * 0.2, size.Height * 0.2));
                }
            }));
        }

        public void Update(float tpf)
        {
            if (ready && messages.Count > 0)
                Open(tpf);
            else if (opening)
                Open(tpf);
            else if (showing)
                Display(tpf);
            else if (closing)
                Close(tpf);
        }

        private void Open(float tpf)
        {
            if (ready && !opening)
            {
                timer = AnimationTime;
                ready = false;
                opening = true;
                element.SetText(messages[0].Text);
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
                ready = true;
                return;
            }

            timer -= tpf;
            Animate(false);
        }

        private void Animate(bool open)
        {
            mgr.BeginInvoke(new Action(() => {
                if (open)
                    element.OpenDoors(1 - (timer / AnimationTime));
                else
                    element.OpenDoors(timer / AnimationTime);
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
    }
}

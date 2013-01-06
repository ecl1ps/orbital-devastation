using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Gui;

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
            throw new NotImplementedException();
        }

        private AlertMessage getFirstItem()
        {
            throw new NotImplementedException();
        }

        public void Show(String text, float time)
        {
            messages.Add(new AlertMessage(text, time));
        }
    }
}

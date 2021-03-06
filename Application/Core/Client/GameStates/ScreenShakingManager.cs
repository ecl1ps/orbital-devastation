﻿using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orbit.Core.Client.GameStates
{
    public enum ShakePower
    {
        WEAK,
        MODERATE,
        STRONG,
    }

    public class ScreenShakingManager : IGameState
    {
        public SceneMgr SceneMgr { get; set; }

        private bool shaking;

        private float limit;
        private float current;
        private bool rising;

        public ScreenShakingManager(SceneMgr mgr)
        {
            SceneMgr = mgr;

            shaking = false;
            rising = false;

            limit = 0;
            current = 0;
        }

        public void Update(float tpf)
        {
            if (shaking)
                Shake(tpf);
        }


        private void Shake(float tpf)
        {
            if (!shaking)
                return;

            if (rising && current > limit)
            {
                limit = -limit;
                limit += 1;
                rising = false;
            }

            if (!rising && current < limit)
            {
                limit = -limit;
                limit -= 1;
                rising = true;
            }

            if (limit < 2 && limit > -2)
            {
                shaking = false;
                current = 0;
                Animate();
                return;
            }

            if (rising)
                current += 250 * tpf;
            else
                current -= 250 * tpf;

            Animate();
        }

        private void Animate()
        {
            SceneMgr.BeginInvoke(new Action(() => {
                SceneMgr.GetCanvas().Margin = new Thickness(current, 0, 0, 0);
                SceneMgr.GetGameVisualArea().Margin = new Thickness(current, 0, 0, 0);
            }));
        }

        public void Start(float strenght, bool send = false)
        {
            limit = strenght;
            rising = true;
            shaking = true;

            if (send)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.SHAKING_START);
                msg.Write(strenght);

                SceneMgr.SendMessage(msg);
            }
        }

        public void Start(ShakePower power, bool send = false)
        {
            switch (power)
            {
                case ShakePower.WEAK:
                    Start(5, send);
                    break;

                case ShakePower.MODERATE:
                    Start(10, send);
                    break;

                case ShakePower.STRONG:
                    Start(20, send);
                    break;
            }
        }
    }
}

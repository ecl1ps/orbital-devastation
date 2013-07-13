using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Lidgren.Network;
using Orbit.Core.Helpers;
using Microsoft.Xna.Framework;

namespace Orbit.Core.Scene.Controls.Implementations
{
    public class ControlableDeviceControl : Control, IControledDevice
    {
        private bool isMovingDown = false;
        public bool IsMovingDown
        {
            get
            {
                return isMovingDown;
            }
            set
            {
                if (isMovingDown != value)
                {
                    isMovingDown = value;
                    SendMovingTypeChanged();
                }
            }
        }
        private bool isMovingTop = false;
        public bool IsMovingTop
        {
            get
            {
                return isMovingTop;
            }

            set
            {
                if (isMovingTop != value)
                {
                    isMovingTop = value;
                    SendMovingTypeChanged();
                }
            }
        }
        private bool isMovingLeft = false;
        public bool IsMovingLeft
        {
            get
            {
                return isMovingLeft;
            }
            set
            {
                if (isMovingLeft != value)
                {
                    isMovingLeft = value;
                    SendMovingTypeChanged();
                }
            }
        }
        private bool isMovingRight = false;
        public bool IsMovingRight
        {
            get
            {
                return isMovingRight;
            }
            set
            {
                if (isMovingRight != value)
                {
                    isMovingRight = value;
                    SendMovingTypeChanged();
                }
            }
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                if (!value)
                {
                    isMovingDown = false;
                    isMovingLeft = false;
                    isMovingRight = false;
                    isMovingTop = false;

                    //sendMovingTypeChanged();
                }

                base.Enabled = value;
            }
        }

        protected override void UpdateControl(float tpf)
        {
            Vector2 botVector = new Vector2(0, SharedDef.SPECTATOR_MODULE_SPEED * tpf);
            Vector2 rightVector = new Vector2(SharedDef.SPECTATOR_MODULE_SPEED * tpf, 0);

            if (IsMovingTop && (me.Position.Y - botVector.Y) > 0)
                me.Position -= botVector;
            if (IsMovingDown && (me.Position.Y + botVector.Y) < SharedDef.VIEW_PORT_SIZE.Height)
                me.Position += botVector;
            if (IsMovingLeft && (me.Position.X - rightVector.X) > 0)
                me.Position -= rightVector;
            if (IsMovingRight && (me.Position.X + rightVector.X) < SharedDef.VIEW_PORT_SIZE.Width)
                me.Position += rightVector;
        }

        private void SendMovingTypeChanged()
        {
            if (!Enabled)
                return;

            NetOutgoingMessage msg = me.SceneMgr.CreateNetMessage();
            msg.Write((int) PacketType.MOVE_STATE_CHANGED);
            msg.Write(me.SceneMgr.GetCurrentPlayer().GetId());
            msg.Write(IsMovingDown);
            msg.Write(IsMovingTop);
            msg.Write(IsMovingLeft);
            msg.Write(IsMovingRight);
            msg.Write(me.Position);

            me.SceneMgr.SendMessage(msg);
        }

        public void ReceivedMovingTypeChanged(NetIncomingMessage msg)
        {
            //TODO dodelat interpolaci na zaklade cas odeslani / cas prijmuti
            isMovingDown = msg.ReadBoolean();
            isMovingTop = msg.ReadBoolean();
            isMovingLeft = msg.ReadBoolean();
            isMovingRight = msg.ReadBoolean();

            me.Position = msg.ReadVector();
        }
    }
}
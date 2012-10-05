using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Lidgren.Network;
using Orbit.Core.Helpers;

namespace Orbit.Core.Scene.Controls.Implementations
{
    class ControlableDeviceControl : Control, IControledDevice
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
                    sendMovingTypeChanged();
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
                    sendMovingTypeChanged();
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
                    sendMovingTypeChanged();
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
                    sendMovingTypeChanged();
                }
            }
        }

        public override void InitControl(ISceneObject me)
        {
        }

        public override void UpdateControl(float tpf)
        {
            Vector botVector = new Vector(0, SharedDef.SPECTATOR_MODULE_SPEED * tpf);
            Vector rightVector = new Vector(SharedDef.SPECTATOR_MODULE_SPEED * tpf, 0);

            if (IsMovingTop && me.Position.Y > 0)
                me.Position -= botVector;
            if (IsMovingDown && me.Position.Y < SharedDef.VIEW_PORT_SIZE.Height)
                me.Position += botVector;
            if (IsMovingLeft && me.Position.X > 0)
                me.Position -= rightVector;
            if (IsMovingRight && me.Position.Y < SharedDef.VIEW_PORT_SIZE.Width)
                me.Position += rightVector;
        }

        private void sendMovingTypeChanged()
        {
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

        public void receiveMovingTypeChanged(NetIncomingMessage msg)
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
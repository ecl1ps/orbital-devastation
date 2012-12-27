using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Core.Helpers;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Orbit.Core.Scene.Entities.Implementations;
using Orbit.Core.Scene.Controls.Implementations;
using Lidgren.Network;
using Orbit.Core.Client.GameStates;

namespace Orbit.Core.Weapons
{
    class Lazor : IWeapon
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set; }
        public int Cost { get; set; }
        public string Name { get; set; }
        public DeviceType DeviceType { get; set; }
        public UpgradeLevel UpgradeLevel { get; set; }
        private float chargingTime = 0;

        private bool charging = false;
        private bool shooting = false;

        private Line rightLine;
        private Line leftLine;

        private Vector leftVector;
        private Vector rightVector;

        private Vector origin;
        private Laser laser;

        public Lazor(SceneMgr mgr, Player owner) 
        {
            SceneMgr = mgr;
            Owner = owner;
            DeviceType = DeviceType.CANNON;
            UpgradeLevel = UpgradeLevel.LEVEL3;
            Name = "Blue Laser";
            Cost = 650;
            origin = Owner.Baze.Position;
            origin.X += Owner.Baze.Size.Width / 2;
            chargingTime = Owner.Data.LaserChargingTime;
        }

        public IWeapon Next()
        {
            return null;
        }

        public void ProccessClickEvent(Point point, MouseButton button, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed && !charging && !shooting)
                startCharging();
            else if (buttonState == MouseButtonState.Released && charging && !shooting)
                stopCharging();
        }

        private void stopCharging()
        {
            charging = false;
        }

        private void startCharging()
        {
            charging = true;
            if (leftLine == null)
                prepareLines();
        }

        private void prepareLines()
        {
            SceneMgr.Invoke(new Action(() =>
            {
                leftVector = new Vector(-1000, 0);
                rightVector = new Vector(1000, 0);


                leftLine = new Line();
                leftLine.Stroke = Brushes.Blue;
                leftLine.X1 = origin.X;
                leftLine.Y1 = origin.Y;
                leftLine.X2 = origin.X + leftVector.X;
                leftLine.Y2 = origin.Y + leftVector.Y;
                leftLine.HorizontalAlignment = HorizontalAlignment.Left;
                leftLine.VerticalAlignment = VerticalAlignment.Center;
                leftLine.StrokeThickness = 1;
                leftLine.Fill = new SolidColorBrush(Colors.Blue);

                rightLine = new Line();
                rightLine.Stroke = Brushes.Blue;
                rightLine.X1 = origin.X;
                rightLine.Y1 = origin.Y;
                rightLine.X2 = origin.X + rightVector.X;
                rightLine.Y2 = origin.Y + rightVector.Y;
                rightLine.HorizontalAlignment = HorizontalAlignment.Left;
                rightLine.VerticalAlignment = VerticalAlignment.Center;
                rightLine.StrokeThickness = 1;
                rightLine.Fill = new SolidColorBrush(Colors.Blue);
            }));

            SceneMgr.AttachGraphicalObjectToScene(leftLine);
            SceneMgr.AttachGraphicalObjectToScene(rightLine);
        }

        public void Shoot(Point point)
        {
            Fire();
        }

        public bool IsReady()
        {
            return !shooting;
        }

        public void TriggerUpgrade(IWeapon old)
        {
            if (old != null)
                SceneMgr.StateMgr.RemoveGameState(old);

            SceneMgr.StateMgr.AddGameState(this);
        }

        public void Update(float tpf)
        {
            if (!shooting)
                UpdateCharging(tpf);
            else
                UpdateShooting(tpf);
        }

        private void UpdateShooting(float tpf)
        {
            if (chargingTime >= Owner.Data.LaserChargingTime)
                stopShooting();
            else
            {
                chargingTime += tpf;
                //changeShootingPosition();
            }
        }

        private void changeShootingPosition()
        {
            Vector v = StaticMouse.GetPosition().ToVector() - origin;
            v.Normalize();
            v *= 1000;
            v += origin;

            laser.End = v;

            if (SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.LASER_MOVE);
                msg.Write(laser.Id);
                msg.Write(laser.End);

                SceneMgr.SendMessage(msg);
            }
        }

        private void stopShooting()
        {
            shooting = false;
            SceneMgr.RemoveFromSceneDelayed(laser);

            if (SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                msg.Write((int)PacketType.REMOVE_OBJECT);
                msg.Write(laser.Id);
                SceneMgr.SendMessage(msg);
            }
        }

        private void UpdateCharging(float tpf)
        {
            if (charging)
            {
                if (chargingTime <= 0)
                    Fire();
                else
                {
                    chargingTime -= tpf;
                    MoveLines();
                }
            }
            else if (chargingTime != Owner.Data.LaserChargingTime)
            {
                if (chargingTime >= Owner.Data.LaserChargingTime)
                {
                    chargingTime = Owner.Data.LaserChargingTime;
                    removeLines();
                }
                else
                {
                    chargingTime += tpf;
                    MoveLines();
                }
            }
        }

        private void removeLines()
        {
            SceneMgr.RemoveGraphicalObjectFromScene(leftLine);
            SceneMgr.RemoveGraphicalObjectFromScene(rightLine);

            leftLine = null;
            rightLine = null;
        }

        private void MoveLines()
        {
            SceneMgr.Invoke(new Action(() =>
            {
                float angle = (float)FastMath.LinearInterpolate(0, Math.PI / 2, (Owner.Data.LaserChargingTime - chargingTime) / Owner.Data.LaserChargingTime);
                Vector lv = leftVector.Rotate(angle);
                Vector rv = rightVector.Rotate(-angle);

                leftLine.X2 = origin.X + lv.X;
                leftLine.Y2 = origin.Y + lv.Y;

                rightLine.X2 = origin.X + rv.X;
                rightLine.Y2 = origin.Y + rv.Y;
            }));

        }

        private void Fire()
        {
            shooting = true;
            charging = false;
            chargingTime = 0;

            Vector v = StaticMouse.GetPosition() - origin.ToPoint();
            v.Normalize();

            if (v.Y > Owner.GetBaseLocation().Y)
                v.Y = Owner.GetBaseLocation().Y;

            v *= 1000;
            v.X += origin.X;
            v.Y += origin.Y;

            laser = new Laser(Owner, IdMgr.GetNewId(Owner.GetId()), SceneMgr, origin, v, Colors.Blue, 3);

            LaserDamageControl control = new LaserDamageControl();
            laser.AddControl(control);

            SceneMgr.DelayedAttachToScene(laser);
            removeLines();

            if (SceneMgr.GameType != Gametype.SOLO_GAME)
            {
                NetOutgoingMessage msg = SceneMgr.CreateNetMessage();
                laser.WriteObject(msg);

                SceneMgr.SendMessage(msg);
            }
        }
    }
}

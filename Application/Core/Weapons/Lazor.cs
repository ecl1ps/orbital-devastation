using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Client;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Orbit.Core.Weapons
{
    class Lazor : IWeapon
    {
        public Player Owner { get; set; }
        public SceneMgr SceneMgr { get; set; }
        public float ReloadTime { get; set; }
        public int Cost { get; set; }
        public string Name { get; set; }
        public WeaponType WeaponType { get; set; }
        public WeaponLevel WeaponLevel { get; set; }
        private float chargingTime = 0;

        private bool charging = false;
        private bool shooting = false;

        private Line rightLine;
        private Line leftLine;

        private Vector leftVector;
        private Vector rightVector;

        private Vector origin;

        public Lazor(SceneMgr mgr, Player owner) 
        {
            SceneMgr = mgr;
            Owner = owner;
            WeaponType = WeaponType.CANNON;
            WeaponLevel = WeaponLevel.LEVEL3;
            Name = "Blue Laser";
            Cost = 650;
            origin = Owner.Baze.Position;
            origin.X += Owner.Baze.Size.Width / 2;
            chargingTime = SharedDef.LASER_CHARGING_TIME;
        }

        public IWeapon Next()
        {
            return null;
        }

        public void ProccessClickEvent(Point point, MouseButton button, MouseButtonState buttonState)
        {
            if(button != MouseButton.Right)
                return;

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
            if(leftLine == null)
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
            throw new NotImplementedException();
        }

        public bool IsReady()
        {
            throw new NotImplementedException();
        }

        public void TriggerUpgrade(IWeapon old)
        {
            if (old != null)
                SceneMgr.StateMgr.RemoveGameState(old);

            SceneMgr.StateMgr.AddGameState(this);
        }

        public void Update(float tpf)
        {
            if(!shooting)
                UpdateCharging(tpf);
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
            else if (chargingTime != SharedDef.LASER_CHARGING_TIME)
            {
                if (chargingTime >= SharedDef.LASER_CHARGING_TIME)
                {
                    chargingTime = SharedDef.LASER_CHARGING_TIME;
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
                float angle = (float) FastMath.LinearInterpolate(0, Math.PI / 2, (SharedDef.LASER_CHARGING_TIME - chargingTime) / SharedDef.LASER_CHARGING_TIME);
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
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Client;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;

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
        private float chargingTime = 0;

        private bool charging = false;
        private bool shooting = false;

        private Line rightLine;
        private Line leftLine;

        private Vector leftVector;
        private Vector rightVector;

        public Lazor(SceneMgr mgr, Player owner) 
        {
            SceneMgr = mgr;
            Owner = owner;
            WeaponType = WeaponType.CANOON;
            Name = "Blue Laser";
            Cost = 650;
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
            chargingTime = SharedDef.LASER_CHARGING_TIME;
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
        }
    }
}

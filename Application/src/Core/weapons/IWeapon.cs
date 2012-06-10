using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;

namespace Orbit.Core.Weapons
{
    public enum WeaponType
    {
        HOOK,
        CANOON,
        MINE
    }

    public interface IWeapon
    {
        float ReloadTime { get; set; }
        int Cost { get; set; }
        String Name { get; set; }
        WeaponType WeaponType { get; set; }

        IWeapon Next();

        void Shoot(Point point);

        Boolean IsReady();

        void UpdateTimer(float value);
    }
}

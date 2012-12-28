using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Scene.Entities;
using System.Windows;
using Orbit.Core.Scene;
using Orbit.Core.Players;
using Orbit.Core.Client;
using System.Windows.Input;

namespace Orbit.Core.Weapons
{
    public interface IActivableWeapon : IWeapon
    {
        ActivableData Data { get; set; }

        bool IsActivableReady();

        void StartActivableAction();
    }

    public class ActivableData
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Cooldown { get; set; }

        public ActivableData(string name, string icon, int cd)
        {
            Name = name;
            Icon = icon;
            Cooldown = cd;
        }
    }
}

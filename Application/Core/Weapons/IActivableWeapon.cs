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
        string ActivableName { get; set; }

        string ActivableIcon { get; set; }

        bool IsActivableReady();

        void StartActivableAction();
    }
}

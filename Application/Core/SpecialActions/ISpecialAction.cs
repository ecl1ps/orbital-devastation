﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Players;
using Orbit.Core.Client;
using System.Windows.Media;
using Orbit.Core.Scene.Entities;

namespace Orbit.Core.SpecialActions
{
    public enum SpecialActionType
    {
        BRUTAL_GRAVITY,
        ASTEROID_THROW,
        ASTEROID_DAMAGE,
        ASTEROID_GROWTH,
        ASTEROID_SLOW,
        STATIC_FIELD,
        SHIELDING
    }

    public interface ISpecialAction : IGameState, ISendable
    {
        Player Owner { get; set; }
        SceneMgr SceneMgr { get; set; }
        String Name { get; set; }
        String ImageSource { get; set; }
        SpecialActionType Type { get; set; }
        float Cost { get; set; }
        float Cooldown { get; set; }
        float RemainingCooldown { get; set; }
        Color BackgroundColor { get; set; }

        void StartAction();

        void StartCoolDown();

        Boolean IsReady();

        Boolean IsOnCooldown();

        void AddSharedAction(ISpecialAction action);

        void removeSharedAction(ISpecialAction action);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orbit.Core.Client;
using Orbit.Core.Players;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Helpers;
using Orbit.Core.Scene.Controls.Implementations;
using System.Windows;

namespace Orbit.Core.SpecialActions.Spectator
{
    class Shielding : SpecialAction
    {
        private ISceneObject shield;
        private ISceneObject toFollow;

        public Shielding(ISceneObject toFollow,SceneMgr mgr, Player plr) : 
            base(mgr, plr) {

                Name = "Static Shield";
                ImageSource = "pack://application:,,,/resources/images/icons/shield-icon.png";
                Cost = 1500;
                this.toFollow = toFollow;
        }

        public override void StartAction()
        {
            base.StartAction();

            shield = SceneObjectFactory.CreateShield(SceneMgr, Owner, toFollow);

            SceneMgr.DelayedAttachToScene(shield);
        }

        public override bool IsReady()
        {
            return Owner.Data.Gold >= Cost && (shield == null || shield.Dead);
        }
    }
}
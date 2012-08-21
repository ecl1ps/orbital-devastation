﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Orbit.Core.Client;
using Orbit.Core.SpecialActions;
using Orbit.Core.Scene.Entities;
using Orbit.Core.Scene.Controls.Implementations;
using Orbit.Core.SpecialActions.Spectator;

namespace Orbit.Core.Players.Input
{
    public class SpectatorInputMgr : IInputMgr
    {
        private Player plr;
        private SceneMgr mgr;
        private IControledDevice device;
        private MiningModuleControl miningControl;

        public SpectatorInputMgr(Player p, SceneMgr sceneMgr, ISceneObject obj)
        {
            IControledDevice d = obj.GetControlOfType(typeof(IControledDevice)) as IControledDevice;
            MiningModuleControl mc = obj.GetControlOfType(typeof(MiningModuleControl)) as MiningModuleControl;

            if (mc == null)
                throw new Exception("U must inicialize SpectatorInputManager with object containg MiningModuleControl");
            if (d == null)
                throw new Exception("U must inicialize SpectatorInputManager with object containg IControledDevice control");
            
            plr = p;
            mgr = sceneMgr;
            device = d;
            miningControl = mc;
        }

        public void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {
            // TODO: akce budou vetsinou spousteny z action baru
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
                new BrutalGravity(mgr, plr).StartAction();
        }

        public void OnActionBarClick(Point point, MouseButtonEventArgs e)
        {

        }

        public void OnKeyEvent(KeyEventArgs e)
        {
            bool down = e.IsDown;
            if (e.Key == Key.W)
                device.IsMovingTop = down;
            else if (e.Key == Key.S)
                device.IsMovingDown = down;
            else if (e.Key == Key.A)
                device.IsMovingLeft = down;
            else if (e.Key == Key.D)
                device.IsMovingRight = down;
        }
    }
}

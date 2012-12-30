using System;
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
using Orbit.Gui.InteractivePanel;
using Orbit.Gui;

namespace Orbit.Core.Players.Input
{
    public class SpectatorInputMgr : AbstractInputMgr
    {
        private Player plr;
        private SceneMgr mgr;
        private IControledDevice device;
        private MiningModuleControl miningControl;

        public SpectatorInputMgr(Player p, SceneMgr sceneMgr, ISceneObject obj, ActionBarMgr actionMgr) : base(actionMgr)
        {
            IControledDevice d = obj.GetControlOfType<IControledDevice>();
            MiningModuleControl mc = obj.GetControlOfType<MiningModuleControl>();

            if (mc == null)
                throw new Exception("U must inicialize SpectatorInputManager with object containg MiningModuleControl");
            if (d == null)
                throw new Exception("U must inicialize SpectatorInputManager with object containg IControledDevice control");
            
            plr = p;
            mgr = sceneMgr;
            device = d;
            miningControl = mc;
        }

        public override void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {
            base.OnCanvasClick(point, e);
        }


        public override void OnKeyEvent(KeyEventArgs e)
        {
            base.OnKeyEvent(e);
            bool down = e.IsDown;
            if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_TOP)))
                device.IsMovingTop = down;
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_BOT)))
                device.IsMovingDown = down;
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_LEFT)))
                device.IsMovingLeft = down;
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_RIGHT)))
                device.IsMovingRight = down;
        }
    }
}

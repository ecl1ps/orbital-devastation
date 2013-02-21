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
using System.Globalization;
using Orbit.Core.Scene.Particles.Implementations;
using System.Windows.Media;
using Orbit.Core.Helpers;

namespace Orbit.Core.Players.Input
{
    public class SpectatorInputMgr : AbstractInputMgr
    {
        private Player plr;
        private IControledDevice device;
        private MiningModuleControl miningControl;

        public SpectatorInputMgr(Player p, SceneMgr sceneMgr, ISceneObject obj, ActionBarMgr actionMgr) : base(actionMgr, sceneMgr)
        {
            IControledDevice d = obj.GetControlOfType<IControledDevice>();
            MiningModuleControl mc = obj.GetControlOfType<MiningModuleControl>();

            if (mc == null)
                throw new Exception("You must initialize SpectatorInputManager with object containig MiningModuleControl");
            if (d == null)
                throw new Exception("You must initialize SpectatorInputManager with object containig IControledDevice control");
            
            plr = p;
            device = d;
            miningControl = mc;
        }

        public override void OnKeyEvent(KeyEventArgs e)
        {
            if (sceneMgr.UserActionsDisabled)
                return;

            base.OnKeyEvent(e);
            bool down = e.IsDown;
            if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_TOP), CultureInfo.InvariantCulture))
                device.IsMovingTop = down;
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_BOT), CultureInfo.InvariantCulture))
                device.IsMovingDown = down;
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_LEFT), CultureInfo.InvariantCulture))
                device.IsMovingLeft = down;
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_ACTION_MOVE_RIGHT), CultureInfo.InvariantCulture))
                device.IsMovingRight = down;
            else if (e.Key == (Key)int.Parse(GameProperties.Props.Get(PropertyKey.PLAYER_SHOW_PROTECTING), CultureInfo.InvariantCulture) && down)
                plr.ShowProtecting();
        }

        public override void OnCanvasClick(Point point, MouseButtonEventArgs e)
        {
            base.OnCanvasClick(point, e);

            if(e.ButtonState == MouseButtonState.Pressed)
                ParticleEmmitorFactory.CreateExplosionEmmitors(sceneMgr, point.ToVector()).Attach(sceneMgr);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Orbit.Gui;
using Orbit.Core.Utils;
using Orbit.Core.Scene;
using System.Windows.Threading;
using Orbit.Core.Weapons;
using Orbit.Core.Players;
using Orbit.Core.Client;
using Orbit.Gui.ActionControllers;
using Orbit.Core.SpecialActions.Spectator;
using Orbit.Gui.InteractivePanel;
using System.Windows.Controls;

namespace Orbit.Core.Helpers
{
    class GuiObjectFactory
    {
        public static ActionUC CreateAndAddActionUC(SceneMgr mgr, ActionBar actionbar, ActionController<ActionUC> controller)
        {
            ActionUC wnd = null;

            mgr.Invoke(new Action(() =>
            {
                wnd = ActionUC.CreateWindow(controller);
                actionbar.AddItem(wnd);
            }));

            return wnd;
        }

        public static AlertBox CreateAndAddAlertBox(SceneMgr mgr, Vector position)
        {
            AlertBox box = new AlertBox();
                
            mgr.GetCanvas().Children.Add(box);
            Canvas.SetLeft(box, position.X);
            Canvas.SetTop(box, position.Y);
            Canvas.SetZIndex(box, 100);

            return box;
        }

        public static EndGameStats CreateAndAddPlayerStatsUc(SceneMgr mgr, bool isPlayer, Vector position)
        {
            EndGameStats stats = CreatePlayerStatsUC(mgr, isPlayer, true);

            mgr.GetCanvas().Children.Add(stats);
            Canvas.SetLeft(stats, position.X);
            Canvas.SetTop(stats, position.Y);
            Canvas.SetZIndex(stats, 200);

            return stats;
        }

        public static EndGameStats CreatePlayerStatsUC(SceneMgr mgr, bool isPlayer, bool limited)
        {
            EndGameStats statsWindow;
            if (limited)
                statsWindow = new EndGameStats(mgr);
            else
                statsWindow = new EndGameStats(null);

            if (isPlayer)
            {
                PlayerStatsUC playerStats = new PlayerStatsUC();
                statsWindow.setStats(playerStats);


                PlayerStatisticsController controller;
                if (limited)
                    controller = new PlayerStatisticsController(mgr, statsWindow, limited, playerStats);
                else
                    controller = new InstaPlayerStatisticsController(mgr, statsWindow, limited, playerStats);
                mgr.StateMgr.AddGameState(controller);
            }
            else
            {
                SpectatorStatsUC playerStats = new SpectatorStatsUC();
                statsWindow.setStats(playerStats);

                SpectatorStatisticController controller;
                if (limited)
                    controller = new SpectatorStatisticController(mgr, statsWindow, limited, playerStats);
                else
                    controller = new InstaSpectatorStatisticsController(mgr, statsWindow, limited, playerStats);

                mgr.StateMgr.AddGameState(controller);
            }

            return statsWindow;
        }
    }
}

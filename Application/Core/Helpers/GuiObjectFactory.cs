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

        public static StatisticsTabbedPanel CreateStatisticsTabbedPanel(SceneMgr mgr)
        {
            StatisticsTabbedPanel panel = new StatisticsTabbedPanel();

            List<Player> players = mgr.GetPlayers();

            players.ForEach(player => panel.AddItem(CreatePlayerStatsUC(mgr, player, player.IsActivePlayer(), false), player.Data.Name));

            return panel;
        }

        public static EndGameStats CreateAndAddPlayerStatsUc(SceneMgr mgr, Player owner, bool isPlayer, Vector position)
        {
            EndGameStats stats = CreatePlayerStatsUC(mgr, owner, isPlayer, true);

            mgr.GetCanvas().Children.Add(stats);
            Canvas.SetLeft(stats, position.X);
            Canvas.SetTop(stats, position.Y);
            Canvas.SetZIndex(stats, 200);

            return stats;
        }

        public static EndGameStats CreatePlayerStatsUC(SceneMgr mgr, Player owner, bool isPlayer, bool limited)
        {
            EndGameStats statsWindow = new EndGameStats();

            if (isPlayer)
            {
                PlayerStatsUC playerStats = new PlayerStatsUC();
                statsWindow.SetStats(playerStats);

                PlayerStatisticsController controller;
                if (limited)
                    controller = new PlayerStatisticsController(mgr, owner, statsWindow, limited, playerStats);
                else
                    controller = new InstaPlayerStatisticsController(mgr, owner, statsWindow, limited, playerStats);

                mgr.StateMgr.AddGameState(controller);
            }
            else
            {
                SpectatorStatsUC playerStats = new SpectatorStatsUC();
                statsWindow.SetStats(playerStats);

                SpectatorStatisticController controller;
                if (limited)
                    controller = new SpectatorStatisticController(mgr, owner, statsWindow, limited, playerStats);
                else
                    controller = new InstaSpectatorStatisticsController(mgr, owner, statsWindow, limited, playerStats);

                mgr.StateMgr.AddGameState(controller);
            }

            return statsWindow;
        }
    }
}

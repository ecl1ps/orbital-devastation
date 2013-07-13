using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Orbit.Core.Scene;
using Orbit.Core.Client.GameStates;
using Orbit.Core;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for GameUC.xaml
    /// </summary>
    public partial class GameUC : UserControl
    {
        public GameUC(UserControl gameControl)
        {
            InitializeComponent();
            //Dispatcher.Invoke(new Action(() => mainCanvas.Children.Add(gameControl)));
            mainCanvas.Children.Add(gameControl);
        }

        private void settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement esc = LogicalTreeHelper.FindLogicalNode(App.WindowInstance.mainGrid, "escMenu") as UIElement;
            if (esc != null)
            {
                App.WindowInstance.ClearMenus();
                if (App.WindowInstance.GameRunning)
                    StaticMouse.Enable(true);
            }
            else
            {
                App.WindowInstance.AddMenu(new EscMenu());
                if (App.WindowInstance.GameRunning)
                    StaticMouse.Enable(false);
            }
        }

        private void overview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GameOverviewUC go = LogicalTreeHelper.FindLogicalNode(App.WindowInstance.mainGrid, "gameOverview") as GameOverviewUC;
            if (go != null)
                App.Instance.ClearMenus();
            else
            {
                App.Instance.GetSceneMgr().Enqueue(new Action(() =>
                {
                    App.Instance.GetSceneMgr().ShowPlayerOverview();
                }));
            }
        }
    }
}

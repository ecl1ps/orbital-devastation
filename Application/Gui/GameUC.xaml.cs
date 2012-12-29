using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Orbit.Core.Scene;
using Orbit.Core.Client.GameStates;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for GameUC.xaml
    /// </summary>
    public partial class GameUC : UserControl
    {
        public GameUC()
        {
            InitializeComponent();
        }

        private void OnCanvasMouseClick(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(mainCanvas);

            (Application.Current as App).GetSceneMgr().Enqueue(new Action(() =>
            {
                (Application.Current as App).GetSceneMgr().OnCanvasClick(p, e);
            }));
        }

        private void OnActionBarClick(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(ActionBarUC);

            (ActionBarUC as ActionBar).OnClick(ActionBarUC.PointToScreen(p));
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (Application.Current == null)
                return;

            Point p = e.GetPosition(mainCanvas);
            (Application.Current as App).GetSceneMgr().Enqueue(new Action(() =>
            {
                (Application.Current as App).GetSceneMgr().OnCanvasMouseMove(p);
            }));
        }

        private void settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GameWindow wnd = Application.Current.MainWindow as GameWindow;
            UIElement esc = LogicalTreeHelper.FindLogicalNode(wnd.mainGrid, "escMenu") as UIElement;
            if (esc != null)
            {
                wnd.ClearMenus();
                if (wnd.GameRunning)
                    StaticMouse.Enable(true);
            }
            else
            {
                wnd.AddMenu(new EscMenu());
                if (wnd.GameRunning)
                    StaticMouse.Enable(false);
            }
        }

        private void overview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GameOverviewUC go = LogicalTreeHelper.FindLogicalNode((Application.Current.MainWindow as GameWindow).mainGrid, "gameOverview") as GameOverviewUC;
            if (go != null)
                (Application.Current as App).ClearMenus();
            else
            {
                (Application.Current as App).GetSceneMgr().Enqueue(new Action(() =>
                {
                    (Application.Current as App).GetSceneMgr().ShowPlayerOverview();
                }));
            }
        }
    }
}

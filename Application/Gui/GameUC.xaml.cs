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
    }
}

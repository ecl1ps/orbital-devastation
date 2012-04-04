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
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Application.Current.Shutdown();
                    break;
            }
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SceneMgr.GetInstance().Enqueue(new Action(() =>
            {
                SceneMgr.GetInstance().OnViewPortChange(e.NewSize);
            }));
        }

        private void OnCanvasMouseClick(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(mainCanvas);
            if (p.Y <= actionArea.Height)
                SceneMgr.GetInstance().Enqueue(new Action(() =>
                {
                    SceneMgr.GetInstance().OnActionAreaClick(p);
                }));
        }

    }
}

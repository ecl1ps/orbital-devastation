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
        private Size SizeOriginal { get; set; }

        public GameUC()
        {
            InitializeComponent();
            SizeOriginal = new Size(mainCanvas.Width, mainCanvas.Height);
        }

        private void OnCanvasMouseClick(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(mainCanvas);

            SceneMgr.GetInstance().Enqueue(new Action(() =>
            {
                SceneMgr.GetInstance().OnCanvasClick(p);
            }));
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainCanvas.RenderTransform = new ScaleTransform(e.NewSize.Width / SizeOriginal.Width, e.NewSize.Height / SizeOriginal.Height);
            SceneMgr.GetInstance().Enqueue(new Action(() =>
            {
                SceneMgr.GetInstance().OnViewPortChange(e.NewSize);
            }));
        }
    }
}

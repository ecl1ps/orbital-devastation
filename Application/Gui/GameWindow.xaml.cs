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
using Orbit.Core.Client;
using Orbit.Core.Client.GameStates;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public bool GameRunning { get; set; }
        public bool tabDown = false;

        public GameWindow()
        {
            InitializeComponent();
        }

        #region ScaleValue Dependency Property
        public static readonly DependencyProperty ScaleValuePropertyX = DependencyProperty.Register("ScaleValueX", typeof(double), typeof(GameWindow), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleValueChanged), new CoerceValueCallback(OnCoerceScaleValue)));
        public static readonly DependencyProperty ScaleValuePropertyY = DependencyProperty.Register("ScaleValueY", typeof(double), typeof(GameWindow), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleValueChanged), new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            GameWindow mainWindow = o as GameWindow;
            if (mainWindow != null)
                return mainWindow.OnCoerceScaleValue((double)value);
            else
                return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GameWindow mainWindow = o as GameWindow;
            if (mainWindow != null)
                mainWindow.OnScaleValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0f;

            value = Math.Max(0.3, value);
            return value;
        }

        protected virtual void OnScaleValueChanged(double oldValue, double newValue)
        {

        }

        public double ScaleValueX
        {
            get
            {
                return (double)GetValue(ScaleValuePropertyX);
            }
            set
            {
                SetValue(ScaleValuePropertyX, value);
            }
        }

        public double ScaleValueY
        {
            get
            {
                return (double)GetValue(ScaleValuePropertyY);
            }
            set
            {
                SetValue(ScaleValuePropertyY, value);
            }
        }
        #endregion

        private void OnSizeChanged(object sender, EventArgs e)
        {
            CalculateScale();
        }

        private void CalculateScale()
        {
            // event musi byt navazany na hodnoty velikosti okna (jinak scale stale preskakuje)
            // ale hodnoty, se kterymi se pocita, musi byt zmensene na velikost osahu okna (okno: 1020*740, obsah okna: 1000*700)
            double xScale = (ActualWidth - 20) / 1000f;
            double yScale = (ActualHeight - 40) / 700f;
            ScaleValueX = (double)OnCoerceScaleValue(mainContainerGrid, xScale);
            ScaleValueY = (double)OnCoerceScaleValue(mainContainerGrid, yScale);
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (Application.Current as App).ShutdownSceneMgr();
            Application.Current.Shutdown();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    UIElement uc;
                    if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "botSelection") as UIElement) != null)
                    {
                        mainGrid.Children.Remove(uc);
                        if (GameRunning)
                            StaticMouse.Enable(true);
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "mouseMenu") as UIElement) != null)
                    {
                        showOptions(uc);
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "soundMenu") as UIElement) != null)
                    {
                        showOptions(uc);
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "playerSettings") as UIElement) != null)
                    {
                        showOptions(uc);
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "keyBindingsMenu") as UIElement) != null)
                    {
                        showOptions(uc);
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "optionsMenu") as UIElement) != null)
                    {
                        mainGrid.Children.Remove(uc);
                        mainGrid.Children.Add(new EscMenu());
                        if (GameRunning)
                            StaticMouse.Enable(false);
                    } 
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(mainGrid, "escMenu") as UIElement) != null)
                    {
                        mainGrid.Children.Remove(uc);
                        if (GameRunning)
                            StaticMouse.Enable(true);
                    } else {
                        mainGrid.Children.Add(new EscMenu());
                        if (GameRunning)
                            StaticMouse.Enable(false);
                    }
                    break;
                case Key.Tab:
                    if (tabDown)
                        return;
                    tabDown = true;
                    GameOverviewUC go = LogicalTreeHelper.FindLogicalNode(mainGrid, "gameOverview") as GameOverviewUC;
                    if (go != null)
                        mainGrid.Children.Remove(go);
                    break;
            }

            (Application.Current as App).OnKeyEvent(e);
        }

        private void showOptions(UIElement elem)
        {
            mainGrid.Children.Remove(elem);
            mainGrid.Children.Add(new OptionsMenu());
            if (GameRunning)
                StaticMouse.Enable(false);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    if (!tabDown)
                        return;

                    tabDown = false;
                    GameOverviewUC go = LogicalTreeHelper.FindLogicalNode(mainGrid, "gameOverview") as GameOverviewUC;
                    if (go != null)
                        mainGrid.Children.Remove(go);
                    break;
            }

            (Application.Current as App).OnKeyEvent(e);
        }
    }
}

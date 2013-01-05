﻿using System;
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
using System.Windows.Interop;
using System.Runtime.InteropServices;

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

            if (WindowStyle == WindowStyle.None)
                Height = 714;
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
            // pokud nema okno listu s krizkem, tak je potreba okno nastavit mensim a misto 40 bodu listy pouzit 14
            double xScale = (ActualWidth - 14) / 1000f;
            double yScale;
            if (WindowStyle != WindowStyle.None)
                yScale = (ActualHeight - 40) / 700f;
            else
                yScale = (ActualHeight - 14) / 700f;

            ScaleValueX = (double)OnCoerceScaleValue(mainContainerGrid, xScale);
            ScaleValueY = (double)OnCoerceScaleValue(mainContainerGrid, yScale);
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Instance.ShutdownSceneMgr();
            Application.Current.Shutdown();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    UIElement uc;
                    if ((uc = LogicalTreeHelper.FindLogicalNode(menuGrid, "botSelection") as UIElement) != null)
                    {
                        ClearMenus();
                        if (GameRunning)
                            StaticMouse.Enable(true);
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(menuGrid, "colorPicker") as UIElement) != null)
                    {
                        AddMenu(new PlayerSettings());
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(menuGrid, "mouseMenu") as UIElement) != null)
                    {
                        ShowOptionsMenu();
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(menuGrid, "soundMenu") as UIElement) != null)
                    {
                        ShowOptionsMenu();
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(menuGrid, "playerSettings") as UIElement) != null)
                    {
                        ShowOptionsMenu();
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(menuGrid, "keyBindingsMenu") as UIElement) != null)
                    {
                        ShowOptionsMenu();
                    }
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(menuGrid, "optionsMenu") as UIElement) != null)
                    {
                        AddMenu(new EscMenu());
                        if (GameRunning)
                            StaticMouse.Enable(false);
                    } 
                    else if ((uc = LogicalTreeHelper.FindLogicalNode(menuGrid, "escMenu") as UIElement) != null)
                    {
                        ClearMenus();
                        if (GameRunning)
                            StaticMouse.Enable(true);
                    } 
                    else if (menuGrid.Children.Count == 0) {
                        AddMenu(new EscMenu());
                        if (GameRunning)
                            StaticMouse.Enable(false);
                    }
                    else
                    {
                        ClearMenus();
                    }
                    break;
                case Key.Tab:
                    if (tabDown)
                        return;

                    tabDown = true;
                    GameOverviewUC go = LogicalTreeHelper.FindLogicalNode(menuGrid, "gameOverview") as GameOverviewUC;
                    if (go != null)
                        menuGrid.Children.Remove(go);
                    break;
            }

            App.Instance.OnKeyEvent(e);
        }

        public void ShowOptionsMenu()
        {
            AddMenu(new OptionsMenu());
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
                    GameOverviewUC go = LogicalTreeHelper.FindLogicalNode(menuGrid, "gameOverview") as GameOverviewUC;
                    if (go != null)
                        menuGrid.Children.Remove(go);
                    break;
            }

            App.Instance.OnKeyEvent(e);
        }

        public void ClearMenus()
        {
            menuGrid.Children.Clear();
            mainGrid.IsHitTestVisible = true;
        }

        public void AddMenu(UserControl menu, bool removeOldMenus = true)
        {
            if (removeOldMenus)
                ClearMenus();

            // ve hre nevypinat testovani hitu, jinak prestanou fungovat menu bary
            if (!(App.Instance.IsGameStarted()))
                mainGrid.IsHitTestVisible = false;

            menuGrid.Children.Add(menu);
        }

        private void mainWindow_Deactivated(object sender, EventArgs e)
        {
            if (GameRunning)
                StaticMouse.Enable(false);
        }

        private void mainWindow_Activated(object sender, EventArgs e)
        {
            if (GameRunning)
                StaticMouse.Enable(true);
        }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}

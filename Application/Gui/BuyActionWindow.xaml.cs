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
using Orbit.Core.Utils;
using Orbit.Core.Scene;
using Orbit.Gui.ActionControllers;
using Orbit.Core.Client;

namespace Orbit.Gui
{
    public partial class BuyActionWindow : UserControl
    {
        private ActionController controller;

        private BuyActionWindow(ActionController c)
        {
            controller = c;
            InitializeComponent();
        }

        /// <summary>
        /// okno musi byt vytvareno statickou tovarni metodou, protoze jinak by slo o nebezpecne publikovani objektu
        /// </summary>
        public static BuyActionWindow CreateWindow(ActionController c)
        {
            BuyActionWindow w = new BuyActionWindow(c);

            c.Enqueue(new Action(() =>
            {
                c.CreateHeaderText(w);
                c.CreatePriceText(w);
                c.CreateImageUriString(w);
            }));

            return w;
        }

        /// <summary>
        /// thread safe
        /// </summary>
        public void SetImageUri(string uri)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(uri);
                image.EndInit();
                ButtonImage.Source = image;
            }));
        }

        /// <summary>
        /// thread safe
        /// </summary>
        public void SetHeaderText(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Header.Text = text;
            }));
        }

        /// <summary>
        /// thread safe
        /// </summary>
        public void SetPriceText(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Price.Text = text;
            }));
        }

        /// <summary>
        /// thread safe
        /// </summary>
        public void Remove()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                (Parent as Panel).Children.Remove(this);
            }));
        }

        public void OnClick(Point point)
        {
            Point p = PointFromScreen(point);
            double maxX = this.ActualWidth;
            double maxY = this.ActualHeight;

            if (p.X < this.ActualWidth && p.X > 0)
                if (p.Y < this.ActualHeight && p.Y > 0)
                {
                    controller.Enqueue(new Action(() =>
                    {
                        controller.ActionClicked(this);
                    }));
                }
        }
    }
}

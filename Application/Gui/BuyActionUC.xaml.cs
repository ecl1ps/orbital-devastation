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
using ShaderEffectLibrary;

namespace Orbit.Gui
{
    public partial class BuyActionUC : UserControl
    {
        private ActionController controller;
        private bool active = true;

        private BuyActionUC(ActionController c)
        {
            controller = c;
            InitializeComponent();
        }

        /// <summary>
        /// okno musi byt vytvareno statickou tovarni metodou, protoze jinak by slo o nebezpecne publikovani objektu
        /// </summary>
        public static BuyActionUC CreateWindow(ActionController c)
        {
            BuyActionUC w = new BuyActionUC(c);

            w.AttachNewController(c);

            return w;
        }

        public void AttachNewController(ActionController c)
        {
            c.Enqueue(new Action(() =>
            {
                c.CreateHeaderText(this);
                c.CreatePriceText(this);
                c.CreateImageUriString(this);
            }));
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
            if (!active)
                return;

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

        /// <summary>
        /// thread safe
        /// </summary>
        public void Activate()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!active)
                {
                    active = true;
                    ButtonImage.Effect = null;
                }
            }));
        }

        /// <summary>
        /// thread safe
        /// </summary>
        public void Deactivate()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (active)
                {
                    active = false;
                    MonochromeEffect eff = new MonochromeEffect();
                    eff.FilterColor = Color.FromArgb(0x00, 0x40, 0x40, 0x40);
                    /*EmbossedEffect eff = new EmbossedEffect();
                    eff.Amount = 10;
                    eff.Width = 0.001;*/
                    ButtonImage.Effect = eff;
                }
            }));
        }
    }
}

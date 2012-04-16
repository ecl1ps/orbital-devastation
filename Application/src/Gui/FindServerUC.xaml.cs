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
using System.Threading;
using Orbit.Core;
using System.Text.RegularExpressions;
using Lidgren.Network;
using System.Net;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for FindServerUC.xaml
    /// </summary>
    public partial class FindServerUC : UserControl
    {
        private Thread searchingThread;
        private NetSearcher searcher;
        public string LastAddress
        {
            set
            {
                tbServerAddress.Text = value;
            }
        }

        public FindServerUC()
        {
            InitializeComponent();
        }

        private void btnFindServers_Click(object sender, RoutedEventArgs e)
        {
            if (searchingThread != null && searchingThread.IsAlive)
            {
                searcher.StartNewSearch();
                return;
            }

            searcher = new NetSearcher();
            searcher.ServerListBox = lbxFoundServers;

            searchingThread = new Thread(new ThreadStart(searcher.Run));
            searchingThread.IsBackground = true;
            searchingThread.Start();
            searcher.StartNewSearch();
        }

        private void lbxFoundServers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxFoundServers.SelectedIndex > -1)
            {
                string ip = lbxFoundServers.Items[lbxFoundServers.SelectedIndex].ToString();
                System.Console.WriteLine(ip);

                searcher.Shutdown();
                Thread.Sleep(10); // vypnuti muze chvili trvat
                (Application.Current as App).ConnectToGame(ip);
            }
        }

        private void btnDirectConnect_Click(object sender, RoutedEventArgs e)
        {
            if (tbServerAddress.Text.Equals(""))
            {
                lblAddressStatus.Content = "Specify the address";
                lblAddressStatus.Foreground = Brushes.Red;
                return;
            }

            Regex ipAddressRegexp = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            //Regex hostNameRegexp = new Regex(@"^(([a-zA-Z]|[a-zA-Z][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$");
            Regex hostNameRegexp = new Regex(@"^([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])(\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9]))*$");
            if (!ipAddressRegexp.IsMatch(tbServerAddress.Text) && !hostNameRegexp.IsMatch(tbServerAddress.Text))
            {
                lblAddressStatus.Content = "Address is not valid";
                lblAddressStatus.Foreground = Brushes.Red;
                return;
            }

            lblAddressStatus.Content = "Checking";
            lblAddressStatus.Foreground = Brushes.Yellow;

            try
            {
                NetUtility.ResolveAsync(tbServerAddress.Text, delegate(IPAddress adr)
                {
                    if (adr == null)
                    {
                        lblAddressStatus.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblAddressStatus.Content = "Address not found";
                            lblAddressStatus.Foreground = Brushes.Red;
                        }));
                        return;
                    }

                    if (searcher != null)
                    {
                        searcher.Shutdown();
                        Thread.Sleep(10); // vypnuti muze chvili trvat
                    }

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        (Application.Current as App).ConnectToGame(tbServerAddress.Text);
                    }));
                    
                });
            }
            catch (System.Exception ex)
            {
                lblAddressStatus.Content = "Address not found";
                lblAddressStatus.Foreground = Brushes.Red;
            }
        }
    }
}

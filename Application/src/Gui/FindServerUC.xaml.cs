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
            if (tbServerAddress.Text.Equals("")) //TODO check adresy
                return;

            if (searcher != null)
            {
                searcher.Shutdown();
                Thread.Sleep(10); // vypnuti muze chvili trvat
            }

            (Application.Current as App).ConnectToGame(tbServerAddress.Text);
        }
    }
}

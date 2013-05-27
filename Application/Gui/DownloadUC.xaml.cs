using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Orbit.Core;

namespace Orbit.Gui
{
    /// <summary>
    /// Interaction logic for DownloadUC.xaml
    /// </summary>
    public partial class DownloadUC : UserControl
    {
        public DownloadUC(string serverVersion)
        {
            InitializeComponent();
            this.linkDownload.NavigateUri = new System.Uri(SharedDef.DOWNLOAD_LINK);
            this.linkDownload.Inlines.Clear();
            this.linkDownload.Inlines.Add(String.Format(Strings.ui_client_version, serverVersion));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.ClearMenus();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
            App.Instance.ClearMenus();
        }
    }
}

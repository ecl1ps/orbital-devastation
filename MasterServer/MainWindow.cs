using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Orbit.Core.Server;
using log4net.Appender;

namespace MasterServer
{
    public partial class MainWindow : Form, IAppender
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MasterServerMgr server;

        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure(Assembly.GetExecutingAssembly().GetManifestResourceStream("MasterServer.logger.config"));
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            server.Shutdown();
            Application.Exit();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Shutdown();
        }

        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            BeginInvoke(new Action(() => 
            {
                lbOut.Items.Add(String.Format("{0}: {1}", loggingEvent.Level.Name, loggingEvent.MessageObject.ToString()));
                lbOut.SelectedIndex = lbOut.Items.Count - 1;
            }));
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.AddAppender(this);
            server = new MasterServerMgr();
        }
    }
}

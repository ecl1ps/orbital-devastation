using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterServer
{
    static class Program
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure(Assembly.GetExecutingAssembly().GetManifestResourceStream("MasterServer.logger.config"));

            Logger.Info("Server started");
            MainWindow wind = null;
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                wind = new MainWindow();
                Application.Run(wind);
            }
            catch (Exception e)
            {
                if (wind != null && ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.Repository.GetAppenders().Contains(wind))
                    ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository()).Root.RemoveAppender(wind);

                Logger.Info("Server crashed");
                Logger.Fatal(e);
                throw;
            }
            Logger.Info("Server shutdown");
        }
    }
}

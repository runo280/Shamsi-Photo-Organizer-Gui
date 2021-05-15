using System.Windows;
using NLog;

namespace Shamsi_Photo_Organizer
{
    public partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Logger.Debug("App started");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Logger.Debug("App exit");
            
        }
    }
}
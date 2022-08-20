using System.Windows;
using System.Windows.Threading;
using NLog;

namespace Presentation.WpfApp
{
    public partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error(e.Exception);
            MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

using System;
using System.ServiceModel;
using System.Windows;

namespace Gldd.AmazonWaterLog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);
            var binding = new NetNamedPipeBinding();
            var bubblerService = new BubblerService();
            var bubblerServiceHost = new ServiceHost(bubblerService, new Uri("net.pipe://localhost/BubblerService/"));
            bubblerServiceHost.AddServiceEndpoint(typeof(IBubblerService), binding, "BubblerService");
            bubblerServiceHost.Open();

            var mainWindowViewModel = new MainWindowViewModel(bubblerService);

            var mainWindow = new MainWindow();
            mainWindow.DataContext = mainWindowViewModel;
            mainWindow.Show();
        }
    }
}

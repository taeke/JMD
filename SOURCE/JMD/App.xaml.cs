namespace JMD
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using DrawMap;
    using DrawMap.Interface;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The startup for the application.
        /// </summary>
        /// <param name="sender"> The sender</param>
        /// <param name="e"> The <see cref="StatupEventArgs"/> instance. </param>
        private void App_Startup(object sender, StartupEventArgs e)
        {
            UnityContainer unityContainer = new UnityContainer();
            unityContainer.RegisterType<IMapFiles, MapFiles>();

            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow(unityContainer.Resolve<IMapFiles>());
            mainWindow.Show();
        }
    }
}

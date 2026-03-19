using SecureBunkerCore;
using System.Windows;

namespace SecureBunker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Prevent automatic shutdown when the login dialog (the only window) closes.
            var previousShutdownMode = this.ShutdownMode;
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Manager.SetSource();
            
            bool resultLogin = Manager.TryLogin();
            if (resultLogin == true)
            {                                
                // Restore a sensible shutdown mode before creating the main window
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;

                //load the source data and show the main window
                Manager.GetSourceData();                

                var mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                mainWindow.ShowDialog();
            }
            
            // shut down the application
            this.Shutdown();
        }
    }
}

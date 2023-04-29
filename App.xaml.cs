using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Menager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);


        }
        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);
            register();
        }
        private void register()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
                HandlerException(e.ExceptionObject as Exception);
            };
            DispatcherUnhandledException += (sender, e) => {
               // e.Handled = true;
                HandlerException(e.Exception);

            };

            TaskScheduler.UnobservedTaskException += (sender, e) => {
                HandlerException(e.Exception);
            };
        }
       

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

       

     private void  HandlerException(Exception e)
        {


            MessageBox.Show(e.Message); 
        }
    }
}

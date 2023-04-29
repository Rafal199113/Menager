using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Threading;

namespace Menager.others
{
   public  class Notifications
    {
        private string message;

        public Grid grid { get;  set; }
        public MainWindow window { get; private set; }
        public CancellationTokenSource? ts { get; private set; }
        public TextBox lab { get; private set; }
        Task task;
        public List<Task> tasks { get; private set; }
        public object can { get; private set; }

        MediaPlayer mediaPlayer = new MediaPlayer();
        public  Notifications(Grid grid, MainWindow mainWindow, CancellationTokenSource ts)
        {
            this.ts = ts;
            
            this.grid = grid;
            this.window = mainWindow;
            this.ts = ts;

            tasks=new List<Task>();
           
        }

       

        public async void show(string message, CancellationTokenSource ts)
        {
            lab = new TextBox();
            lab.Template = window.FindResource("message") as ControlTemplate;
            lab.Text = message;
          


            tasks.Add(new Task(() =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                {
                   
                    mediaPlayer.Open(new Uri("assets\\Notification.mp3", UriKind.Relative));
                    mediaPlayer.Play();

                    Grid.SetColumn(lab, 1);
                    Grid.SetRow(lab, 5);




                    grid.Children.Add(lab);



                });

                Thread.Sleep(2000);
                System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                {

                    grid.Children.Remove(lab);

                });

            }));
     
           
          
            for(int i=0;i<tasks.Count; i++)
            {
                
                tasks[i].Start();
                
                tasks.Remove(tasks[i]);

            }

        }
    }
}

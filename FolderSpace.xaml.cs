using Notatki;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Menager
{
    /// <summary>
    /// Interaction logic for FolderSpace.xaml
    /// </summary>
    public partial class FolderSpace : Window
    {
        List<FileInfo> files;
  
        List<foldery> directories;
        List<DirectoryInfo> dirs;
        double folderS;
        int size;
        double procet;
        Folders openFolder;
        int filesCoutnt = 0;
        public Icon set { get; private set; }
        public CancellationTokenSource ts { get; private set; }

        public ObservableCollection<string> filesList { get; set; }

        public FolderSpace(string v, Notatki.Icon set, Folders folder)

        {
            filesList = new ObservableCollection<string>() { "cipeczka","cycuszki"};
            filesList.Add("cipeczka");
            filesList.Add("cipeczka");
            filesList.Add("cipeczka");
            filesList.Add("cipeczka");
            filesList.Add("cipeczka");
            filesList.Add("cipeczka");
            filesList.Add("cipeczka");
            InitializeComponent();
            DataContext = this;
            this.openFolder = folder;
            this.set = set;
          
            DriveInfo[] drives = DriveInfo.GetDrives();
            string DiskName = v.Split("\\")[0] + "\\";
            long freeSpace = 0;
            foreach (DriveInfo drive in drives)
            {

                if (drive.Name == DiskName)
                {

                    freeSpace = drive.TotalSize;

                }

                if (drive.IsReady) Console.WriteLine(drive.TotalSize);
            }

            files = new List<FileInfo>();
            dirs = new List<DirectoryInfo>();
            directories = new List<foldery>();
        



            size = (int)(((freeSpace / 1024)) / 1024);
            DiscSize.Content = size / 1024 + "GB";

            int x = 0;
            folderS = 0;
            getFolderSize(v);

            Task.Run(async () =>
            {


                await Task.Run(() =>
                {


                });

            });


        }
        public async Task<KeyValuePair<long, int>> getFolderSize(string folder)
        {

            await Task.Run(() =>
            {
                getDirectories(folder);
                procet = (folderS / size) * 100;
                System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                {
                    FolderSize.Content = Math.Round(folderS / 1024, 2) + "GB";
                    foldersize.Content = Math.Round(folderS) + "MB";
                });

                int x = ((int)Math.Ceiling(procet)) * 10;
                for (int i = 0; i <= x; i++)
                {
                    Thread.Sleep(20);
                    System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                    {
                        rect.Width++;
                    });
                }
                System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                {
                    folderList.ItemsSource = directories;
                });


            });
            var key = new KeyValuePair<long, int>((long)folderS, 2);

            return key;
            //  MessageBox.Show(Math.Round(procet, 2).ToString());
        }
        public void getDirectories(string path)
        {
            KeyValuePair<int,double> count = getFiles(path);

            foreach (string x in Directory.GetDirectories(path))
            {
                getDirectories(x);
                dirs.Add(new DirectoryInfo(x));

            }
            System.Windows.Application.Current.Dispatcher.Invoke(async delegate
            {
                DirectoryInfo file = new DirectoryInfo(path);
                directories.Add(new foldery() { FolderName = file.Name, FileCount = count.Key ,fullPath=file.FullName, FilesSize= (int)Math.Ceiling(count.Value/1024) });

            });

        }
     
        public KeyValuePair<int, double> getFiles(string path)
        {
            double size = 0;
            int count = 0;
            
            foreach (string xx in Directory.GetFiles(path))
            {
                count++;
                files.Add(new FileInfo(xx));

                folderS += To(new FileInfo(xx).Length, 2);
                size+= To(new FileInfo(xx).Length, 1);
                

            }




            KeyValuePair<int,double> keyValue = new KeyValuePair<int, double>(count,size);
            return keyValue;


        }
       
        public static double To(double value, int kind) =>
    value / Math.Pow(1024, (int)kind);
        private long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }
        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Escape) { this.Close(); }
        }

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foldery obj = ((FrameworkElement)sender).DataContext as foldery;
            openFolder.Tss.Cancel();
            openFolder.Tss.Dispose();
            ts = new CancellationTokenSource();
            openFolder.getFolder(obj.fullPath, ts = new CancellationTokenSource());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object item = folderList.SelectedItem;
            string ID = (folderList.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text;
           foreach(string x in Directory.GetFiles(ID))
            {
                filesList.Add(x);
                System.Diagnostics.Debug.WriteLine(x);
               
            }
    
        }
    }

    public class foldery
    {

      
        public string FolderName { get; set; }
        public int FileCount { get; set; }
        public string fullPath { get; set; }
        public int FilesSize { get; set; }

    }
}

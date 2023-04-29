
using Microsoft.Win32;
using System;

using System.Collections.Generic;

using System.Diagnostics;
using System.IO;
using System.Linq;

using System.Runtime.InteropServices;

using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Notatki
{

   public  class functions
    {
        Label label;
        WrapPanel discWrap;
        Label freeSpace;
        ProgressBar progress;
        WrapPanel disc;
        Image image;
        Folders folder;
        ProcessStartInfo startInfo;
       
       

        DriveInfo[] allDrives = DriveInfo.GetDrives();
        float size = 0;
        int getFolderSize(string path)
        {

            try
            {

                foreach (string file in Directory.GetDirectories(path))
                {

                    DirectoryInfo directory = new DirectoryInfo(file);

                    FileInfo[] plik = directory.GetFiles();
                    foreach (FileInfo p in plik)
                    {
                        size = size + p.Length;

                    }


                    getFolderSize(file);






                }

            }
            catch (System.UnauthorizedAccessException e)
            { MessageBox.Show(e.Message); }

            return ((int)(size / Math.Pow(1024, 2)));
        }
        public List<string> RecommendedPrograms(string ext)
        {
            //Search programs names:
            List<string> names = new List<string>();
            string baseKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\." + ext;
            string s;

            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(baseKey + @"\OpenWithList"))
            {
                if (rk != null)
                {
                    string mruList = (string)rk.GetValue("MRUList");
                    if (mruList != null)
                    {
                        foreach (char c in mruList)
                        {
                            s = rk.GetValue(c.ToString()).ToString();

                            names.Add(s);
                        }
                    }
                }
            }

            if (names.Count == 0)
                return names;

            //Search paths:
            List<string> paths = new List<string>();
            baseKey = @"Software\Classes\Applications\{0}\shell\open\command";

            foreach (string name in names)
                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(String.Format(baseKey, name)))
                {
                    if (rk != null)
                    {
                        s = rk.GetValue("").ToString();
                        s = s.Substring(1, s.IndexOf("\"", 2) - 1); //remove quotes
                        paths.Add(s);
                    }
                }


            return names;





        }
      
        public class Wallpaper
        {
            public Wallpaper() { }

            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);



            public void Set(int style, string url)
            {









                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                switch (style)
                {
                    case 1:

                        key.SetValue(@"WallpaperStyle", 2.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case 2:


                        key.SetValue(@"WallpaperStyle", 1.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case 3:


                        key.SetValue(@"WallpaperStyle", 1.ToString());
                        key.SetValue(@"TileWallpaper", 1.ToString());
                        break;

                }





                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0,
                    url,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
        }
        public ProcessStartInfo newProcess(string path, string workDirectory, string program)
        {
            FileInfo file = new FileInfo(path);
            startInfo = new ProcessStartInfo(file.FullName);
            startInfo.FileName = program;
            startInfo.Verb = "edit";
            startInfo.WorkingDirectory = workDirectory;
            startInfo.Arguments = "\"" + path + "\"";
            return startInfo;


        }
       
        public void drivers( Grid gridlayout, string sort, Folders folder, WrapPanel drivers, Window window, Image img, CancellationTokenSource ts)
        {
            BitmapImage logo;
            foreach (DriveInfo d in allDrives)
            {
                logo = new BitmapImage();
                logo.CacheOption = BitmapCacheOption.OnLoad;
                logo.BeginInit();
                logo.UriSource = new Uri("Resources/change35.png",UriKind.RelativeOrAbsolute);
                logo.EndInit();
                try
                {
                    progress = new ProgressBar();
                    progress.Maximum = d.TotalSize;
                    progress.Width = 100;
                    progress.Height = 20;
                    progress.Value = d.TotalSize - d.TotalFreeSpace;
                    progress.Foreground = new SolidColorBrush(Color.FromRgb(90, 90, 90));
                    label = new Label();
                    label.Content = d.Name;

                    freeSpace = new Label();
                    freeSpace.Content = ((int)(d.AvailableFreeSpace / Math.Pow(1024, 3))) + "GB";

                    discWrap = new WrapPanel();
                    discWrap.Children.Add(progress);
                    discWrap.Children.Add(freeSpace);

                    disc = new WrapPanel();
                    WrapPanel dysk = new WrapPanel();
                    dysk.Orientation = Orientation.Horizontal;
                    image = new Image();
                    image.Width = 20;
                    image.Source = logo;



                    dysk.VerticalAlignment = VerticalAlignment.Center;
                    disc.Orientation = Orientation.Horizontal;
                    disc.Children.Add(image);
                    disc.Children.Add(label);
                    disc.Children.Add(discWrap);
                    discWrap.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);

                    async void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
                    {
                        gridlayout.Children.Clear();
                    
                        if (ts.IsCancellationRequested)
                        {
                           
                            folder.Tss.Cancel();
                            folder.Tss.Dispose();
                            ts = new CancellationTokenSource();
                            folder.getFolder(d.Name, ts);
                            folder.MyStack.Push(d.Name);

                        }
                        else
                        {

                            folder.Tss.Cancel();
                            folder.Tss.Dispose();
                            ts = new CancellationTokenSource();


                            folder.getFolder(d.Name, ts);
                            folder.MyStack.Push(d.Name);
                        }
                       
                    }




                    drivers.Children.Add(disc);
                }
                catch (System.IO.IOException e)
                {

                }
            }
        }
        public void Templattte_Click1(object sender, RoutedEventArgs e, string path, TextBox pathContent)
        {
            OpenWith openWith;
            if (new FileInfo(path).Extension.Split(".")[1] == "exe")
            {
                System.Diagnostics.Process.Start(path);
            }
            else
            {
              openWith = new OpenWith(path, pathContent);
               openWith.Show();
            }

        }
        public static class Copier
        {
            public static async Task CopyFiles(Dictionary<string, string> files, Action<string, long, int, Label> progressCallback)
            {
                long total_size = files.Keys.Select(x => new FileInfo(x).Length).Sum();

                long total_read = 0;
                Label label;
                double progress_size = 100;
                System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                foreach (var item in files)
                {
                    label = new Label();
                    label.Content = item.Key;
                    long total_read_for_file = 0;
                    long oneItem = new FileInfo(item.Key).Length;
                    int size = (int)(oneItem / 1024) / 1024;
                    var from = item.Key;
                    var to = item.Value;

                    using (var outStream = new FileStream(to, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        using (var inStream = new FileStream(from, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await CopyStream(inStream, outStream, x =>
                            {

                                progressCallback(item.Key, (x / 1024) / 1024, size, label); ;
                            });
                        }
                    }

                    total_read += total_read_for_file;
                }
            }

            public static async Task CopyStream(Stream from, Stream to, Action<long> progress)
            {
                int buffer_size = 19910240;
                System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                byte[] buffer = new byte[buffer_size];

                long total_read = 0;

                while (total_read < from.Length)
                {
                    int read = await from.ReadAsync(buffer, 0, buffer_size);

                    await to.WriteAsync(buffer, 0, read);

                    total_read += read;

                    progress(total_read);
                }
            }

        }




        //public void createFileShortCut(string path, string targetPath, string name)
        //{
        //    var wshShell = new WshShell();
        //    IWshRuntimeLibrary.IWshShortcut myShortcut;

        //    var desktopFolder = Environment.GetFolderPath(
        //        Environment.SpecialFolder.DesktopDirectory);

        //    var shortcutPath = desktopFolder + @"\" + name.Split(".")[0] + ".lnk";
        //    myShortcut = (IWshRuntimeLibrary.IWshShortcut)
        //        wshShell.CreateShortcut(shortcutPath);
        //    myShortcut.TargetPath =
        //       path;
        //    myShortcut.Save();

        //}
        public void Kupiuj_Click(object sender, RoutedEventArgs e,TextBox path)
        {

            MenuItem mnu = sender as MenuItem;
            Label sp = null;
            Label label = null;
            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as Label;
                if (!sp.Content.ToString().StartsWith("Data") && !sp.Content.ToString().StartsWith("Data"))
                {
                    Clipboard.SetDataObject(path.Text + "\\" + sp.Content);
                }
                else
                {
                    Clipboard.SetDataObject(sp.Content);

                }


            }

        }
        public void Check_Unchecked(object sender, RoutedEventArgs e, List<string> filestoDelete,Dictionary<string,string> list, TextBox pathContent)
        {
            WrapPanel panel = (WrapPanel)((CheckBox)sender).Parent;
            TextBox label = (TextBox)panel.Children[2];
            filestoDelete.Remove(pathContent.Text + "\\" + label.Text.ToString());
            list.Remove(label.Text.ToString());
           
        }
        public void Check_Checked(object sender, RoutedEventArgs e, List<string> filestoDelete, Dictionary<string, string> list,TextBox pathContent)
        {
            WrapPanel panel = (WrapPanel)((CheckBox)sender).Parent;
            TextBox label = (TextBox)panel.Children[2];
            filestoDelete.Add(pathContent.Text+"\\"+ label.Text.ToString());
            if (!list.ContainsKey(label.Text.ToString()))
            {

                list.Add(label.Text.ToString(), pathContent.Text + "\\" + label.Text.ToString());
            }
          



        }
       
    
         public   void ButtonCreatedByCode_Click4(object sender, RoutedEventArgs e, string fullName, Folders folder,Image image)
        {

            try
            {
                using (var stream = System.IO.File.OpenRead(fullName))
                {

                    try
                    {

                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.StreamSource = stream;
                        bmp.CacheOption = BitmapCacheOption.OnLoad;

                        bmp.EndInit();

                        image.Source = bmp;
                    }
                    catch (System.NotSupportedException ex) { }


                }
            }
            catch (System.IO.FileNotFoundException errro)
            {
               
            }
               














          








        }

      
        public static class LoadImages
        {

            public static async Task LoadWrapPanel(string path, Action<WrapPanel, string> progressCallback)
            {

                
              
                
                    progressCallback(new WrapPanel(), path);
                
                
                
               





            }



            public static async Task CopyFiles(List<FileInfo> filee, int size, Action<BitmapImage,string,FileInfo> progressCallback)
            {
              
                foreach (FileInfo x in filee)
                {
                
                    if (x.Extension == ".jpg"|| x.Extension == ".jpeg"|| x.Extension == ".png"|| x.Extension == ".tiff")
                    {

                        await CreateThumbnail(x.FullName,size, (bit) => { progressCallback(bit, x.FullName,x );}); ; ; ;
                    }
                  
                  
                  
                    
                   
               
                   


                }
           
            }
            public static async Task<ImageSource> ImageSourceForBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                ImageSource newSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());


                return (ImageSource)newSource;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
            public static async Task LoadFolder(string path, int size, Action<BitmapImage, string> progressCallback)
            {

             

                    await CreateThumbnail(path,size, (bit) => { progressCallback(bit,path); }); ;



                

            }
            public static async Task CreateThumbnail(string imagePath, int size, Action<BitmapImage> bitmapp)
            {
                try
                {
                    var bitmap = new BitmapImage();

                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                
                        bitmap.BeginInit();
                        bitmap.DecodePixelWidth = size;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();

                   
                  
                }

                bitmap.Freeze();

                bitmapp(bitmap);

                }
                catch (System.NotSupportedException exe)
                {



                }
            }
         

        }
       
        public static System.Drawing.Bitmap BitmapSourceToBitmap2(BitmapSource srs)
        {
            int width = srs.PixelWidth;
            int height = srs.PixelHeight;
            int stride = width * ((srs.Format.BitsPerPixel + 7) / 8);
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(height * stride);
                srs.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);
                using (var btm = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, ptr))
                {
                    // Clone the bitmap so that we can dispose it and
                    // release the unmanaged memory at ptr
                    return new System.Drawing.Bitmap(btm);
                }
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }
        public   ImageSource ImageSourceForBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                ImageSource newSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());


                return (ImageSource)newSource;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        
        
    }

}

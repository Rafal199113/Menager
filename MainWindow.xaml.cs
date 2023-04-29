using Menager.others;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Notatki;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Media;
using System.Net;
using System.Security.Policy;
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
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Menager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       
        Folders folder;
        functions functions;

        internal Notifications notifications { get; private set; }

        ManagerFilesSettings MenagerSettings;
        Icon set;


        //Variable Declartions
        bool checkdAll = false;
        string sort = "date";
        string currectCollection = "Pictures";
        public static RoutedCommand MyCommand = new RoutedCommand();
        CancellationTokenSource ts;


        //Controls Declarations
        WrapPanel Container = new WrapPanel();
        WrapPanel Folder;
        Label name;
        Stream stream;

        //List Collections
        List<string> filestoDelete = new List<string>();
        List<string> paths = new List<string>();
        List<string> LiveWallpapers;
        List<List<string>> fouvourite = new List<List<string>>() { new List<string>(), new List<string>(), new List<string>(), new List<string>() };
        Dictionary<string, string> selectedFilesPaths = new Dictionary<string, string>();
        Stack<string> filesStack = new Stack<string>();


        //Context Menus
        ContextMenu bookContext = new ContextMenu();
        ContextMenu context;
        MenuContexts contexts;



        public MainWindow()
        {

           
            try
            {
                InitializeComponent();
                functions = new functions();
                 notifications = new Notifications(gridddd, this,ts);
           





                MenuItem close = new MenuItem();
                close.Foreground = new SolidColorBrush(Colors.White);
                close.Header = "Close";
                close.Click += Close_Click; ;

                bookContext.Items.Add(close);
              
                //get favourites
                string last = File.ReadAllText(@"data/favourite.txt", Encoding.UTF8);
                fouvourite = JsonConvert.DeserializeObject<List<List<string>>>(last);
             
                //get settings
                set = new Icon(); ;
                string date = File.ReadAllText(@"data/MenagerSettings.txt", Encoding.UTF8);
                set = JsonConvert.DeserializeObject<Icon>(date);
              

                //Set background of main window and set DataContext to style class
                this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(set.iconColor);
                this.DataContext = set;


                MyCommand.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));


              


               






                ContextMenu c = new ContextMenu();

                MenuItem paste = new MenuItem();
                paste.Header = "Paste";
                paste.Click += Wklej;

                MenuItem selectAll = new MenuItem();
                selectAll.Header = "Select All";
                selectAll.Click += SelectAll_Click;


                MenuItem newFolder = new MenuItem();
                newFolder.Header = "New Folder";
                newFolder.Click += NewFolder_Click; ;



                c.Items.Add(newFolder);
                c.Items.Add(paste);
                c.Items.Add(selectAll);


                scroll.ContextMenu = c;










                ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM MSFT_PhysicalDisk");
                string type = "";
                scope.Connect();
                searcher.Scope = scope;
                ts = new CancellationTokenSource();


                contexts = new MenuContexts(pathContent,this,set, notifications,ts);
                context = new ContextMenu();
             
                folder = new Folders(FolderInfo, GridLayout, pathContent, context, sort, this, FolderInfo, set, bookMarksPanel, ts,contexts);
             
                folder.LastFolders = lastFolders;
                folder.LastFiles = lastFiles;

                folder.FilestoDelete = filestoDelete;

                folder.getFolder("D:\\", ts);


                functions.drivers( GridLayout, sort, folder, Drivers, this, view, ts);


              
                contexts.folder= folder;


               


                

              

              




                    context.Name = "menu";

            
             
                
               Container.Orientation = Orientation.Vertical;
                Container.Height = 800;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                
            }


           





      






















        }

        

        

       

        private void Remove_Click(object sender, RoutedEventArgs e)
        {

            MenuItem mnu = sender as MenuItem;
            Image sp = null;

            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as System.Windows.Controls.Image;



            }
            WrapPanel panel = sp.Parent as WrapPanel;
            TextBox text = panel.Children[1] as TextBox;
            if (Directory.Exists(text.Text))
            {
                fouvourite[0].Remove(text.Text);

            }
            else
            {
                FileInfo file = new FileInfo(text.Text);
                if (file.Extension == ".jpg" || file.Extension == ".png" || file.Extension == ".jfif" || file.Extension == ".jpeg")
                {
                    fouvourite[1].Remove(text.Text);
                }
                else
                {
                    fouvourite[2].Remove(text.Text);
                }
            }

        }



        private void NewFolder_Click(object sender, RoutedEventArgs e)
        {
            notifications.show("Folder have been created",ts);
            Directory.CreateDirectory(pathContent.Text + "//New Folder");
            folder.getFolder(pathContent.Text, ts);
        }

       
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Border element in GridLayout.Children)
            {
                WrapPanel panel = element.Child as WrapPanel;
                TextBox text = panel.Children[1] as TextBox;
                string path;
                
                if (pathContent.Text == "Collections") path = text.Text;
                else path = pathContent.Text + "\\" + text.Text;
          
               


                if (element.BorderBrush.ToString() == new SolidColorBrush(Colors.Green).ToString())
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString(set.borderColor);
                    element.BorderBrush = brush;
                    filestoDelete.Remove(path);
                    folder.List.Remove(path);


                }
                else
                {
                    filestoDelete.Add(path);
                    element.BorderBrush = new SolidColorBrush(Colors.Green);
                    try
                    {
                        folder.List.Add(path, path);
                    }
                    catch (System.ArgumentException error)
                    {
                        folder.List.Remove(path);
                        folder.List.Add(path, path);
                    }

                }





            }

        }
        private void Favourite_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            Image sp = null;

            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as Image;


            }

            WrapPanel panel = sp.Parent as WrapPanel;
            TextBox txt = panel.Children[1] as TextBox;
            if (Directory.Exists(pathContent.Text + "\\" + txt.Text))
            {
                fouvourite[0].Add(pathContent.Text + "\\" + txt.Text);
            }
            else
            {
                FileInfo file = new FileInfo(pathContent.Text + "\\" + txt.Text);
                if (file.Extension == ".jpg" || file.Extension == ".png" || file.Extension == ".jfif" || file.Extension == ".jpeg")
                {

                    fouvourite[1].Add(pathContent.Text + "\\" + txt.Text);
                }
                else if (file.Extension == ".exe")
                {
                    fouvourite[3].Add(pathContent.Text + "\\" + txt.Text);
                }
                else
                {
                    fouvourite[2].Add(pathContent.Text + "\\" + txt.Text);
                }


            }

            string json = JsonConvert.SerializeObject(fouvourite, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(@"data/favourite.txt", json);

        }








        private async void Wklej(object sender, RoutedEventArgs e)
        {

            var dictionary = new Dictionary<string, string>();



            MenuItem mnu = sender as MenuItem;
            WrapPanel sp = null;
            Label label = null;
            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as WrapPanel;


            }
            Dictionary<string, string> filesDict = new Dictionary<string, string>();
            if (pathContent.Text != "Collections")
            {
                foreach (string x in folder.FilestoDelete)
                {
                    int z = x.Split("\\").Count();

                    if (Directory.Exists(x))
                    {

                        Dictionary<string, string> dict = CopyDirectory(x, pathContent.Text + "\\" + x.Split("\\")[z - 1]);
                        await Copier.CopyFiles(dict, (prog, file, filesize, totalsize, lab) => { prgBaseMap.Value = prog; fileName.Text = "File: " + file; fileSize.Text = "Size " + filesize + " MB/" + totalsize + " MB"; FolderInfo.Content = lab.Content; });

                    }
                    else
                    {

                        filesDict.Add(x, pathContent.Text + "\\" + x.Split("\\")[z - 1]);

                        System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                        await Copier.CopyFiles(filesDict, (prog, file, filesize, totalsize, lab) => { prgBaseMap.Value = prog; fileName.Text = file; fileSize.Text = filesize + " MB/" + totalsize + " MB"; FolderInfo.Content = lab.Content; });
                    }

               ;






                }
                folder.getFolder(pathContent.Text, ts);
                folder.FilestoDelete.Clear();
            }









        }
        Dictionary<string, string> dic = new Dictionary<string, string>();

        public Label lab { get; private set; }

        private Dictionary<string, string> CopyDirectory(string source, string target)
        {

            DirectoryInfo info;
            info = new DirectoryInfo(source);
            try
            {
                foreach (FileInfo file in info.GetFiles())
                {



                    Directory.CreateDirectory(target);
                    dic.Add(source + "\\" + file.Name, target + "\\" + file.Name);

                }
                foreach (DirectoryInfo directory in info.GetDirectories())
                {
                    string dir = pathContent.Text + "\\" + info.Name + "\\" + directory.Name;
                    Directory.CreateDirectory(dir);

                    CopyDirectory(directory.FullName, dir);


                }

            }
            catch (System.IO.DirectoryNotFoundException error)
            {

                dic.Add(source, target);
            }

            return dic;
        }



        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.Key == Key.Enter)
            {

                GridLayout.Children.Clear();
                folder.Tss.Cancel();
                folder.Tss.Dispose();
                ts = new CancellationTokenSource();
                folder.getFolder(pathContent.Text, ts);

            }
        }

        private void iconPics_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ts.IsCancellationRequested)
            {

                GridLayout.Children.Clear();
                try
                {
                    string back = folder.MyStack.Pop() as string;
                    folder.Tss.Cancel();
                    folder.Tss.Dispose();
                    ts = new CancellationTokenSource();
                    folder.getFolder(back, ts);


                }
                catch (System.InvalidOperationException error)
                {

                }

            }
            else
            {
                folder.Tss.Cancel();
                folder.Tss.Dispose();
                ts = new CancellationTokenSource();
                GridLayout.Children.Clear();
                try
                {
                    string back = folder.MyStack.Pop() as string;

                    folder.getFolder(back, ts);


                }
                catch (System.InvalidOperationException error)
                {
                    folder.getFolder(pathContent.Text, ts);
                }

              

            }


        }





        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {


            var list = filestoDelete.ToList();
            if (folder.FilestoDelete.Count != 0)
            {

                for (int i = 0; i < filestoDelete.Count(); i++)
                {

                    string path = (list[i]);


                    if (Directory.Exists(path))
                    {

                        Directory.Delete(list[i], true);
                        folder.List.Clear();
                        filestoDelete.Clear();
                    }
                    else
                    {

                        File.Delete(path);
                    }





                    folder.FilestoDelete.Remove(list[i]);


                }

            }
            GridLayout.Children.Clear();
            ts.Cancel();
            ts.Dispose();
            ts = new CancellationTokenSource();
            folder.getFolder(pathContent.Text, ts);
        }





        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
         
            this.Close();
        }

        private void TextBox_KeyDown_1(object sender, KeyEventArgs e)
        {

            int x = GridLayout.Children.Count - 1;
            Label? panel;
            List<string> list = new List<string>();
            if (e.Key == Key.Enter)
            {
                WrapPanel? scroll = FindName("folders") as WrapPanel;
                List<WrapPanel> lista = new List<WrapPanel>();
                folder.Search = searcher.Text;



                foreach (Border border in GridLayout.Children)
                {

                    WrapPanel c = border.Child as WrapPanel;
                    TextBox text = c.Children[1] as TextBox;
                    if (searcher.Text == string.Empty)
                    {
                        list.Clear();

                    }
                    else
                    if (text.Text.ToLower().Contains(searcher.Text.ToLower()))
                    {
                        list.Add(pathContent.Text + "\\" + text.Text.ToLower());
                        border.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(set.borderColor);

                    }

                }



                SelectedFiles selectedFiles = new SelectedFiles(list, pathContent, folder, ts, set,functions);
                selectedFiles.Show();

            }

        }




        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            GridLayout.Children.Clear();
            var result = sender as RadioButton;
            sort = result.Name;
            folder.Sort = sort;
            folder.Tss.Cancel();
            folder.Tss.Dispose();
          ts = new CancellationTokenSource();
            folder.getFolder(pathContent.Text, ts);
        }

        private async void WrapPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            folder.Tss.Cancel();
            folder.Tss.Dispose();
            ts = new CancellationTokenSource();
            await Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine("cipeczka"); ;
                System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                {
                    System.Diagnostics.Debug.WriteLine("cycuszki"); ;
                    string userRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
                    // Now let's get C:\Users\Username\Downloads:
                    string downloadFolder = System.IO.Path.Combine(userRoot, "Downloads");
                    WrapPanel panel = sender as WrapPanel;
                    GridLayout.Children.Clear();
                    switch (panel.Name)
                    {
                        case "download":

                            await folder.getFolder(downloadFolder, ts);
                            break;
                        case "startup":

                            await folder.getFolder(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ts);
                            break;
                        case "desktop":




                            await folder.getFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ts);
                            break;
                    }


                });
            });


        }


        private void minimalize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Media.Stop();

          
            this.WindowState = WindowState.Minimized;
        }

        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            searcher.Focus();
        }

        private void cipeczka_KeyDown(object sender, KeyEventArgs e)
        {
            KeyGesture exit = new KeyGesture(Key.Space, ModifierKeys.Control);
            KeyGesture increaseOpacity = new KeyGesture(Key.Add, ModifierKeys.Control);
            KeyGesture decraseOpacity = new KeyGesture(Key.Subtract, ModifierKeys.Control);
            KeyGesture nextWallpaper = new KeyGesture(Key.A, ModifierKeys.Control);
            KeyGesture lastWallpaper = new KeyGesture(Key.PageDown, ModifierKeys.Control);
            KeyGesture selectAll = new KeyGesture(Key.E, ModifierKeys.Control);
            KeyGesture quick = new KeyGesture(Key.Q, ModifierKeys.Control);
            KeyGesture filesInfo = new KeyGesture(Key.A, ModifierKeys.Control);

            if (selectAll.Matches(null, e))
            {
                foreach (WrapPanel element in GridLayout.Children)
                {
                    CheckBox check = element.Children[0] as CheckBox;
                    switch (checkdAll)
                    {
                        case false:

                            check.IsChecked = true;

                            break;
                        case true:

                            check.IsChecked = false;

                            break;

                    }





                }
                checkdAll = !checkdAll;
            }


            if (e.Key == Key.P)
            {



            }
            if (filesInfo.Matches(null, e))
            {

               SelectedFiles selectedFiles = new SelectedFiles(folder.FilestoDelete, pathContent, folder, new CancellationTokenSource(), set,functions); ;
                selectedFiles.Show();

            }
            if (quick.Matches(null, e))
            {
                if (last.IsVisible == true) { last.Visibility = Visibility.Hidden; }
                else
                {
                    last.Visibility = Visibility.Visible;
                }




            }


            if (exit.Matches(null, e))
            {
                this.WindowState = WindowState.Minimized;
            }
            if (increaseOpacity.Matches(null, e))
            {

                set.iconWidth += 10;
                GridLayout.Children.Clear();
                folder.getFolder(pathContent.Text, ts);
            }
            if (decraseOpacity.Matches(null, e))
            {

                set.iconWidth -= 10;
                GridLayout.Children.Clear();
                folder.getFolder(pathContent.Text, ts);
            }
            if (e.Key == Key.Back)
            {
                try
                {
                    folder.Tss.Cancel();
                    folder.Tss.Dispose();
                    ts = new CancellationTokenSource();

                    GridLayout.Children.Clear();
                    folder.getFolder((string)folder.MyStack.Pop(), ts);

                }
                catch (System.InvalidOperationException error)
                {
                    ts = new CancellationTokenSource();
                    GridLayout.Children.Clear();
                    folder.getFolder(pathContent.Text, ts);
                }

            }
            if(e.Key == Key.Enter)
            {
                
                 
            }
            if (e.Key == Key.Delete)
            {
                var list = filestoDelete.ToList();
                if (folder.FilestoDelete.Count != 0)
                {

                    for (int i = 0; i < filestoDelete.Count(); i++)
                    {

                        string path = (pathContent.Text + "\\" + list[i]);




                        try
                        {
                            File.Delete(path);
                        }
                        catch (System.IO.IOException error)
                        {
                            Directory.Delete(list[i]);

                        }




                        folder.FilestoDelete.Remove(list[i]);


                    }

                }
                GridLayout.Children.Clear();
                folder.getFolder(pathContent.Text, ts);

            }

        }

        private void settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            MenagerSettings = new ManagerFilesSettings(this);
           MenagerSettings.Show();
            this.Close();
         

        }
        void ButtonCreatedByCode_Click4(object sender, RoutedEventArgs e, string name)
        {
            var fi1 = new FileInfo(name);

            try
            {
                using (var stream = File.OpenRead(fi1.FullName))
                {
                    try
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.StreamSource = stream;
                        bmp.CacheOption = BitmapCacheOption.OnLoad;

                        bmp.EndInit();



                        view.Source = bmp;







                    }
                    catch (System.NotSupportedException error)
                    {
                        lastFiles.Children.Remove((Label)sender);
                    }



                }

            }
            catch (System.IO.FileNotFoundException error)
            {

            }
            catch (System.IO.DirectoryNotFoundException error)
            {

                lastFiles.Children.Remove((Label)sender);

            }



        }
        public static class Copier
        {
            public static async Task CopyFiles(Dictionary<string, string> files, Action<double, string, int, int, Label> progressCallback)
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
                    int oneItem = (int)new FileInfo(item.Key).Length;
                    int size = (oneItem / 1024) / 1024;
                    var from = item.Key;
                    var to = item.Value;

                    using (var outStream = new FileStream(to, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        using (var inStream = new FileStream(from, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await CopyStream(inStream, outStream, x =>
                            {
                                total_read_for_file = x;
                                progressCallback(((total_read + total_read_for_file) / (double)total_size) * progress_size, item.Key, ((int)x / 1024) / 1024, size, label); ;
                            });
                        }
                    }

                    total_read += total_read_for_file;
                }
            }

            public static async Task CopyStream(Stream from, Stream to, Action<long> progress)
            {
                int buffer_size = 10240;
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

        private void refresh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            GridLayout.Children.Clear();

            if (ts.IsCancellationRequested)
            {

                if (pathContent.Text == "Collections")
                {

                    foldersList_MouseLeftButtonDown(sender, e);
                }
                else
                {
                    folder.Tss.Cancel();
                    folder.Tss.Dispose();
                    ts = new CancellationTokenSource();
                    folder.getFolder(pathContent.Text, ts);
                }
            }
            else
            {

             

                if (pathContent.Text == "Collections")
                {
                    foldersList_MouseLeftButtonDown(sender, e);
                }
                else
                {
                    folder.Tss.Cancel();
                    folder.Tss.Dispose();
                    ts = new CancellationTokenSource();
                    
                        folder.getFolder(pathContent.Text, ts);

                  
             
                }

            }





        }






        private void searcher_TextChanged(object sender, TextChangedEventArgs e)
        {




            WrapPanel? scroll = FindName("folders") as WrapPanel;
            List<WrapPanel> lista = new List<WrapPanel>();
            folder.Search = searcher.Text;


            foreach (Border border in GridLayout.Children)
            {

                WrapPanel c = border.Child as WrapPanel;
                TextBox text = c.Children[1] as TextBox;
                if (searcher.Text == string.Empty)
                {

                    border.Background = new SolidColorBrush(Colors.Transparent); ;
                }
                else if
                     (text.Text.ToLower().Contains(searcher.Text.ToLower()))
                {

                    border.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(set.borderColor);
                    text.Foreground= (SolidColorBrush)new BrushConverter().ConvertFrom(set.labelColor);

                }
                else
                {

                    border.Background = new SolidColorBrush(Colors.Transparent); ;

                }

            }
        }

        private void print_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
          SelectedFiles selectedFiles = new SelectedFiles(folder.FilestoDelete, pathContent, folder, new CancellationTokenSource(), set, functions); ;
            selectedFiles.Show();
        }

        private void addBookMark_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            Border border = new Border();
            border.Style = this.FindResource("border") as Style;
            border.Width = 100;
            border.BorderThickness = new Thickness(1, 1, 1, 0);
            border.Margin = new Thickness(5);
            Label label = new Label();
            label.Content = pathContent.Text;
            label.Margin = new Thickness(5);
            label.MouseLeftButtonDown += Label_MouseLeftButtonDown;
            border.Child = label;
            border.ContextMenu = bookContext;
            bookMarksPanel.Children.Insert(0, border);


        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            Border sp = null;

            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as Border;
                bookMarksPanel.Children.Remove(sp);

            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Label lab = sender as Label;
            folder.Tss.Cancel();
            folder.Tss.Dispose();
            ts = new CancellationTokenSource();

            folder.getFolder(lab.Content.ToString(), ts);
        }

        private async void foldersList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string last = File.ReadAllText(@"data/favourite.txt", Encoding.UTF8);
            fouvourite = JsonConvert.DeserializeObject<List<List<string>>>(last);


            pathContent.Text = "Collections";

            GridLayout.Children.Clear();
            Label lab = sender as Label;
            if (lab != null)
            {
                currectCollection = lab.Content.ToString();

            }

            int GridRow = 0;
            int columnCounter = 0;
            Border border;
            WrapPanel panel;
            TextBox name;
            Image image;
            List<string> currectList = null;
            switch (currectCollection)
            {
                case "Folders":
                    currectList = fouvourite[0];
                    break;
                case "Pictures":
                    currectList = fouvourite[1];
                    break;
                case "Files":
                    currectList = fouvourite[2];
                    break;
                case "Programs":
                    currectList = fouvourite[3];
                    break;
                case "Database":
                   

                    break;




            }
            GridLayout.Children.Clear();
            if (!folder.Tss.IsCancellationRequested)
            {
                folder.cancel();
            }
            if (currectList != null)
            {
                foreach (string x in currectList)
                {


                    border = new Border();
                    border.Style = this.FindResource("iconBorder") as Style;
                    panel = new WrapPanel();
                    panel.Style = this.FindResource("IconPanel") as Style;


                    image = new Image();
                
                    image.Style = this.FindResource("IconImage") as Style;
                    if (Directory.Exists(x))
                    {
                        BitmapImage img = new BitmapImage();


                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.UriSource = new Uri("assets/Folder.png", UriKind.RelativeOrAbsolute);
                        img.EndInit();
                        image.Source = img;
                       
                        image.MouseLeftButtonDown += (sender, e) => folder.Templattte_Click(sender, e, pathContent.Text, x, ts, filestoDelete);
                    }
                    else
                    {
                        string file = x.Split(".").Last();
                        if (file == "jpg" || file == "png" || file == "jfif" || file == "jpeg")
                        {
                            using (stream = File.OpenRead(x))
                            {
                                stream.Position = 0;

                                var smallerBitmapImage = new BitmapImage();
                                smallerBitmapImage.BeginInit();
                                smallerBitmapImage.DecodePixelWidth = 180;
                                smallerBitmapImage.StreamSource = stream;
                                smallerBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                smallerBitmapImage.EndInit();


                                var convertedBitmap = new FormatConvertedBitmap();
                                convertedBitmap.BeginInit();
                                convertedBitmap.Source = smallerBitmapImage;

                                convertedBitmap.EndInit();
                                convertedBitmap.Freeze();

                                image.Source = convertedBitmap;
                               






                                stream.Close();

                            }

                            image.MouseLeftButtonDown += (sender, e) => folder.Templattte_Click1(sender, e, x, pathContent);
                            image.Width = 180;
                        }
                        else
                        {
                            if (!File.Exists(x)) { continue; }
                            try
                            {
                                System.Drawing.Icon iconn = System.Drawing.Icon.ExtractAssociatedIcon(x);
                                System.Drawing.Bitmap bmp = iconn.ToBitmap();
                                image.Source = functions.ImageSourceForBitmap(bmp);
                               
                                image.MouseLeftButtonDown += (sender, e) => folder.Templattte_Click1(sender, e, x, pathContent);
                            }
                            catch (System.IO.FileNotFoundException ex)
                            {

                            }

                        }

                    }


                    image.MouseDown += (sender, e) => folder.Image_MouseDown(sender, e, x);
                    image.ContextMenu = contexts.FileContext(x);


                    name = new TextBox();
                    name.Text = x;
                    name.Style = this.FindResource("IconTextBox") as Style;
                    name.MouseDoubleClick += (sender, e) => folder.IconName_MouseDoubleClick(sender, e, x, pathContent);
                    panel.Children.Add(image);
                    panel.Children.Add(name);

                    border.Child = panel;
                    System.Windows.Controls.Grid.SetRow(border, GridRow);
                    System.Windows.Controls.Grid.SetColumn(border, columnCounter);
                    columnCounter++;
                    if (columnCounter % 3 == 0)
                    {
                        columnCounter = 0;

                        GridLayout.RowDefinitions.Add(new RowDefinition());


                        GridRow++;
                    }



                    GridLayout.Children.Add(border);




                }


            }


        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {

        }
       
    }
}

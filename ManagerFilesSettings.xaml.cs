using ColorPicker;
using Menager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
using System.Xml.Linq;

namespace Notatki
{
    /// <summary>
    /// Logika interakcji dla klasy ManagerFilesSettings.xaml
    /// </summary>
     
    public partial class ManagerFilesSettings : Window
    {
        string radio;
        TextBox wallpaperLabel;
        System.Windows.Controls.Image finalImage;
        settings set;
        getsettings getset;
        SolidColorBrush color;
        Label font;
        string selectedFolder;
        Window window;
        private FileStream stream;
        functions functions = new functions();
        Thread th1;
    
        public class settings
        {
            public string Video { get; set; }
            public System.Windows.Media.Brush labelColor { get; set; }
            public System.Windows.Media.Brush fontColor { get; set; }
            public System.Windows.Media.Brush borderColor { get; set; }
            public System.Windows.Media.Brush iconColor { get; set; }
            public System.Windows.Media.Brush backgroundColor { get; set; }
            public int  fontsize { get; set; }
            public int  iconsize { get; set; }
            public string fontfamily { get; set; }
            public string selectedWallpaperFolder { get; set; }
            public bool transparent { get; set; }
            public int iconWidth { get; set; }




        }
        public class getsettings
        {
            public string Video { get; set; }
            public string labelColor { get; set; }
            public string fontColor { get; set; }
            public string borderColor { get; set; }
            public string iconColor { get; set; }
            public string backgroundColor { get; set; }
            public int fontsize { get; set; }
            public int iconsize { get; set; }
            public string fontfamily { get; set; }
            public string selectedWallpaperFolder { get; set; }
            public bool transparent { get; set; }



        }
        public ManagerFilesSettings(Window window)
        {
            this.window = window;
            set = new settings();
           getset=new getsettings(); ;
            string date = File.ReadAllText(@"data/MenagerSettings.txt", Encoding.UTF8);
          
            getset = JsonConvert.DeserializeObject<getsettings>(date);
            selectedFolder = getset.selectedWallpaperFolder;
            this.DataContext = getset;

           
            InitializeComponent();

            loadLiveWallpapers(getset.selectedWallpaperFolder);
          


            using (System.Drawing.Text.InstalledFontCollection col = new System.Drawing.Text.InstalledFontCollection())
            {
                foreach (System.Drawing.FontFamily fa in col.Families)
                {
                    System.Windows.Media.FontFamily mfont = new System.Windows.Media.FontFamily(fa.Name);
                    font = new Label();
                    font.Content= fa.Name;
                    font.FontFamily = mfont;
                    fontsList.Items.Add(fa.Name);
                }
            }
        }
        private void loadLiveWallpapers(string path)
        {
            try
            {
                list.Items.Clear();
                Task.Run(async () =>
                {


                    await Task.Run(() =>
                    {


                        load(path);
                    });

                });

          

            }
            catch (System.ArgumentNullException error) { }
     


        }
        async void load(string path)
        {

      
           

            th1 = new Thread(async () =>
            {
            
                List<string> listt = Directory.GetFiles(path).ToList();
                List<FileInfo> filee = new List<FileInfo>();

                System.Windows.Controls.Image img = new System.Windows.Controls.Image();

                foreach (string file in listt)
                {
                    filee.Add(new FileInfo(file));
                }
                filee.Sort((x, y) => x.CreationTime.CompareTo(y.CreationTime));
                filee.Reverse();


                await functions.LoadImages.CopyFiles(filee,300, (x,path,file) => {
                    Dispatcher.Invoke(() => {

                        if (list.Dispatcher.CheckAccess())
                        {
                            img = new System.Windows.Controls.Image();
                            img.MouseLeftButtonDown += (sender, e) => WallpaperLabel_MouseEnter(sender, e, path);
                            img.Width = 350;
                        
                            img.Source = x;
                           
                            list.Items.Add(img);
                            

                          





                        }




                    });
                });


            });




            th1.SetApartmentState(ApartmentState.STA);
            th1.Start();

            th1.Join();


        }

        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task WallpaperLabel_MouseEnter(object sender, MouseEventArgs e, string path)
        {
           
           
           
            if(path.Contains(" "))
            {
                string newName = path.Replace(" ", "-");


                await rename(path, newName);
                viewer.Source = new System.Uri(path);
                set.Video = newName;

             
            }else
            {
                viewer.Source = new System.Uri(path);
                set.Video = path;
            }
        }

        
        private  async Task rename(string path, string newName)
        {
            System.IO.File.Move(path, newName);
        }
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
              
                loadLiveWallpapers(fbd.SelectedPath);
                if(fbd.SelectedPath!=null) selectedFolder = fbd.SelectedPath;
            }
               
        }

        private void SquarePicker_ColorChanged(object sender, RoutedEventArgs e)
        {
            SquarePicker picekr = sender as SquarePicker;
            System.Diagnostics.Debug.WriteLine(radio);
            switch (radio)
            {
                case "labels":
                    color= new SolidColorBrush(System.Windows.Media.Color.FromArgb(picekr.SelectedColor.A, picekr.SelectedColor.R, picekr.SelectedColor.G, picekr.SelectedColor.B));
                    rect.Fill = color;
                   
                    break;
                 case "fonts":
                    color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(picekr.SelectedColor.A, picekr.SelectedColor.R, picekr.SelectedColor.G, picekr.SelectedColor.B));
                    rect5.Fill = color;
                   


                    break;
                          case "border":

                    color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(picekr.SelectedColor.A, picekr.SelectedColor.R, picekr.SelectedColor.G, picekr.SelectedColor.B));
                    rect3.Fill = color;
                    

                    break;
                case "mouseEnterColor":

                    color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(picekr.SelectedColor.A, picekr.SelectedColor.R, picekr.SelectedColor.G, picekr.SelectedColor.B));
                    rect4.Fill = color;


                    break;
                case "backgroundColor":

                    color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(picekr.SelectedColor.A, picekr.SelectedColor.R, picekr.SelectedColor.G, picekr.SelectedColor.B));
                    rect6.Fill = color;


                    break;
            }
          
        }

        private void labelColor_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void labels_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rbutton = sender as RadioButton;
            radio  = rbutton.Name;
             

        }

        private void save_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            set.Video = viewer.Source.AbsolutePath;
            set.labelColor = rect.Fill;
            set.fontColor = rect5.Fill;
            set.borderColor = rect3.Fill;
            set.iconColor = rect4.Fill;
            set.backgroundColor = rect6.Fill;
            set.fontsize = Int32.Parse(size.Text);
            set.iconsize = Int32.Parse(iconFontSize.Text);
            set.fontfamily = fontsList.SelectedValue.ToString();
            set.selectedWallpaperFolder = selectedFolder;
            set.iconWidth = 100;
            if ((bool)transparent.IsChecked)
            {
                set.transparent = true;
            }
            else
            {
                set.transparent = false;
            }
         
            string json = JsonConvert.SerializeObject(set, Formatting.Indented);
            
            File.WriteAllText(@"data/MenagerSettings.txt", json);

            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}

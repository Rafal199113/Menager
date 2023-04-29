using Microsoft.Win32;
using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Shell32;
using Newtonsoft.Json;
using Notatki;

namespace Menager
{
    /// <summary>
    /// Logika interakcji dla klasy DesktopBackgound.xaml
    /// </summary>
    public partial class DesktopBackgound : Window
    {
        Icon set;
        Shell shellObject = new Shell();
        string v;
        Window window;
        public DesktopBackgound(string v, Window slider)
        {
            this.window = slider;
            this.v = v;
            set = new Icon(); ;
            string date = File.ReadAllText(@"data/MenagerSettings.txt", Encoding.UTF8);
            set = JsonConvert.DeserializeObject<Icon>(date);
            this.DataContext = set;
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            WrapPanel panel = radio.Parent as WrapPanel;
            Label lab = panel.Children[0] as Label;
            
         
            switch (lab.Content.ToString())
            {
                case "Fill":
                    Wallpaper.Set(new Uri(v), Wallpaper.Style.Fill);
                    shellObject.ToggleDesktop();
                    break;
                case "Fit":
                    Wallpaper.Set(new Uri(v), Wallpaper.Style.Fit);
                    shellObject.ToggleDesktop();
                    break;
                
                case "Stretch":
                    Wallpaper.Set(new Uri(v), Wallpaper.Style.Stretch);
                    shellObject.ToggleDesktop();
                    break;
                case "Tile":
                    Wallpaper.Set(new Uri(v), Wallpaper.Style.Tile);
                    shellObject.ToggleDesktop();
                    break;
                case "Center":
                    Wallpaper.Set(new Uri(v), Wallpaper.Style.Center);
                    shellObject.ToggleDesktop();
                    break;


            } 
        }
        public sealed class Wallpaper
        {
            Wallpaper() { }

            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

            public enum Style : int
            {
                Fill,
                Fit,
               
                Stretch,
                Tile,
                Center
            }

            public static void Set(Uri uri, Style style)
            {
                System.IO.Stream s = new System.Net.WebClient().OpenRead(uri.ToString());

                System.Drawing.Image img = System.Drawing.Image.FromStream(s);
                string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
                img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                if (style == Style.Fill)
                {
                    key.SetValue(@"WallpaperStyle", 10.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Fit)
                {
                    key.SetValue(@"WallpaperStyle", 6.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
              
                if (style == Style.Stretch)
                {
                    key.SetValue(@"WallpaperStyle", 2.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Tile)
                {
                    key.SetValue(@"WallpaperStyle", 0.ToString());
                    key.SetValue(@"TileWallpaper", 1.ToString());
                }
                if (style == Style.Center)
                {
                    key.SetValue(@"WallpaperStyle", 0.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }

                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0,
                    tempPath,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                window.WindowState = WindowState.Maximized;
                this.Close();
                
            }

        }
    }
}

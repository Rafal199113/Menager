using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using Shell32;
namespace Menager
{
    /// <summary>
    /// Logika interakcji dla klasy Slider.xaml
    /// </summary>
    public partial class Slider : Window
    {
        List<FileInfo> lista = new List<FileInfo>();
        FileInfo file;
        Window window;
        int index = 0;
        public Slider(string text, MainWindow mainWindow, string text1)
        {
          this.window= mainWindow;

            InitializeComponent();
            try
            {
                List<string> x = Directory.GetFiles(text).ToList();
                x.ForEach((s) =>
                {
                    if (!Directory.Exists(s))
                    {
                        file = new FileInfo(s);
                        if (file.Extension == ".jpg" || file.Extension == ".png" || file.Extension == ".jfif" || file.Extension == ".jpeg")
                        {



                            lista.Add(file);

                        }

                    }


                });
                FileInfo find = lista.Find(x => x.FullName.Equals(text + "\\" + text1));
                if (find != null)
                {
                    currectimage.Source = new BitmapImage(new Uri(find.FullName));
                }




                index = lista.IndexOf(find);

            }
            catch(System.IO.DirectoryNotFoundException error)
            {
                this.Close();
            }
          
       
        }

        private async  void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (e.Key == Key.Space)
            {

                try
                {
                    currectimage.Source = new BitmapImage(new Uri(lista[++index].FullName));
                }
                catch(System.ArgumentOutOfRangeException error)
                {
                    index = 0;
                    currectimage.Source = new BitmapImage(new Uri(lista[index].FullName));
                        
                }
                    
                
               

            }
            if(e.Key== Key.Escape) { this.Close(); }
            if(e.Key== Key.C) new DesktopBackgound(currectimage.Source.ToString(), this).Show();
            if(e.Key==Key.V)new Edit(currectimage.Source.ToString(),this).Show();
         
        }
   
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.XButton1:
                    if (index == 0) { index = lista.Count - 1; }
                    index--;

                    currectimage.Source = new BitmapImage(new Uri(lista[index].FullName));

                    break;
                case MouseButton.XButton2:
                    index++;
                    if (index == lista.Count)
                    {
                          index = 0;
                    }

                      
            
                  
                        currectimage.Source = new BitmapImage(new Uri(lista[index].FullName));
                    
                
                    if (index == lista.Count - 1) { index = 0; }
                    break;
                default:
                    break;
            }
        }
      
    }
}

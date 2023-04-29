using Microsoft.Win32;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Menager
{
    /// <summary>
    /// Logika interakcji dla klasy Edit.xaml
    /// </summary>
    public partial class Edit : Window
    {
        Rectangle rectangle;
        double max;
        double width=0;
        private Point anchorPoint;

        BitmapImage bitmap;
        public Edit(string v, Slider slider)
        {
           
            InitializeComponent();
         
             bitmap= new BitmapImage(new Uri(v));
            toEdit.Source = bitmap;
            var absolute_path = System.IO.Path.Combine(v);
            FileInfo file = new FileInfo(absolute_path);

            name.Content = file.Name;
            resolution.Content=bitmap.Width+" X "+bitmap.Height;
            
            type.Content = file.Extension;
                



           
             rectW = new Label();
            rectW.Background=new SolidColorBrush(Colors.White);

            toEdit.Width =bitmap.Width - (bitmap.Width * 0.15);
            toEdit.Height = bitmap.Height - (bitmap.Height * 0.15);
             canvas.Width= bitmap.Width - (bitmap.Width * 0.15);
            canvas.Height = bitmap.Height - (bitmap.Height * 0.15);


            rectangle = new Rectangle();
            rectangle.StrokeThickness = 10;
            rectangle.Fill = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            rectangle.Opacity = 0.5;
            rectangle.MouseMove += Rectangle_MouseMove;
            rectangle.Name = "toMove";
           

        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.RightButton==MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(rectangle, rectangle, DragDropEffects.Move);
            }
        }

        public Point location2 { get; private set; }
        public Point location { get; private set; }
        public Label rectW { get; private set; }
        public bool isScrollActive = false ;

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                canvas.CaptureMouse();
                location2 = e.MouseDevice.GetPosition(canvas);



                Canvas.SetLeft(rectangle, Mouse.GetPosition(canvas).X);
                Canvas.SetTop(rectangle, Mouse.GetPosition(canvas).Y);
                rectangle.Width = 0;

                rectangle.Height = 0;
                if (!canvas.Children.Contains(rectangle))
                {
                    canvas.Children.Add(rectangle);
                    canvas.Children.Add(rectW);

                }
            }
              
          

        }

       
        public static BitmapImage ToWpfImage( System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();  // no using here! BitmapImage will dispose the stream after loading
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage ix = new BitmapImage();
            ix.BeginInit();
            ix.CacheOption = BitmapCacheOption.OnLoad;
            ix.StreamSource = ms;
            ix.EndInit();
            return ix;
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
           
            if (!canvas.IsMouseCaptured)
                return;
             location = e.MouseDevice.GetPosition(canvas);
            int width= Math.Abs((int)(location.X - location2.X));
            int height = Math.Abs((int)(location.Y - location2.Y));
            if (width >0 && height > 0)
            {
                rectangle.Width = width;

                rectangle.Height = height;
            }
            rectW.Content = width.ToString()+" X " +height.ToString();
          

     


            Canvas.SetLeft(rectW, Mouse.GetPosition(canvas).X);
            Canvas.SetTop(rectW, Mouse.GetPosition(canvas).Y);
          



        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            canvas.ReleaseMouseCapture();
        }

        private void canvas_Drop(object sender, DragEventArgs e)
        {
            Point dropPos=e.GetPosition(canvas);    
            Canvas.SetLeft(rectangle, dropPos.X-(rectangle.Width/2));
            Canvas.SetTop(rectangle, dropPos.Y-(rectangle.Height/2));
        }
        public void ConvertCanvasToBitmap(Canvas canvas)
        {
            int x = (int)location2.X;
            int y = (int)location2.Y;
            int width =(int) rectangle.Width;
            int height =(int) rectangle.Height;
            canvas.Children.Remove(rectangle);
            canvas.Children.Remove(rectW);
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
         (int)canvas.RenderSize.Width,
    (int)canvas.RenderSize.Height,
            96d,
            96d,
            PixelFormats.Pbgra32);
            CroppedBitmap crop = null ;
            try
            {
                 crop = new CroppedBitmap(renderBitmap, new Int32Rect(x, y, width, height));
            }
            catch(System.ArgumentException error)
            {
                canvas.Children.Remove(rectangle);
                canvas.Children.Remove(rectW);
            }

           



            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);

           
            PngBitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create( crop));
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var stream = saveFileDialog.OpenFile())
                {
                    encoder.Save(stream);
                }

            }
               
            
           
        
    }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space) {
                ConvertCanvasToBitmap(canvas);
                MessageBox.Show("Saved" );
            }
            if(e.Key==Key.LeftCtrl) {
                if (!isScrollActive) { isScrollActive = true; } 
                else
                if(isScrollActive) 
                {
                    isScrollActive = false;
                }
            }

           
            
            if (e.Key == Key.Escape) this.Close();
        }

        private void canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(isScrollActive)
            {
                if (e.Delta > 0)
                {
                    toEdit.Width = toEdit.Width + 10;
                    toEdit.Height = toEdit.Height + 10;
                }


                else if (e.Delta < 0)
                {
                    toEdit.Width = toEdit.Width - 10;
                    toEdit.Height = toEdit.Height - 10;
                }
            }
          
        }

        private void canvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                isScrollActive = false;
            }
        }

    
        public System.Drawing.Bitmap ResizeBitmap(System.Drawing.Bitmap bmp, int width, int height)
        {
            System.Drawing.Bitmap result = new System.Drawing.Bitmap(width, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }

        private void flip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        
        }
       
    }
}

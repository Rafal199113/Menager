

using Menager.others;
using Microsoft.Win32;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.IO;
using System.Linq;


using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using System.Windows.Threading;


using Button = System.Windows.Controls.Button;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;

namespace Notatki
{
    public class Folders
    {
        Label FolderInfo;
       
        System.Windows.Controls.TextBox pathContent;
        WrapPanel Folder;
        
        System.Windows.Window window;
        Thread thread1;
        Button button;
    
       Icon set;
        Stack myStack;
        WrapPanel lastFolders;
        string sort;
        Label lab;
        WrapPanel lastFiles;
        Label label;
        WrapPanel panel;
        Dictionary<string, string> list = new Dictionary<string, string>();
        List<string> filestoDelete;
        Image heart;
        private string search = "";
        WrapPanel second;
        Grid gridLayout;
        ToolTip tool;
        Icon oneIcon;
        Image view;

        internal MenuContexts? contexts { get; private set; }

        Label info;
        List<Icon> foldery = new List<Icon>();
        Stack<string> foldersStack = new Stack<string>();
        Stack<string> filesStack = new Stack<string>();
         MediaPlayer mediaPlayer = new MediaPlayer();
    
       OpenWith openWith;
        ObservableCollection<Border> ObFolders = new ObservableCollection<Border>();
        ContextMenu context;
        CancellationTokenSource tss;
        public Folders(Label FolderInfo, Grid gridLayout, System.Windows.Controls.TextBox pathContent, ContextMenu context, string sort, Window systemInfo,  System.Windows.Controls.Label info,Icon set,WrapPanel bookMarksPanel, CancellationTokenSource? tss, MenuContexts con )
        {
          
            myStack = new Stack();
            filestoDelete = new List<string>();
            this.context = context;
            this.Sort = sort;
            this.info = info;
            this.window = systemInfo;
            this.view = view;
            this.contexts = con;
            this.FolderInfo = FolderInfo;
            this.pathContent = pathContent;
          
            this.set = set;
            this.gridLayout = gridLayout;
            var json2 = File.ReadAllText(@"data/extensions.txt", Encoding.UTF8);
            this.bookMarksPanel = bookMarksPanel;
            this.tss = tss;
            this.foldery = foldery;
        }

      

        public Label FolderInfo1 { get => FolderInfo; set => FolderInfo = value; }
   
        public WrapPanel Folder1 { get => Folder; set => Folder = value; }
        public Stack MyStack { get => myStack; set => myStack = value; }
        public List<string> FilestoDelete { get => filestoDelete; set => filestoDelete = value; }
        public Dictionary<string, string> List { get => list; set => list = value; }
        public string Search { get => search; set => search = value; }
        public WrapPanel LastFolders { get => lastFolders; set => lastFolders = value; }
        public WrapPanel LastFiles { get => lastFiles; set => lastFiles = value; }
        public string Sort { get => sort; set => sort = value; }
        public List<Icon> Foldery { get => foldery; set => foldery = value; }
        public Thread Thread1 { get => thread1; set => thread1 = value; }
        public ObservableCollection<Border> ObFolders1 { get => ObFolders; set => ObFolders = value; }
        public CancellationTokenSource Tss { get => tss; set => tss = value; }
        public RoutedEvent LoadedEvent { get; private set; }

        Task t = new Task(() => {
        
        
        });
        functions functions = new functions();
        private WrapPanel bookMarksPanel;
  
        public async Task getFolder(string pathh ,   CancellationTokenSource can)
        {
            this.Tss = can;
         var   token = tss.Token;


           

          
            gridLayout.Children.Clear();
            pathContent.Text = pathh;
         
            
            TextBox child = new TextBox();
            WrapPanel childPanel;
         
            RowDefinition row;
            var lista = Directory.GetFiles(pathh).ToList();
            List<FileInfo> filesInfo = new List<FileInfo>();
            Border border = new Border();
            var folderslista = Directory.GetDirectories(pathh).ToList();
            List<DirectoryInfo> infoofolders = new List<DirectoryInfo>();
            int coutFolderContent = 0;
          
            foreach (string x in folderslista)
            {
                infoofolders.Add(new DirectoryInfo(x));
            }
            foreach (string x in lista)
            {
                filesInfo.Add(new FileInfo(x));
            }
            int countFiles = 0;
            int allFiles = lista.Count() + folderslista.Count();
            int GridRow = 0;
            int columnCounter = 0;


            await Task.Run(() =>
            {
                
                thread1 = new Thread(async () =>
                {

                    //Sortowanie plików
                    switch (Sort)
                    {
                        case "namee":
                            infoofolders.Sort((x, y) => x.Name.CompareTo(y.Name));
                            filesInfo.Sort((x, y) => x.Name.CompareTo(y.Name));
                            break;
                        case "date":
                            infoofolders.Sort((x, y) => x.CreationTime.CompareTo(y.CreationTime));
                            filesInfo.Sort((x, y) => x.CreationTime.CompareTo(y.CreationTime));
                            filesInfo.Reverse();
                            break;
                       

                            break;

                    }

                    //Ładowanie folderów
                    await functions.LoadImages.LoadFolder("assets/Folder.png", 290, (x, path) =>
                    {
                       
                      
                   
                        foreach (DirectoryInfo dirInfo in infoofolders)
                        {
                            countFiles++;
                            if (can.IsCancellationRequested)
                            {
                             //   MessageBox.Show(" Zadanie anulowane.");
                                System.Diagnostics.Debug.WriteLine(" Zadanie anulowane.");
                                break;

                            }
                            coutFolderContent++;
                            System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                            {


                              

                                border = new Border();
                                border.Style = window.FindResource("iconBorder") as Style;
                                panel = new WrapPanel();
                                panel.Style= window.FindResource("IconPanel") as Style;

                              
                                Image image = new Image();
                                image.Style = window.FindResource("IconImage") as Style;
                                image.Source = x;
                                image.MouseLeftButtonDown+= (sender, e) => Templattte_Click(sender, e, pathContent.Text, dirInfo.FullName, can,filestoDelete);
                                image.MouseDown += (sender,e)=>Image_MouseDown(sender,e,dirInfo.FullName);
                                image.ContextMenu = contexts.FileContext(dirInfo.FullName);

                                TextBox iconName = new TextBox();
                                iconName.Text= dirInfo.FullName.Split("\\")[dirInfo.FullName.Split("\\").Count() - 1];
                                iconName.IsReadOnly = true;
                                iconName.Style = window.FindResource("IconTextBox") as Style;
                                iconName.MouseDoubleClick +=(sender,e)=> IconName_MouseDoubleClick(sender,e,dirInfo.FullName,pathContent);
                                
                                panel.Children.Add(image);
                                panel.Children.Add(iconName);





                                border.Child = panel;

                               



                             

                                System.Windows.Controls.Grid.SetRow(border, GridRow);
                                System.Windows.Controls.Grid.SetColumn(border, columnCounter);

                                columnCounter++;
                                if (columnCounter % 3 == 0)
                                {
                                    columnCounter = 0;

                                    gridLayout.RowDefinitions.Add(new RowDefinition());


                                    GridRow++;
                                }
                               
                                gridLayout.Children.Add(border);
                                info.Content = "";
                            });

                            System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                            {

                            });
                        }



                    });
                    coutFolderContent = 0;
                    //Ładowanie plików
                    foreach (FileInfo file in filesInfo)
                    {

                        countFiles++;
                       
                        coutFolderContent++;
                      
                        await System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                       {
                              info.Content = "Pliki: " + coutFolderContent + " /" + filesInfo.Count;

                          
                            
                           
                          
                           
                              
                          




                              




                          });

                        if (file.Extension == ".jpg" || file.Extension == ".png" || file.Extension == ".jfif" || file.Extension == ".jpeg")
                        {
                            if (can.IsCancellationRequested)
                            {
                                //   MessageBox.Show(" Zadanie anulowane.");
                                System.Diagnostics.Debug.WriteLine(" Zadanie anulowane.");
                                break;

                            }else
                            {
                                await functions.LoadImages.LoadFolder(file.FullName, 100, (x, path) =>
                                {

                                    System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                                    {

                                        border = new Border();
                                        border.Style = window.FindResource("iconBorder") as Style;
                                        panel = new WrapPanel();
                                        panel.Style = window.FindResource("IconPanel") as Style;

                                     

                                        Image image = new Image();
                                        image.Style = window.FindResource("IconImage") as Style;
                                        image.Source = x;
                                        image.MouseLeftButtonDown += (sender, e) => Templattte_Click1(sender, e, file.FullName, pathContent);
                                        image.MouseDown += (sender, e) => Image_MouseDown(sender, e, file.FullName);
                                        image.ContextMenu = contexts.FileContext(file.FullName);
                                        image.MouseLeftButtonDown += (sender, e) => Image_MouseLeftButtonDown(sender, e, file.FullName);

                                        tool = new ToolTip();
                                       
                                        tool.DataContext = new img() { Path = path,Size=x.Width+"X"+x.Height,Type=file.Extension };
                                        image.ToolTip = tool;

                                        TextBox iconName = new TextBox();
                                        iconName.Text = file.FullName.Split("\\")[file.FullName.Split("\\").Count() - 1];
                                        iconName.Style = window.FindResource("IconTextBox") as Style;
                                        iconName.IsReadOnly = true;
                                        iconName.MouseDoubleClick += (sender, e) => IconName_MouseDoubleClick(sender, e, file.FullName, pathContent);
                                        panel.Children.Add(image);
                                        panel.Children.Add(iconName);




                                  












                                        border.Child = panel;


                                        System.Windows.Controls.Grid.SetRow(border, GridRow);
                                        System.Windows.Controls.Grid.SetColumn(border, columnCounter);
                                        columnCounter++;
                                        if (columnCounter % 3 == 0)
                                        {
                                            columnCounter = 0;

                                            gridLayout.RowDefinitions.Add(new RowDefinition());


                                            GridRow++;
                                        }



                                        gridLayout.Children.Add(border);

                                    });















                                });

                            }

                   
                          

                        }
                        else
                        {



                            System.Drawing.Icon iconn = System.Drawing.Icon.ExtractAssociatedIcon(file.FullName);
                            System.Drawing.Bitmap bmp = iconn.ToBitmap();

                            await System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                               {

                                   border = new Border();
                                   border.Style = window.FindResource("iconBorder") as Style;
                                   panel = new WrapPanel();
                                   panel.Style = window.FindResource("IconPanel") as Style;


                                   Image image = new Image();
                                   image.Style = window.FindResource("IconImage") as Style;
                                   image.Source = functions.ImageSourceForBitmap(bmp);
                                 
                                   image.MouseDown += (sender, e) => Image_MouseDown(sender, e, file.FullName);
                                   image.ContextMenu = contexts.FileContext(file.FullName);
                                   image.MouseLeftButtonDown += (sender, e) => Image_MouseLeftButtonDown(sender, e, file.FullName);

                                   TextBox iconName = new TextBox();
                                   iconName.Text = file.FullName.Split("\\")[file.FullName.Split("\\").Count() - 1];
                                   iconName.Style = window.FindResource("IconTextBox") as Style;
                                
                                   iconName.MouseDoubleClick += (sender, e) => IconName_MouseDoubleClick(sender, e, file.FullName, pathContent);
                                   panel.Children.Add(image);
                                   panel.Children.Add(iconName);


                              
                                   border.Child = panel;
                                   System.Windows.Controls.Grid.SetRow(border, GridRow);
                                   System.Windows.Controls.Grid.SetColumn(border, columnCounter);
                                   columnCounter++;
                                   if (columnCounter % 3 == 0)
                                   {
                                       columnCounter = 0;

                                       gridLayout.RowDefinitions.Add(new RowDefinition());


                                       GridRow++;
                                   }
                                   gridLayout.Children.Add(border);


                                   info.Content = "";
                               });
                        }
                        await System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                        {
                            info.Content = "Pliki: " + countFiles + " /" + allFiles ;

                        });
                    }

                })
                {

                };
                thread1.SetApartmentState(ApartmentState.STA); ;
                thread1.Start();
                thread1.Join();


            },token); ;
            
            



      
            void Lab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e, string pathh, System.Windows.Window window,List<string>filestoDelete)

            {
                 
                gridLayout.Children.Clear();
                if (Directory.Exists(pathh))
                {
                    getFolder(pathh,can);
                }
                else
                {
                    lastFolders.Children.Remove((Label)sender);
                }

            }
           
        }

       

 
        private void Image_MouseEnter(object sender, MouseEventArgs e, BitmapImage x)
        {
            info.Content="Resolution:"+x.Width + "/" + x.Height;
        }

        public void IconName_MouseDoubleClick(object sender, MouseButtonEventArgs e,string oldPath,TextBox pathContent)
        {
            TextBox textBox = sender as TextBox;
            textBox.IsReadOnly = false;
            textBox.Text = string.Empty;
            textBox.Focus();
            if (Directory.Exists(oldPath))
            {
                textBox.KeyDown += (sender, e) => TextBox_KeyDown(sender, e, textBox.Text, oldPath, pathContent);
            }
            else
            {
                textBox.KeyDown += (sender, e) => RenameFile(sender, e, textBox.Text, oldPath, pathContent);
            }
          
            
        }

        private async void TextBox_KeyDown(object sender, KeyEventArgs e,string newPath,string oldPath, TextBox pathContent)
        {
            TextBox? textBox = sender as TextBox;
            if (e.Key == Key.Enter)
            {
              await  Task.Run( () =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                    {
                        Directory.Move(oldPath, pathContent.Text + "\\" + newPath);
                    });

                });
             
                textBox.IsReadOnly = true;
            }
        }
        private async void RenameFile(object sender, KeyEventArgs e, string newPath, string oldPath, TextBox pathContent)
        {
            TextBox? textBox = sender as TextBox;
            if (e.Key == Key.Enter)
            {
                FileInfo file=new FileInfo (oldPath);
                await Task.Run( () =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                    {
                        File.Move(oldPath, pathContent.Text + "\\" + newPath+file.Extension);
                    });

                });

                textBox.IsReadOnly = true;
            }
        }

        public void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e,string path)
        {
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

        public void Image_MouseDown(object sender, MouseButtonEventArgs e, string path)
        {



            if (e.ChangedButton == MouseButton.Middle)
            {
                Image button = sender as Image;
                WrapPanel wrapPanel = button.Parent as WrapPanel;

                Border b = wrapPanel.Parent as Border;
                ;
                if (b.BorderBrush.ToString() == new SolidColorBrush(Colors.Green).ToString())
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString(set.borderColor);
                    b.BorderBrush = brush;
                    filestoDelete.Remove(path);
                  

                 
                }
                else
                {
                 
                    filestoDelete.Add(path);
                    b.BorderBrush = new SolidColorBrush(Colors.Green);
                 
                   
                }
            }
        }

        private void Panel_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e,string path,int counter)
        {


           
            if (e.ChangedButton == MouseButton.Left)
            {
                counter++;
                if (counter == 2)
                {
                    if (new FileInfo(path).Extension.Split(".")[1] == "exe")
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                    else
                    {
                       openWith = new OpenWith(path, pathContent);
openWith.Show();
                    }

                    counter = 0;
                }
                MessageBox.Show(counter.ToString());
                WrapPanel button = sender as WrapPanel;

                Border b = button.Parent as Border;
                ;
                if (b.BorderBrush.ToString() == new SolidColorBrush(Colors.Green).ToString())
                {

                    b.BorderBrush = new SolidColorBrush(Color.FromRgb(105, 105, 105));
                    filestoDelete.Remove(path);
                    list.Remove(path);


                }
                else
                {
                    filestoDelete.Add(path);
                    b.BorderBrush = new SolidColorBrush(Colors.Green);
                    try
                    {
                        list.Add(path, path);
                    }
                    catch (System.ArgumentException error)
                    {
                        list.Remove(path);
                        list.Add(path, path);
                    }

                }
            }
            
        }

        

      
      

        private void Panel_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e, string path, TextBox pathContent)
        {

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

        public void Templattte_Click1(object sender, RoutedEventArgs e,string path,TextBox pathContent)
        {
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

        public async Task Templattte_Click(object setnder, RoutedEventArgs e, string path, string folderPath, CancellationTokenSource tsd, List<string> filestoDelete)
        {
            
            gridLayout.Children.Clear();
            pathContent.Text = folderPath;
            myStack.Push(path); 
            this.Tss.Cancel();
            this.Tss.Dispose();
            this.Tss = new CancellationTokenSource();
        
            getFolder(folderPath, Tss);
          
          
   }

       

       

    
       
       
      

     
        public void Label_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(@"Paths.txt", json);
            MessageBox.Show("Pomyślnie dodano");
           
        }
      public bool cancel()
        {
            if (Tss.IsCancellationRequested)
            {
               
            }
            else
            {

                tss.Cancel();
                tss.Dispose();
                tss = new CancellationTokenSource();



            }
            return Tss.IsCancellationRequested;
        }
      
    }
    public class img
    {
        private string path="";
        private string size="";
        private string type="";



        public string Path { get => path; set => path = value; }
        public string Size { get => size; set => size = value; }
        public string Type { get => type; set => type = value; }
    }
}

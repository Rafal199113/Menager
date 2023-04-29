using Newtonsoft.Json;
using Notatki;
using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Menager.others
{
  public  class MenuContexts

    {
   
        private ContextMenu FileContextMenu;
       

        public List<List<string>> fouvourite { get; private set; }
     
        TextBox pathContent;

        public MainWindow window { get; private set; }
        public Icon set { get; private set; }
        public Notifications notification { get; private set; }
        public Folders folder { get;  set; }

        MenuItem favourite;
       
        public MenuItem remove { get; private set; }
        public MenuItem delete { get; private set; }
        public MenuItem path { get; private set; }
        public string name { get; private set; }
        public MenuItem slide { get; private set; }
        public MenuItem size { get; private set; }
        public MenuItem renameFewFiles { get; private set; }
        public MenuItem background { get; private set; }
        public CancellationTokenSource ts { get; private set; }

        public  MenuContexts(TextBox pathContent, MainWindow mainWindow, Icon set, Notifications notifications, CancellationTokenSource ts) {
            string last = File.ReadAllText(@"data/favourite.txt", Encoding.UTF8);
            fouvourite = JsonConvert.DeserializeObject<List<List<string>>>(last);
            this.pathContent=pathContent;
            this.window = mainWindow;
            this.set = set;
            this.notification = notifications;
           
            createContextItems();
            FileContextMenu = new ContextMenu();
            FileContextMenu.Items.Add(favourite);
            FileContextMenu.Items.Add(remove);
            FileContextMenu.Items.Add(delete);
            FileContextMenu.Items.Add(path);
            FileContextMenu.Items.Add(slide);
            FileContextMenu.Items.Add(size);
            FileContextMenu.Items.Add(renameFewFiles);
            
            



        }

        public ContextMenu FileContext(string fullName)
        {
       
             
         
            return FileContextMenu;
        }
       
        private void createContextItems()
        {

             favourite = new MenuItem();
            favourite.Header = "Add to Favourite";
            favourite.Click += Favourite_Click;
             remove = new MenuItem();
            remove.Header = "Remove from Favourite";
            remove.Click += Remove_Click;
             delete = new MenuItem();
            delete.Header = "Delete";
            delete.Click += Delete_Click;
             path = new MenuItem();
            
            path.Header = "Copy path...";
            path.Click += Path_Click;

             slide = new MenuItem();
            slide.Header = "Slider";
            slide.Click += (sender, e) => {


                MenuItem mnu = sender as MenuItem;
                Image sp = null;

                if (mnu != null)
                {
                    sp = ((ContextMenu)mnu.Parent).PlacementTarget as System.Windows.Controls.Image;



                }
                WrapPanel parent = sp.Parent as WrapPanel;
                TextBox text = parent.Children[1] as TextBox;
               
                if (pathContent.Text!="Collections")
                {
                    new Slider(pathContent.Text, window, text.Text).Show();
                }
                   
            };
             size = new MenuItem();
            size.Header = "Size";
            size.Click += (sender, e) =>
            {

                MenuItem mnu = sender as MenuItem;
                Image sp = null;

                if (mnu != null)
                {
                    sp = ((ContextMenu)mnu.Parent).PlacementTarget as System.Windows.Controls.Image;



                }
                WrapPanel parent = sp.Parent as WrapPanel;
                TextBox text = parent.Children[1] as TextBox;
                if (pathContent.Text == "Collections")
                {

                    if (!Directory.Exists(  text.Text))
                    {
                        int size = (int)new FileInfo(text.Text).Length;
                        MessageBox.Show(((size / 1024) / 1024).ToString() + "MB");

                    }
                    else
                    {
                        new FolderSpace( text.Text, set, folder).Show();
                    }
                }
                else
                {
                    if (!Directory.Exists(pathContent.Text + "\\" + text.Text))
                    {
                        int size = (int)new FileInfo(pathContent.Text + "\\" + text.Text).Length;
                        MessageBox.Show(((size / 1024) / 1024).ToString() + "MB");
                    }
                    else
                    {
                        new FolderSpace(pathContent.Text + "\\" + text.Text, set, folder).Show();
                    }
                }
               


            };
             renameFewFiles = new MenuItem();
            renameFewFiles.Header = "Zmień nazwe ";
            renameFewFiles.Click += Rename_Click;
             background = new MenuItem();
            background.Header = "Set as Desktop";
            background.Click += Background_Click;

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
            string path = pathContent.Text + "\\" + text.Text;
            if (Directory.Exists(path))
            {
                fouvourite[0].Remove(path);

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

                    string x = pathContent.Text + "\\" + text.Text;

                    string isCollection = x.Split("\\")[0];
                    if (isCollection == "Collections")
                    {
                        x=text.Text;
                        fouvourite[2].Remove(x);
                    }
                    else
                    {
                        fouvourite[2].Remove(x);
                    }
                   
                    
                }
              
            }
            string json = JsonConvert.SerializeObject(fouvourite, Formatting.Indented);


            File.WriteAllText(@"data/favourite.txt", json);

        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            Image sp = null;

            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as System.Windows.Controls.Image; ;



            }
            WrapPanel panel = sp.Parent as WrapPanel;

            TextBox text = panel.Children[1] as TextBox;
            string path = pathContent.Text + "\\" + text.Text;
            if (MessageBox.Show("Are you sure you want to delete this file?",
                                     text.Text,
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        try
                        {
                            Directory.Delete(path);
                            notification.show("Folder had deleted succesfully...", ts);
                            folder.Tss.Cancel();
                            folder.Tss.Dispose();
                            ts = new CancellationTokenSource();
                            folder.getFolder(pathContent.Text,ts);

                        }catch (Exception ex)
                        {
                            notification.show("This folder was not possible to delete...", ts);
                        }
                       
                       
                    }
                    else
                    {
                        System.IO.File.Delete(path);
                        folder.Tss.Cancel();
                        folder.Tss.Dispose();
                        ts = new CancellationTokenSource();
                        await folder.getFolder(pathContent.Text, ts);
                    }

                }
                catch (System.IO.IOException ex)
                {

                    ;
                }


            }

        }
        private void Path_Click(object sender, RoutedEventArgs e)
        {

            MenuItem mnu = sender as MenuItem;
            Image sp = null;

            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as System.Windows.Controls.Image; ;



            }
            WrapPanel panel = sp.Parent as WrapPanel;

            TextBox text = panel.Children[1] as TextBox;
            Clipboard.SetText(pathContent.Text + "\\" + text.Text);
        }
        private void Rename_Click(object sender, RoutedEventArgs e)
        {

            MenuItem mnu = sender as MenuItem;
            System.Windows.Controls.Image sp = null;

            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as System.Windows.Controls.Image;
                WrapPanel panel = sp.Parent as WrapPanel; ;
                TextBox text = panel.Children[1] as TextBox;
                text.IsReadOnly = false;

            }
        }
        private void Background_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            Image sp = null;

            if (mnu != null)
            {
                sp = ((ContextMenu)mnu.Parent).PlacementTarget as System.Windows.Controls.Image; ;



            }
            WrapPanel panel = sp.Parent as WrapPanel;

            TextBox text = panel.Children[1] as TextBox;
            FileInfo file = new FileInfo(pathContent.Text + "\\" + text.Text);
            if (file.Extension == ".jpg" || file.Extension == ".png" || file.Extension == ".jfif" || file.Extension == ".jpeg")
            {
                new DesktopBackgound(pathContent.Text + "\\" + text.Text, window).Show();
            }

        }

    }
}

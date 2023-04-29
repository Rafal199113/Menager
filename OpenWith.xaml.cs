using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Notatki
{
    /// <summary>
    /// Logika interakcji dla klasy OpenWith.xaml
    /// </summary>
    public partial class OpenWith : Window
    {
        Dictionary<string , List<string>> programs=new Dictionary<string, List<string>>();
        Label oneProgram;
        string path;
        
        string ext;
        TextBox contentPath;
        functions functions = new functions();
        public OpenWith(string path, TextBox contentPath )
            {
            this.contentPath = contentPath;
            this.path = path;
            try
            {
                string date = File.ReadAllText(@"data/cycki.txt", Encoding.UTF8);
                programs = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(date);
            }
            catch (System.IO.FileNotFoundException e) { }
            
           


            
            InitializeComponent();
             ext = new FileInfo(path).Extension.Split(".")[1];
            type.Content = ext;

            update(ext);
      


        }
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        { 
           
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Exe Files (.exe)|*.exe";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        filePath = openFileDialog.FileName;
                        programs[type.Content.ToString()].Add(filePath);

                        string json = JsonConvert.SerializeObject(programs, Formatting.Indented);


                        File.WriteAllText(@"data/cycki.txt", json);
                        loadPrograms.Children.Clear();
                        update(type.Content.ToString());

                        var fileStream = openFileDialog.OpenFile();

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContent = reader.ReadToEnd();
                        }
                    }
                    catch(System.Collections.Generic.KeyNotFoundException error)
                    {

                        programs.Add(ext, new List<string> { filePath });
                        string json = JsonConvert.SerializeObject(programs, Formatting.Indented);


                        File.WriteAllText(@"cycki.txt", json);
                        loadPrograms.Children.Clear();
                        update(ext);
                    }
                
                }
            }
        }
        void update(string ext)
        {
            foreach (string x in programs.Keys)
            {
                if (x == ext)
                {

                    foreach (string xz in programs[x])
                    {
                        int count = xz.Split("\\").Count();
                        oneProgram = new Label();
                        oneProgram.Template = FindResource("lab") as ControlTemplate;
                        string cycki = string.Empty;
                        if (contentPath.Text == "Collections")
                        {
                            var cipka = path.Split("\\");
                            var array = cipka.SkipLast(1).ToArray();
                          
                            foreach (var c in array) { cycki += c; }
                        }
                        else cycki = contentPath.Text; 
                        
                     
                  
                        oneProgram.Style = FindResource("load") as Style;
                        oneProgram.Content = xz.Split("\\")[count-1];
                        oneProgram.MouseLeftButtonDown+= (e, sender) => OneProgram_MouseLeftButtonDown(e, sender, path,cycki,xz );
                        loadPrograms.Children.Add(oneProgram);
                    }
                    
                }
                if(!programs.Keys.Contains(ext))
                {
                  
                    oneProgram = new Label();
                    oneProgram.Template = FindResource("lab") as ControlTemplate;
                    oneProgram.Style = FindResource("load") as Style;
                    oneProgram.Content = "Brak wyników...";
                
                     loadPrograms.Children.Add(oneProgram);
                    break;
                }
            }
        }

        private void OneProgram_MouseLeftButtonDown(object sender, MouseButtonEventArgs e, string path, string work,string program)
        {
            try
            {
                ProcessStartInfo info = functions.newProcess(path, work, program);

                Process.Start(info);

            }
            catch(System.ComponentModel.Win32Exception error)
            {

              
                List<string> lista = programs[new FileInfo(path).Extension.Split(".")[1]];
                lista.Remove(program);

              
                string json = JsonConvert.SerializeObject(programs, Formatting.Indented);


                File.WriteAllText(@"data/cycki.txt", json);
                this.Close();
            }
             
           
               
               
                this.Close();
            
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}

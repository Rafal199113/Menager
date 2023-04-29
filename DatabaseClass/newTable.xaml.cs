using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Management;
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

namespace Menager.DatabaseClass
{
    /// <summary>
    /// Interaction logic for newTable.xaml
    /// </summary>
    public partial class newTable : Window
    {
        private string  DataType;
        WrapPanel WrapPanel;
        WrapPanel panel;
        TextBox Namelabel;
        TextBox typeLabel;
        Button delete;
        serverConnect server;
        ObservableCollection<KeyValuePair<string,string>> fieldss;
        public newTable(string v)
        {
            InitializeComponent();
            fieldss=new ObservableCollection<KeyValuePair<string, string>>();
            server = new serverConnect();
            databaseName.Content = "Database: "+v;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (columnName.Text != "" && tableName.Text != "")
            {
                if (DataType == "bool")
                {
                    DataType = "int";
                }
                KeyValuePair<string, string> key = new KeyValuePair<string, string>(columnName.Text, DataType);
                fieldss.Add(key);

                addToFields(key);

            }

        }

        private void addToFields(KeyValuePair<string, string> key)
        {
           WrapPanel = new WrapPanel();
         delete=new Button();
            delete.Content = "Delete";
            delete.Width = 100;
            delete.Click += (sender, e) =>
            {
              
                fieldss.Remove(key);
              for(int i = 0; i < fields.Children.Count; i++) 
                {
                    WrapPanel panel = fields.Children[i] as WrapPanel;

                   if(((TextBox)panel.Children[0]).Text == key.Key)
                    {
                        fields.Children.Remove(fields.Children[i]);
                    }
                }

            };

            WrapPanel.Orientation = Orientation.Horizontal;
            Namelabel=new TextBox();
          
            Namelabel.Text= key.Key;
            typeLabel= new TextBox();
          
            typeLabel.Text= key.Value;
            WrapPanel.Children.Add(delete);
            WrapPanel.Children.Add(Namelabel);  
            WrapPanel.Children.Add(typeLabel);
            

            fields.Children.Add(WrapPanel);
        }

        

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton ck = sender as RadioButton;
            if (ck.IsChecked.Value)
                DataType = ck.Content.ToString();
                
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            StringBuilder builder = new StringBuilder();
            string query="CREATE TABLE "+tableName.Text+"(";
            foreach(KeyValuePair<string, string> kvp in fieldss)
            {
              query=  query + kvp.Key + " " + kvp.Value + ",";

            }
           query= query.Remove(query.Length-1);
            query += ");";
            MessageBox.Show(query);
            server.connect();
            server.createTable(query);
        }
    }
}

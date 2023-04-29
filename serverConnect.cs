using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Menager
{
    internal class serverConnect
    {
       string connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private SqlConnection cnn;
        public void connect()
        {
            SqlDataReader dr;

            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
           
            if (cnn.State == ConnectionState.Open)
            {
             
            }



        }
        public List<String> getServers()
        {
            string sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES";
            using var cmd = new SqlCommand(sql, cnn);
            Console.WriteLine("Count was succesfull \n");
            List<string> lista = new List<string>();

            using SqlDataReader rdr = cmd.ExecuteReader();
            int i = 0;
            while (rdr.Read())
            {
                while (i < rdr.FieldCount)
                {
                    lista.Add(rdr.GetString(i++));
                  
                }



            }
            return lista;
        }
        public void disconnect() {
            cnn.Close();
        
        }

        internal void createTable(string query)
        {
          

            using (SqlCommand command = new SqlCommand(query, cnn))
                command.ExecuteNonQuery();
        }
    }
}

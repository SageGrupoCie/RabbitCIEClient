using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace RabbitCIEClient
{
    class BaseDatos
    {
        private static SqlConnection conexion;
        private static bool conectado;
        public BaseDatos(string servidor, string database, string user, string pass)
        {
            ConectarBD(servidor,database,user,pass);
        }
        public static SqlConnection getConexion()
        {
            return conexion;
        }
        public bool estaConectado() { return conectado; }
        public void desConectarBD() { conexion.Close(); }
        public static void ConectarBD(string servidor, string database, string user, string pass)
        {
            conexion = new SqlConnection();
            conexion.ConnectionString =
              "Data Source=" + servidor + ";" +
              "Initial Catalog=" + database + ";" +
              "User id=" + user + ";" +
              "Password=" + pass + ";";
            try
            {
                conexion.Open();
                conectado = true;
            }
            catch(Exception ex) 
            {
                conectado = false;
            }
            /*
            MessageBox.Show("Se abrió la conexión con el servidor SQL Server correctamente");
            String sql = "Select Empresa FROM Empresas";
            using (SqlCommand command = new SqlCommand(sql, conexion))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MessageBox.Show("Empresa: " + reader.GetString(0));
                    }
                }
            }
            */
        }

        public void InsertarDatos(List<String> lista)
        {

            String sql = "INSERT INTO CieTmpClientesIGEO values(";


            for (int i = 0; i < lista.Count; i++)
            {
                if (i == lista.Count - 1)
                {
                    sql += lista[i].ToString() + ")";
                }
                else
                {
                    sql += lista[i].ToString() + ",";
                }
            }

            if (conectado)
            {

                using (var connection = getConexion())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Registro realizado");
                    }
                }
            }

            MessageBox.Show(sql);


        }
    }
}

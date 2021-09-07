﻿using System;
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
        public BaseDatos(string servidor, string database, string user, string pass)
        {
            ConectarBD(servidor,database,user,pass);
        }
        public static SqlConnection getConexion()
        {
            return conexion;
        }
        public bool estaConectado() { return (conexion.State == System.Data.ConnectionState.Open); }
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

            }
            catch(Exception ex) 
            {
                
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

        public void InsertarDatos(List<String> lista, string indicesNumericos="", string nombreTabla="")
        {

            String sql = "INSERT INTO " + nombreTabla + " values(";
            string valor;


            for (int i = 0; i < lista.Count; i++)
            {
                bool esIndNumerico = false;
                if (indicesNumericos != "")
                {
                    string axInd = "," + i.ToString() + ",";
                    if (indicesNumericos.Contains(axInd)) { esIndNumerico = true; }
                }
                if (lista[i] == null)
                {
                    valor = "";
                }
                else
                {
                    valor = lista[i].ToString();
                }
                if (i == lista.Count - 1)
                {
                    if (esIndNumerico)
                    {
                        if (valor == "") { valor = "null"; }
                        sql += valor + ")";
                    }
                    else
                    {
                        sql += "'" + valor + "'" + ")";
                    }
                }
                else
                {
                    if(esIndNumerico)
                    {
                        if (valor == "") { valor = "null"; }
                        sql += valor + ",";
                    }
                    else
                    {
                        sql += "'" + valor + "'" + ",";
                    }
                }
            }
            
            if (estaConectado())
            {

                using (var connection = getConexion())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                            MessageBox.Show("Registro realizado");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Registro NO realizado: " + ex.Message);
                        }
                    }
                }
            }
            

            MessageBox.Show(sql);


        }
    }
}

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
                MessageBox.Show(ex.Message);
                
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

        public void eliminarDatosTabla(string nombreTabla, string whereDelete)
        {
            String sql = "DELETE from " + nombreTabla + " " + whereDelete;
            if (estaConectado())
            {

                using (var connection = getConexion())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }
            }
        }

        public bool InsertarDatos(List<String> lista, Logs lg, string esPRevio, string indicesNumericos = "", string nombreTabla = "", string indicesBool = "", string indicesDate = "")
        {

            String sql = "INSERT INTO " + nombreTabla + " values(";
            string valor;

            try
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    bool esIndNumerico = false;
                    if (indicesNumericos != "")
                    {
                        string axInd = "," + i.ToString() + ",";
                        if (indicesNumericos.Contains(axInd)) { esIndNumerico = true; }
                    }
                    bool esIndDate = false;
                    if (indicesDate != "")
                    {
                        string axInd = "," + i.ToString() + ",";
                        if (indicesDate.Contains(axInd)) { esIndDate = true; }
                    }
                    bool esIndBool = false;
                    if (indicesBool != "")
                    {
                        string axInd = "," + i.ToString() + ",";
                        if (indicesBool.Contains(axInd)) { esIndBool = true; }
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
                        else if (esIndDate)
                        {
                            if (valor == "") { valor = "null"; }
                            string valFech = "";
                            try
                            {
                                DateTime axDM = DateTime.Parse(valor);
                                valFech = axDM.ToString("yyyy-MM-dd h:m:s");
                            }
                            catch
                            {
                                valFech = "null";
                            }
                            if (valFech == "null") { sql += valFech + ")"; }
                            else { sql += "'" + valFech + "')"; }
                        }
                        else if (esIndBool)
                        {
                            if (valor == "") { valor = "null"; }
                            string valFech = "";
                            try
                            {
                                bool axBL = bool.Parse(valor);
                                valFech = "0";
                                if (axBL) { valFech = "-1"; }
                            }
                            catch
                            {
                                valFech = "null";
                            }
                            sql += valFech + ")";
                        }
                        else
                        {
                            sql += "'" + valor.Replace("'", " ") + "'" + ")";
                        }
                    }
                    else
                    {
                        if (esIndNumerico)
                        {
                            if (valor == "") { valor = "null"; }
                            sql += valor.Replace("'", " ") + ",";
                        }
                        else if (esIndDate)
                        {
                            if (valor == "") { valor = "null"; }
                            string valFech = "";
                            try
                            {
                                DateTime axDM = DateTime.Parse(valor);
                                valFech = axDM.ToString("yyyy-MM-dd h:m:s");
                            }
                            catch
                            {
                                valFech = "null";
                            }
                            if (valFech == "null") { sql += valFech + ","; }
                            else { sql += "'" + valFech + "',"; }
                        }
                        else if (esIndBool)
                        {
                            if (valor == "") { valor = "null"; }
                            string valFech = "";
                            try
                            {
                                bool axBL = bool.Parse(valor);
                                valFech = "0";
                                if (axBL) { valFech = "-1"; }
                            }
                            catch
                            {
                                valFech = "null";
                            }
                            sql += valFech + ",";
                        }
                        else
                        {
                            sql += "'" + valor.Replace("'", " ") + "'" + ",";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //andreu
                //MessageBox.Show("Registro NO realizado: " + ex.Message);
                string tipolist = "erroresProcesado";
                if (esPRevio != "") { tipolist = "erroresPreviosProcesado"; }
                lg.addError(tipolist, "\r\n" + "Error al insertar los datos en la tabla: " + ex.Message + "\r\n" + sql + "\r\n");
                return false;
            }
            //andreu
            //MessageBox.Show(sql);
            if (estaConectado())
            {

                using (var connection = getConexion())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                            //MessageBox.Show("Registro realizado");
                        }
                        catch (Exception ex)
                        {
                            //andreu
                            //MessageBox.Show("Registro NO realizado: " + ex.Message);
                            string tipolist = "erroresProcesado";
                            if (esPRevio != "") { tipolist = "erroresPreviosProcesado"; }
                            lg.addError(tipolist, "\r\n" + "Error al insertar los datos en la tabla: " + ex.Message + "\r\n" + sql + "\r\n");
                            return false;
                        }
                    }
                }
            }
            return true;

            //MessageBox.Show(sql);


        }
    }
}

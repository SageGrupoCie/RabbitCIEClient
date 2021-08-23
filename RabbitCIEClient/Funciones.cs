using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RabbitCIEClient
{
    class Funciones
    {
        public static string obtenerValoresIni(string cadena)
        {
            string linea;
            string nombreFilePath = @"C:\COMPARTIDA\ConfigCIERabbit.ini";
            string resultado = "";
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(nombreFilePath);
                while ((linea = file.ReadLine()) != null)
                {   //recorremos cada una de las líneas del archivo para que se procesen
                    //System.Console.WriteLine(linea);
                    string[] sp1 = linea.Split(':');
                    string clave = sp1[0];
                    if (cadena == clave)
                    {
                        resultado = sp1[1];
                    }
                }
                file.Close();
            }
            catch 
            {
                return "";
            }
            
            return resultado;
        }

        public static string[] obtenerContadorIni()
        {
            string[] resultado = new string[2];
            string contIni = Funciones.obtenerValoresIni("CONTADOR");
            if (contIni != "")
            {
                if (contIni.Split('#')[0] != DateTime.Now.ToString("yyyyMMdd"))
                {
                    resultado[0] = DateTime.Now.ToString("yyyyMMdd");   //FECHA
                    resultado[1] = "0";   //CONTADOR FECHA
                }
                else
                {
                    resultado[0] = contIni.Split('#')[0];   //FECHA
                    resultado[1] = contIni.Split('#')[1];   //CONTADOR FECHA
                }
            }
            else 
            {
                resultado[0] = DateTime.Now.ToString("yyyyMMdd");   //FECHA
                resultado[1] = "0";   //CONTADOR FECHA
            }
            return resultado;
        }


        public static void borrarValoresIni(string cadena)
        {
            File.Delete(cadena);
        }

        public static void guardarValoresIni(string cadena, string valor)
        {
            /*
            string nombreFilePath = @"C:\COMPARTIDA\ConfigCIERabbit.ini";
            TextWriter tw = new StreamWriter(nombreFilePath);
            tw.NewLine(cadena + ":" + valor);
            tw.NewLine();
            //tw.WriteLine(cadena + ":" + valor);
            tw.Close();
            */
            if (cadena == "CONTADOR")
            {
                if (File.Exists(@"C:\COMPARTIDA\ConfigCIERabbit.ini"))
                {

                    using (StreamWriter fileWrite = new StreamWriter(@"C:\COMPARTIDA\ConfigCIERabbitAUX.ini"))
                    {
                        using (StreamReader fielRead = new StreamReader(@"C:\COMPARTIDA\ConfigCIERabbit.ini"))
                        {
                            String line;
                            while ((line = fielRead.ReadLine()) != null)
                            {
                                string[] datos = line.Split(new char[] { ':' });
                                
                                if (datos[0] != cadena)
                                {
                                    fileWrite.WriteLine(datos[0] + ":" + datos[1]);
                                }

                            }
                            fileWrite.WriteLine(cadena + ":" + valor);
                            //File.AppendAllText(@"C:\COMPARTIDA\ConfigCIERabbitAUX.ini", cadena + ":" + valor);
                        }
                    }

                    File.Delete(@"C:\COMPARTIDA\ConfigCIERabbit.ini");
                    File.Move(@"C:\COMPARTIDA\ConfigCIERabbitAUX.ini", @"C:\COMPARTIDA\ConfigCIERabbit.ini");
                }
            }
            else if (File.Exists(@"C:\COMPARTIDA\ConfigCIERabbit.ini"))
            {
                File.AppendAllText(@"C:\COMPARTIDA\ConfigCIERabbit.ini", "\r\n");
                File.AppendAllText(@"C:\COMPARTIDA\ConfigCIERabbit.ini", cadena + ":" + valor);
            }
            else
            {
                File.AppendAllText(@"C:\COMPARTIDA\ConfigCIERabbit.ini", cadena + ":" + valor);
            }

        }


        public static void procesar_ficheros()
        {
            string folderPath = @"C:\COMPARTIDA";
            foreach (string nombreFilePath in Directory.EnumerateFiles(folderPath, "*.txt"))
            {   //recorremos todos los arhivos de la carpeta
                string nombreFile = Path.GetFileName(nombreFilePath);
                string readText = File.ReadAllText(nombreFilePath);
                JObject jsonfil = JObject.Parse(readText);
                string tipo = (string)jsonfil["claseEntidadIgeo"];
                string resulprocesa = "#";
                int empresaSAGE = Int32.Parse(Funciones.obtenerValoresIni("EMPRESA_SAGE"));
                if (tipo != null)
                {
                    string comando = (string)jsonfil["comando"];
                    switch (tipo)
                    {
                        case "SEDE":
                            resulprocesa = procesaSEDE(comando, jsonfil, empresaSAGE);
                            break;
                        case "CLIENTE":
                            resulprocesa = procesaCLIENTE(comando, jsonfil, empresaSAGE);
                            break;
                        case "FACTURA":
                            resulprocesa = procesaFACTURA(comando, jsonfil, empresaSAGE);
                            break;
                    }

                }
                //si el resultado no es OK escribimos en el log
                if (resulprocesa.Split('#')[0] != "OK") { }
                //pasamos el archivo a la carpeta de procesados
                string directorioProcesados = Path.Combine(folderPath, "procesados");
                if (!Directory.Exists(directorioProcesados))
                {
                    Directory.CreateDirectory(directorioProcesados);
                }
                if (File.Exists(Path.Combine(directorioProcesados, nombreFile)))
                {
                    File.Delete(Path.Combine(directorioProcesados, nombreFile));
                }
                System.IO.File.Move(nombreFilePath, Path.Combine(directorioProcesados, nombreFile));
            }
        }

        private static string procesaSEDE(string comando, JObject jsonfil, int empSAGE)
        {
            return "";
        }

        private static string procesaCLIENTE(string comando, JObject jsonfil, int empSAGE)
        {
            // Obtenemos claves primarias para tabla de SAGE
            string codCliente = (string)jsonfil["datos"]["codigo"];
            // FIN CLAVES PRIVAMRIAS
            if (codCliente == null) { return "ERROR#Faltan datos de clave primaria"; }
            string codFormaPAgo = (string)jsonfil["datos"]["codigoFormaPago"];

            return "";
        }

        private static string procesaFACTURA(string comando, JObject jsonfil, int empSAGE)
        {
            return "";
        }

    }
}

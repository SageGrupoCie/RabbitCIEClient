using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RabbitCIEClient
{
    class Funciones
    {
        private static string xServidor;
        private static string xUser;
        private static string xPass;
        private static string xDataBase;
        private static bool existeErrorEntidad;

        public static string obtenerValoresIni(string cadena, string tipoIni = "")
        {   // tipoIni puede ser "" o "BD", indicando respectivamente si el ini es el de parámetros de rabbit y contador, o es el de parámetros de la base de datos
            string linea;
            string pathPrincipal = AppDomain.CurrentDomain.BaseDirectory;
            if (tipoIni == "BD")
            {
                pathPrincipal += "ConfigCIERabbitDATABASE.ini";
            }
            else if (tipoIni == "EMAIL")
            {
                pathPrincipal += "ConfigCIERabbitEMAIL.ini";
            }
            else
            {
                pathPrincipal += "ConfigCIERabbit.ini";
            }
            if (!File.Exists(@pathPrincipal))
            {
                return "";
            }
                string nombreFilePath = @pathPrincipal;
            string resultado = "";
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(nombreFilePath);
                while ((linea = file.ReadLine()) != null)
                {   //recorremos cada una de las líneas del archivo para que se procesen
                    //System.Console.WriteLine(linea);
                    linea = Seguridad.DesEncriptar(linea);
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

        public static void eliminar_ficherosAntiguos(int diasLimitHisto)
        {
            // ELIMINAMOS LOS FICHEROS ANTIGUOS
            try
            {
                List<string> strFiles = Directory.GetFiles(Path.Combine(carpetaFicherosRabbit(), "procesados"), "*", SearchOption.AllDirectories).ToList();
                foreach (string fichero in strFiles)
                {
                    if (File.GetCreationTime(fichero).AddDays(diasLimitHisto) <= DateTime.Today)
                    {
                        File.Delete(fichero);
                    }
                }
            }
            catch { }
            try
            {
                List<string> strFiles = Directory.GetFiles(Path.Combine(carpetaFicherosRabbit(), "ERROR_procesados"), "*", SearchOption.AllDirectories).ToList();
                foreach (string fichero in strFiles)
                {
                    if (File.GetCreationTime(fichero).AddDays(diasLimitHisto) <= DateTime.Today)
                    {
                        File.Delete(fichero);
                    }
                }
            }
            catch { }

            // ELIMINAMOS LOS REGISTROS DE LA BASE DE DATOS ANTIGUOS EN LAS TABLAS TEMPORALES
            BaseDatos bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
            if (bd.estaConectado())
            {
                try
                {
                    string whereAx = "WHERE (CieProcesado = - 1) AND (DATEADD(DAY," + diasLimitHisto + ",CieFechaProcesado) < GETDATE())";
                    bd.eliminarDatosTabla("CieTmpSedeIGEO", whereAx);
                    bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
                    bd.eliminarDatosTabla("CieTmpClienteIGEO", whereAx);
                    whereAx = " WHERE (CodigoEmpresa IN (SELECT CodigoEmpresa" +
                              "                            FROM CieTmpCabFacturaIGEO" +
                              "                           WHERE (CieProcesado = -1)" +
                              "                             AND (DATEADD(DAY," + diasLimitHisto + ",CieFechaProcesado) <= GETDATE())))" +
                              "   AND (CieNumeroFacturaIGEO IN (SELECT CieNumeroFacturaIGEO" +
                              "                                   FROM CieTmpCabFacturaIGEO AS CieTmpCabFacturaIGEO_2" +
                              "                                  WHERE (CieProcesado = -1)" +
                              "                                    AND (DATEADD(DAY," + diasLimitHisto + ",CieFechaProcesado) <= GETDATE())))" +
                              "   AND (Ejercicio IN (SELECT Ejercicio" +
                              "                        FROM CieTmpCabFacturaIGEO AS CieTmpCabFacturaIGEO_1" +
                              "                       WHERE (CieProcesado = -1)" +
                              "                         AND (DATEADD(DAY," + diasLimitHisto + ",CieFechaProcesado) <= GETDATE())))";
                    bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
                    bd.eliminarDatosTabla("CieTmpLinFacturaIGEO", whereAx);
                    whereAx = "WHERE (CieProcesado = - 1) AND (DATEADD(DAY," + diasLimitHisto + ",CieFechaProcesado) <= GETDATE())";
                    bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
                    bd.eliminarDatosTabla("CieTmpCabFacturaIGEO", "WHERE (CieProcesado = - 1) AND (DATEADD(DAY," + diasLimitHisto + ",CieFechaProcesado) <= GETDATE())");
                    bd.desConectarBD();
                }
                catch { }
            }
        }

        public static void rellenaDatosBD()
        {
            xServidor = obtenerValoresIni("SERVIDOR", "BD");
            xUser = obtenerValoresIni("USUARIO", "BD");
            xPass = obtenerValoresIni("PASSWORD", "BD");
            xDataBase = obtenerValoresIni("DATABASE", "BD");
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


        public static void borrarValoresIni( string tipoIni = "")
        {
            string pathPrincipal = AppDomain.CurrentDomain.BaseDirectory;
            if (tipoIni == "BD")
            {
                pathPrincipal += "ConfigCIERabbitDATABASE.ini";
            }
            else if(tipoIni == "EMAIL")
            {
                pathPrincipal += "ConfigCIERabbitEMAIL.ini";
            }
            else
            {
                pathPrincipal += "ConfigCIERabbit.ini";
            }
            File.Delete(pathPrincipal);
        }

        public static String carpetaFicherosRabbit()
        {
            string pathPrincipal = AppDomain.CurrentDomain.BaseDirectory;
            pathPrincipal += "FICHEROS_RABBIT";
            if (!System.IO.Directory.Exists(pathPrincipal))
            {
                System.IO.Directory.CreateDirectory(pathPrincipal);
            }
            return pathPrincipal;
        }

        public static void guardarValoresIni(string cadena, string valor,string tipoIni = "")
        {   // tipoIni puede ser "" o "BD", indicando respectivamente si el ini es el de parámetros de rabbit y contador, o es el de parámetros de la base de datos
            /*
            string nombreFilePath = @"C:\COMPARTIDA\ConfigCIERabbit.ini";
            TextWriter tw = new StreamWriter(nombreFilePath);
            tw.NewLine(cadena + ":" + valor);
            tw.NewLine();
            //tw.WriteLine(cadena + ":" + valor);
            tw.Close();
            */
            string pathPrincipal = AppDomain.CurrentDomain.BaseDirectory;
            string pathAUX = "";
            if (tipoIni == "BD")
            {
                pathPrincipal += "ConfigCIERabbitDATABASE.ini";
            }
            else if (tipoIni == "EMAIL")
            {
                pathPrincipal += "ConfigCIERabbitEMAIL.ini";
            }
            else
            {
                pathPrincipal += "ConfigCIERabbit.ini";
                pathAUX += "ConfigCIERabbitAUX.ini";
            }
            if (cadena == "CONTADOR" && tipoIni == "")
            {
                if (File.Exists(@pathPrincipal))
                {

                    using (StreamWriter fileWrite = new StreamWriter(@pathAUX))
                    {
                        using (StreamReader fielRead = new StreamReader(pathPrincipal))
                        {
                            String line;
                            while ((line = fielRead.ReadLine()) != null)
                            {
                                string[] datos = Seguridad.DesEncriptar(line).Split(new char[] { ':' });
                                
                                if (datos[0] != cadena)
                                {
                                    fileWrite.WriteLine(Seguridad.Encriptar(datos[0] + ":" + datos[1]));
                                }

                            }
                            fileWrite.WriteLine(Seguridad.Encriptar(cadena + ":" + valor));
                            //File.AppendAllText(@"C:\COMPARTIDA\ConfigCIERabbitAUX.ini", cadena + ":" + valor);
                        }
                    }

                    File.Delete(@pathPrincipal);
                    File.Move(@pathAUX, @pathPrincipal);
                }
            }
            else if (File.Exists(@pathPrincipal))
            {
                File.AppendAllText(@pathPrincipal, "\r\n");
                File.AppendAllText(@pathPrincipal, Seguridad.Encriptar(cadena + ":" + valor));
            }
            else
            {
                File.AppendAllText(@pathPrincipal, Seguridad.Encriptar(cadena + ":" + valor));
            }

        }


        public static void procesar_ficheros(Logs lg, string esPRevio="")
        {
            rellenaDatosBD();
            bool errorEmpresa = false;
            int empresaSAGE=-1;
            bool chkCR = false;
            if (Funciones.obtenerValoresIni("OPTION_CREATE") == "True")
            {
                chkCR = true;
            }
            bool chkUP = false;
            if (Funciones.obtenerValoresIni("OPTION_UPDATE") == "True")
            {
                chkUP = true;
            }
            bool chkDL = false;
            if (Funciones.obtenerValoresIni("OPTION_DELETE") == "True")
            {
                chkDL = true;
            }
            bool chkCLIENTE = false;
            if (Funciones.obtenerValoresIni("OPTION_CLIENTE") == "True")
            {
                chkCLIENTE = true;
            }
            bool chkSEDE = false;
            if (Funciones.obtenerValoresIni("OPTION_SEDE") == "True")
            {
                chkSEDE = true;
            }
            bool chkFACTURA = false;
            if (Funciones.obtenerValoresIni("OPTION_FACTURA") == "True")
            {
                chkFACTURA = true;
            }
            try
            {
                empresaSAGE = Int32.Parse(obtenerValoresIni("EMPRESA_SAGE","BD"));
            }
            catch (Exception ex)
            {
                errorEmpresa = true;
            }
            if ((errorEmpresa) || (empresaSAGE == -1))
            {
                if (esPRevio == "SI") { lg.addError("erroresPreviosProcesado", "El código de empresa no se encuentra informado"); }
                else { lg.addError("erroresProcesado", "El código de empresa no se encuentra informado"); }
            }
            else
            {
                string folderPath = Funciones.carpetaFicherosRabbit();
                foreach (string nombreFilePath in Directory.EnumerateFiles(folderPath, "RabMQCIE_*.txt"))
                {   //recorremos todos los arhivos de la carpeta
                    existeErrorEntidad = false;
                    string nombreFile = Path.GetFileName(nombreFilePath);
                    string[] arraOrden = nombreFile.Split('_');
                    int ordenFic = int.Parse(arraOrden[1].Substring(2, arraOrden[1].Length - 2) + arraOrden[2].Substring(0, arraOrden[2].Length - 4));
                    string readText = File.ReadAllText(nombreFilePath);
                    string resulprocesa = "#";
                    try
                    {
                        JObject jsonfil = JObject.Parse(readText);
                        string tipo = jsonControl(jsonfil,lg, esPRevio, "claseEntidadIgeo");
                        if (tipo != null)
                        {
                            string comando = (string)jsonfil["comando"];
                            if (((comando == "CREATE") && !chkCR) || ((comando == "UPDATE") && !chkUP) || ((comando == "DELETE") && !chkDL))
                            {
                                resulprocesa = "OK#ELIMINAR";
                            }
                            else
                            {
                                switch (tipo)
                                {
                                    case "SEDE":
                                        if (chkSEDE)
                                        {
                                            resulprocesa = procesaSEDE(comando, jsonfil, empresaSAGE, ordenFic, lg, esPRevio);
                                        }
                                        else { resulprocesa = "OK#ELIMINAR"; }
                                        break;
                                    case "CLIENTE":
                                        if (chkCLIENTE)
                                        {
                                            resulprocesa = procesaCLIENTE(comando, jsonfil, empresaSAGE, ordenFic, lg, esPRevio);
                                        }
                                        else { resulprocesa = "OK#ELIMINAR"; }
                                        break;
                                    case "FACTURA":
                                        if (chkFACTURA)
                                        {
                                            resulprocesa = procesaFACTURACli(comando, jsonfil, empresaSAGE, ordenFic, lg, esPRevio);
                                        }
                                        else { resulprocesa = "OK#ELIMINAR"; }
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string errx = "No se ha podido procesar el fichero " + nombreFilePath;
                        if (esPRevio == "")
                        {
                            lg.addError("erroresProcesado", errx);
                        }
                        else
                        {
                            lg.addError("erroresPreviosProcesado", errx);
                        }
                    }

                    //si el resultado no es OK escribimos en el log
                    if (resulprocesa.Split('#')[0] != "OK") 
                    {
                        //pasamos el archivo a la carpeta de NO procesados
                        string directorioNOProcesados = Path.Combine(folderPath, "ERROR_procesados");
                        if (!Directory.Exists(directorioNOProcesados))
                        {
                            Directory.CreateDirectory(directorioNOProcesados);
                        }
                        if (File.Exists(Path.Combine(directorioNOProcesados, nombreFile)))
                        {
                            File.Delete(Path.Combine(directorioNOProcesados, nombreFile));
                        }
                        System.IO.File.Move(nombreFilePath, Path.Combine(directorioNOProcesados, nombreFile));
                        continue;
                    }
                    //pasamos el archivo a la carpeta de procesados
                    if (resulprocesa.Split('#')[1] == "ELIMINAR")
                    {
                        File.Delete(nombreFilePath);
                    }
                    else  //pasamos el archivo a la carpeta de procesados
                    {
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
            }
        }

        private static string procesaSEDE(string comando, JObject jsonfil, int empSAGE, int ordenFic, Logs lg, string esPRevio)
        {
            // Obtenemos claves primarias para tabla de SAGE
            //string codSede = jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigo");
            //string codCliente = jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoCliente");

            // FIN CLAVES PRIVAMRIAS
            //if ((codSede == "") && (codCliente == "")) { return "ERROR#Faltan datos de clave primaria"; }
            List<String> lista = new List<String>();
            //INSERTAMOS PRIMERO LAS CLAVES PRIMARIAS
            string codEmpresaSage = empSAGE.ToString();
            lista.Add(codEmpresaSage);
            string codCliente = jsonControl(jsonfil, lg, esPRevio, "datos", 2, "codigoCliente", "", "", "string", "", "SI");
            lista.Add(codCliente);
            string codSede = jsonControl(jsonfil, lg, esPRevio, "datos", 2, "codigo", "", "", "string", "", "SI");
            lista.Add(codSede);
            lista.Add(ordenFic.ToString());
            // FIN INSERCION CLAVES PRIMARIAS
            lista.Add(comando);
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "observaciones"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "zonaComercial"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoZonaComercial"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "latitud"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "longitud"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "distanciaDelegacionSede"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "nombre"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "estado"));




            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "motivoInactivo"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaBaja"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "direccion2"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaAlta"));
            string personaContacto = jsonControl(jsonfil, lg, esPRevio, "datos", 2, "personaContacto");
            if (personaContacto == null) { personaContacto = ""; }
            if (personaContacto.Length > 15){ personaContacto = personaContacto.Substring(0, 14); }
            lista.Add(personaContacto);
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "telefonoPersonaContacto"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "horarios"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "m2"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "m3"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "notificarCliente","","","bool")); //true false
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "cifDni"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "distrito"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "tipo"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosContacto", "telefono"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosContacto", "movil"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "emailEnvioFacturas"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosContacto", "fax"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosPostales", "direccion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosPostales", "codigoPostal"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosPostales", "localidad"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosPostales", "provincia"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosPostales", "alfa2codepais"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosPostales", "pais"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosPostales", "provinciaId"));
            
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "actividad", "nombre"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "codigoActividad"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "actividad", "tipoEmpresa"));

            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "codigoDelegacion"));

            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "datosBaseDelegacion", "id"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "datosBaseDelegacion", "nombre"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "datosBaseDelegacion", "cif"));


            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "idioma"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "codigoIdioma"));
            lista.Add("False");
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "parentCode"));
            lista.Add("");

            BaseDatos bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
            if (bd.estaConectado())
            {
                if (existeErrorEntidad) { return "ERROR#"; }
                string indicesNumericos = ",0,3,8,9,10,22,36,41,47,";
                string indicesBool = ",22,46,";
                string indicesDate = ",14,16,48,";
                bool resInsert = bd.InsertarDatos(lista,lg,esPRevio, indicesNumericos, "CieTmpSedeIGEO", indicesBool, indicesDate);
                bd.desConectarBD();
                bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
                if (!resInsert)
                {
                    bd.eliminarDatosTabla("CieTmpSedeIGEO", "WHERE CodigoEmpresa = " + codEmpresaSage + "  AND CodigoCliente = " + codCliente + " AND CodigoSede = " + codSede + " AND CieOrden = " + ordenFic.ToString());
                    bd.desConectarBD();
                    return "ERROR#";
                }
            }


            return "OK#";
        }
        
        private static string procesaCLIENTE(string comando, JObject jsonfil, int empSAGE, int ordenFic, Logs lg, string esPRevio)
        {
            // Obtenemos claves primarias para tabla de SAGE
            //string codCliente = jsonControl(jsonfil,lg, esPRevio, "datos",2,"codigo");

            // FIN CLAVES PRIVAMRIAS
            //if (codCliente == "") { return "ERROR#Faltan datos de clave primaria"; }

            List<String> lista = new List<String>();
            //INSERTAMOS PRIMERO LAS CLAVES PRIMARIAS
            string codEmpresaSage = empSAGE.ToString();
            lista.Add(codEmpresaSage);
            string codCliente = jsonControl(jsonfil, lg, esPRevio, "datos", 2, "codigo", "", "", "string", "", "SI");
            lista.Add(codCliente);
            lista.Add(ordenFic.ToString());
            // FIN INSERCION CLAVES PRIMARIAS
            
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "emailFacturacion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoTerminoPago"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoFormaPago"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "diasPago"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "diasEmisionRecibo"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "cuentas"));
            JArray items;
            int countLineas=0;
            bool agregarCuentas = true;
            try
            {
                items = (JArray)jsonfil["datos"]["cuentas"];
                countLineas = items.Count;
            }
            catch(NullReferenceException ex)
            {
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("0");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                //lista.Add("");
                //lista.Add("");
                //lista.Add("");
                //lista.Add("");
                //lista.Add("");
                //lista.Add("");
                //lista.Add("");
                agregarCuentas = false;
            }
            if (agregarCuentas)
            {
                if (countLineas > 0)
                {       // DEL ARRAY DE CUENTAS SOLO COGEMOS EL PRIMERO, SI EXISTE
                        //cuentas
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["codigoBanco"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["iban"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["swiftCode"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["nombre"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["activa"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["cuentaContable"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["bancoGenericoDto"]["nombre"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["bancoGenericoDto"]["prefijoIban"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["bancoGenericoDto"]["codigoSwift"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["bancoGenericoDto"]["cif"]);
                    lista.Add((string)jsonfil["datos"]["cuentas"].First["bancoGenericoDto"]["id"]);
                    //lista.Add((string)jsonfil["datos"]["cuentas"].First["bancoGenericoDto"]["empresaId"]);
                    //lista.Add((string)jsonfil["datos"]["cuentas"].First["bancoGenericoDto"]["parentId"]);
                    //lista.Add((string)jsonfil["datos"]["cuentas"].First["bancoGenericoDto"]["parentCode"]);
                    //lista.Add((string)jsonfil["datos"]["cuentas"].First["id"]);
                    //lista.Add((string)jsonfil["datos"]["cuentas"].First["empresaId"]);
                    //lista.Add((string)jsonfil["datos"]["cuentas"].First["parentId"]);
                    //lista.Add((string)jsonfil["datos"]["cuentas"].First["parentCode"]);
                    //fin cuentas
                }
                else
                {
                    lista.Add("");
                    lista.Add("");
                    lista.Add("");
                    lista.Add("");
                    lista.Add("");
                    lista.Add("");
                    lista.Add("");
                    lista.Add("");
                    lista.Add("");
                    lista.Add("");
                    lista.Add("");
                    //lista.Add("");
                    //lista.Add("");
                    //lista.Add("");
                    //lista.Add("");
                    //lista.Add("");
                    //lista.Add("");
                    //lista.Add("");
                }
            }
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaAlta"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "tipoCliente"));
            string nombreCli = jsonControl(jsonfil, lg, esPRevio, "datos", 2, "nombre");
            if (nombreCli == null) { nombreCli = ""; }
            if (nombreCli.Length > 34)
            {
                lista.Add(nombreCli.Substring(0, 33));
            }
            else 
            {
                lista.Add(nombreCli);
            }
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "apellidos"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "observaciones"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaAltaPotencial"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "numeroDeCliente"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoSecundario"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "comoNosHaConocido"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "detallesComoNosHaConocido"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "subCuenta"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "url"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "estado"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "motivoInactivo"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "importadoDesde"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nombreEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "direccionEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "distritoEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoPostalEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "localidadEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nombrePaisEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "telefonoEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoProvinciaEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nombreProvinciaEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "idProvinciaEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "atencionAEnvio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "observacionesFacturacion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "datosImportacion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "emailsDondeEnviarFacturas"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "riesgoEconomico"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "sepaFirmado"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "coste"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "rentabilidad"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "beneficio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "totalVentas"));


            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "formaCobro"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "diasDePago"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "diasDeFacturacion"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaPuntualFactura"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaEmisionPuntual"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "variasFechasFactura"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo1"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo2"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo3"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo4"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo5"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo6"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo7"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo8"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo9"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo10"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo11"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nDiasEmisionRecibo12"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "permiteValidarCertificados"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoComoProveedor"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "contadorNumeroDeCliente"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "emailsDondeEnviarPresupuestos"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "formaFacturacion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "prorrateoFacturasFechasContrato"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "generarFacturasAPlazoVencido"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "numeroFacturasAEmitir"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "requiereLlevarFacturasImpresos"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "esIntracomunitario"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "zonaComercial"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoZonaComercial"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaBaja"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "perdidoCompetidor"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "tcNecesarios"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigosClasificacion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nombresClasificacion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "grupoContableClienteCodigo"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "grupoContableClienteCuentaContableCliente"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "grupoContableNegocioCodigo"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "grupoContableNegocioDescripcion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "grupoRegistroIVACodigo"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "grupoRegistroIVADescripcion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "sectorCodigo"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "sectorNombre"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "actividad"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoActividad"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "actividad", "codigo"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoDelegacion"));
            //gestionadoPor
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "gestionadoPor", "id"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "gestionadoPor", "nombre"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "gestionadoPor", "apellidos"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "gestionadoPor", "alias"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "gestionadoPor", "codigoIdentificacion"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "gestionadoPor", "codigoEmpleado"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoGestionadoPor"));
            //fin gestionadoPor
            //idioma
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "idioma", "nombre"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "idioma", "codigo"));
            //fin idioma
            //personaContacto
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "personaContacto", "telefono"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "personaContacto", "movil"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "personaContacto", "email"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "personaContacto", "fax"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "nombre"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "personaContacto", "movil")); // repetido
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "personaContacto", "fax"));   // repetido
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "personaContacto", "telefono")); // repetido
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "personaContacto", "email"));  // repetido



            string cargoCli = jsonControl(jsonfil, lg, esPRevio, "datos", 3, "personaContacto", "cargo");
            if (cargoCli == null) { cargoCli = ""; }
            if (cargoCli.Length > 20)
            {
                lista.Add(cargoCli.Substring(0,19));
            }
            else
            {
                lista.Add(cargoCli);
            }
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "esLaPrincipal"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "direccion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "codigoPostal"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "localidad"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "numeroFax"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 4, "personaContacto", "provincia", "id"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 4, "personaContacto", "provincia", "nombre"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 4, "personaContacto", "provincia", "codigo"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 4, "personaContacto", "provincia", "paisId"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "id"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "empresaId"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "parentId"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "personaContacto", "parentCode"));
            //fin personaContacto
            //datosFacturacion
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "nombreFacturacion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "codigoIdentificacion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "direccion"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "codigoPostal"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "datosFacturacion", "alfa2codepais"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "datosFacturacion", "pais"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "datosFacturacion", "comuna"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 3, "datosFacturacion", "regimenFiscal"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "localidad"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "provincia"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "codigoProvincia"));
            
 
            
            //fin datosFacturacion
            //datosClienteFacturae
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_activada"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_canalEnvio"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_customerId"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_dir3"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_dir31"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_dir32"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_dir33"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_dir34"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_direccionOficinaContable"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_direccionOrganoGestor"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosClienteFacturae", "facturae_denominacionUnidadTramitadora"));
            //fin datosClienteFacturae
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "clienteEdi"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "tipoEDI"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "tipoPersonaReceptor"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "tipoResidencia"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "plataFormaEDI"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "autoFactura"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "subTipoEDI"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "infoExtraEDI"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "referenciaContratoReceptorEDI"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoPagador"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaBloqueo"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "cuentaBancariaPreferida"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "comuna"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "acteco"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "figuraJuridica"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoFiguraJuridica"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoIdentificativoDestinatarioFacturaE"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "partidaIva"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "emailNotificacionesCertificadas"));
            //datosBaseDelegacion
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosBaseDelegacion", "id"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosBaseDelegacion", "nombre"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosBaseDelegacion", "codigo"));
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosBaseDelegacion", "cif"));
            lista.Add("False"); //Procesado
            //fin datosBaseDelegacion
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "localidad"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "provincia"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 3, "datosFacturacion", "codigoProvincia"));
            lista.Add(comando);
            lista.Add("");


            BaseDatos bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
            if(bd.estaConectado())
            {
                if (existeErrorEntidad) { return "ERROR#"; }
                string indicesNumericos = ",0,2,46,47,48,49,71,90,";
                string indicesBool = ",12,29,45,70,72,73,119,124,141,";
                string indicesDate = ",19,129,146,";
                bool resInsert = bd.InsertarDatos(lista,lg,esPRevio, indicesNumericos, "CieTmpClienteIGEO",indicesBool,indicesDate);
                bd.desConectarBD();
                bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
                if (!resInsert)
                {
                    bd.eliminarDatosTabla("CieTmpClienteIGEO", "WHERE CodigoEmpresa = " + codEmpresaSage + "  AND CodigoCliente = " + codCliente + " AND CieOrden = " + ordenFic.ToString());
                    bd.desConectarBD();
                    return "ERROR#";
                }
            }



            return "OK#";
        }

        

        private static string procesaFACTURACli(string comando, JObject jsonfil, int empSAGE, int ordenFic, Logs lg, string esPRevio)
        {
            BaseDatos bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
            List<String> lista = new List<String>();
            //CLAVES PRIMARIAS
            string codEmpresaSage = empSAGE.ToString();
            lista.Add(codEmpresaSage);
            string numFacturaIGEO = jsonControl(jsonfil, lg, esPRevio, "datos", 2, "numeroFactura", "", "", "string", "", "SI");
            lista.Add(numFacturaIGEO);
            //Obtenemos el ejercico
            DateTime xAux;
            string anyo = "";
            try
            {
                xAux = DateTime.Parse(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "fechaEmision", "", "", "datetime"));
                anyo = xAux.ToString("yyyy");
            }
            catch
            {
                return "ERROR#";
            }
            lista.Add(anyo);
            //Fin obtención ejercicio
            lista.Add(ordenFic.ToString());
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "fechaEmision","","", "datetime", "","SI"));
            //FIN CLAVES PRIMARIAS
            //lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "cif"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "numero"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaRegistro"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "fechaVencimiento"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "facturaANombreDe"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "facturaANumeroCliente"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "importeSinImpuestos"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "importe"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "formaCobro"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "metodoPago"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "numeroFacturaCorregida"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "nombreLineaNegocio"));
            lista.Add(jsonControl(jsonfil,lg, esPRevio, "datos", 2, "codigoLineaNegocio"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "cuota"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "numeroDePedido"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "tipoFactura"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "folioUUID"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "codigoDivisa"));
            lista.Add(jsonControl(jsonfil, lg, esPRevio, "datos", 2, "numeroDePedido"));//campo duplicado. Hay que solucionarlo en la definición de la tabla en SAGE
            lista.Add("False");
            lista.Add("");
            lista.Add(comando);
            

            if (!existeErrorEntidad)
            {
                //lineas
                JArray items = (JArray)jsonfil["datos"]["lineas"];
                int countLineas = items.Count;
                int countLinAX = 0;
                while (countLineas > countLinAX)
                {
                    List<String> listaLineas = new List<String>();
                    //CLAVES PRIMARIAS
                    listaLineas.Add(codEmpresaSage);
                    listaLineas.Add(numFacturaIGEO);
                    listaLineas.Add(anyo);
                    listaLineas.Add(ordenFic.ToString());
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["numero"]);
                    //FIN CLAVES PRIMARIAS
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["tipo"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["descripcion"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["dimension1"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["dimension2"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["cantidad"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["precioVenta"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["precioUnitario"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["importeSinIva"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["importe"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["porcentajeImpuesto"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["codigoLineaNegocio"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["codigoProducto"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["tipoProductoServicio"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["claveProductoServicio"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["subCuenta"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["numeroPedido"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["unidadDeMedida"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["descuento"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["cuota"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["esTangible"]);
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["importeBruto"]);
                    //listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["numero"]);     //Retenciones ???
                    listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["codigoSede"]);
                    listaLineas.Add("False");
                    listaLineas.Add("");
                    listaLineas.Add(comando);
                    //Insertar línea en BD teniendo en cuenta las claves primarias de la cabecera
                    if (bd.estaConectado())
                    {
                        string indicesNumericos = ",0,2,3,4,9,10,11,12,13,14,22,23,25,";
                        string indicesBool = ",24,27,";
                        string indicesDate = ",28,";
                        bool resInsert = bd.InsertarDatos(listaLineas, lg, esPRevio, indicesNumericos, "CieTmpLinFacturaIGEO", indicesBool, indicesDate);
                        bd.desConectarBD();
                        bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
                        if (!resInsert) 
                        {
                            bd.eliminarDatosTabla("CieTmpLinFacturaIGEO", "WHERE CodigoEmpresa = " + codEmpresaSage + "  AND CieNumeroFacturaIGEO = " + numFacturaIGEO + " AND Ejercicio = " + anyo + " AND CieOrden = " + ordenFic.ToString());
                            bd.desConectarBD();
                            return "ERROR#"; 
                        }
                    }
                    countLinAX += 1;
                }
                //fin lineas
            }
            else 
            {
                return "ERROR#";
            }
            if (bd.estaConectado())
            {
                string indicesNumericos = ",0,2,3,5,10,11,17,18,";
                string indicesBool = ",23,";
                string indicesDate = ",4,6,7,24,";
                bool resInsert = bd.InsertarDatos(lista, lg, esPRevio, indicesNumericos, "CieTmpCabFacturaIGEO", indicesBool, indicesDate);
                bd.desConectarBD();
                if (!resInsert)
                {
                    bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
                    bd.eliminarDatosTabla("CieTmpCabFacturaIGEO", "WHERE CodigoEmpresa = " + codEmpresaSage + "  AND CieNumeroFacturaIGEO = " + numFacturaIGEO + " AND Ejercicio = " + anyo + " AND CieOrden = " + ordenFic.ToString());
                    bd.desConectarBD();
                    return "ERROR#";
                }
            }

            return "OK#";

        }

        private static string jsonControl(JObject jsonfil, Logs lg, string esPRevio, string codClave, int numClaves = 1, string codClave2 = "", string codClave3 = "", string codClave4 = "", string tipoCampo = "string", string tipoFichero = "", string obligatorio = "NO")
        {
            string resultado = "";
            string clavesJSON = "";
            try
            {
                if (numClaves == 4)
                {
                    resultado = (string)jsonfil[codClave][codClave2][codClave3][codClave4];
                    clavesJSON = "[" + codClave + "][" + codClave2 + "][" + codClave3 + "][" + codClave4 + "]";
                }
                else if (numClaves == 3)
                {
                    resultado = (string)jsonfil[codClave][codClave2][codClave3];
                    clavesJSON = "[" + codClave + "][" + codClave2 + "][" + codClave3 + "]";
                }
                else if (numClaves == 2)
                {
                    resultado = (string)jsonfil[codClave][codClave2];
                    clavesJSON = "[" + codClave + "][" + codClave2 + "]";
                }
                else if (numClaves == 1)
                {
                    resultado = (string)jsonfil[codClave];
                    clavesJSON = "[" + codClave + "]";
                }
                else
                {
                    resultado = "";
                }
            }
            catch
            {
                resultado = "";
            }

            if (tipoCampo != "string")
            {   //COMPROBAMOS QUE LOS TIPOS SE CORRESPONDEN REALIZANDO UNA CONVERSIÓN AUXILIAR DE COMPROBACIÓN
                bool xCorrecto = true;
                switch (tipoCampo)
                {
                    case "int":
                        try
                        {
                            int xAux;
                            xAux = int.Parse(resultado);
                        }
                        catch
                        {
                            xCorrecto = false;
                        }
                        break;
                    case "double":
                        try
                        {
                            double xAux;
                            xAux = Double.Parse(resultado);
                        }
                        catch
                        {
                            xCorrecto = false;
                        }
                        break;
                    case "datetime":
                        try
                        {
                            DateTime xAux;
                            xAux = DateTime.Parse(resultado);
                        }
                        catch
                        {
                            xCorrecto = false;
                        }
                        break;
                    case "bool":
                        try
                        {
                            bool xAux;
                            xAux = bool.Parse(resultado);
                            resultado = "0";
                            if (xAux == true) { resultado = "-1"; }
                        }
                        catch
                        {
                            xCorrecto = false;
                        }
                        break;
                }
                if ((!xCorrecto) && (obligatorio != "NO"))
                {
                    existeErrorEntidad = true;
                    string tipolist = "erroresProcesado";
                    if (esPRevio != "") { tipolist = "erroresPreviosProcesado"; }
                    lg.addError(tipolist, "Error al convertir campo (" + clavesJSON + ") al formato " + tipoCampo + ".");
                }
                else if ((!xCorrecto) && (obligatorio == "NO"))
                {
                    return null;
                }

                if (resultado == null) { resultado = ""; }
                if ((obligatorio != "NO") && (resultado == ""))
                {
                    existeErrorEntidad = true;
                    string tipolist = "erroresProcesado";
                    if (esPRevio != "") { tipolist = "erroresPreviosProcesado"; }
                    lg.addError(tipolist, "El campo (" + clavesJSON + ") de tipo " + tipoCampo + " no está informado y es obligatorio.");
                }
                //return resultado;
            }
            return resultado;

        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RabbitCIEClient
{
    class Funciones
    {
        private static string xServidor;
        private static string xUser;
        private static string xPass;
        private static string xDataBase;

        public static string obtenerValoresIni(string cadena, string tipoIni = "")
        {   // tipoIni puede ser "" o "BD", indicando respectivamente si el ini es el de parámetros de rabbit y contador, o es el de parámetros de la base de datos
            string linea;
            string pathPrincipal = AppDomain.CurrentDomain.BaseDirectory;
            if (tipoIni == "BD")
            {
                pathPrincipal += "ConfigCIERabbitDATABASE.ini";
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

                    File.Delete(@pathPrincipal);
                    File.Move(@pathAUX, @pathPrincipal);
                }
            }
            else if (File.Exists(@pathPrincipal))
            {
                File.AppendAllText(@pathPrincipal, "\r\n");
                File.AppendAllText(@pathPrincipal, cadena + ":" + valor);
            }
            else
            {
                File.AppendAllText(@pathPrincipal, cadena + ":" + valor);
            }

        }


        public static void procesar_ficheros(Logs lg, string esPRevio="")
        {
            rellenaDatosBD();
            string lgCab = "Error en ficheros pendientes de procesar:";
            string folderPath = Funciones.carpetaFicherosRabbit();
            foreach (string nombreFilePath in Directory.EnumerateFiles(folderPath, "RabMQCIE_*.txt"))
            {   //recorremos todos los arhivos de la carpeta
                string nombreFile = Path.GetFileName(nombreFilePath);
                string[] arraOrden = nombreFile.Split('_');
                int ordenFic = int.Parse(arraOrden[1].Substring(2, arraOrden[1].Length-2) + arraOrden[2].Substring(0,arraOrden[2].Length-4));
                string readText = File.ReadAllText(nombreFilePath);
                string resulprocesa = "#";
                try
                {
                    JObject jsonfil = JObject.Parse(readText);
                    string tipo = jsonControl(jsonfil, "claseEntidadIgeo");
                    
                    int empresaSAGE = Int32.Parse(Funciones.obtenerValoresIni("EMPRESA_SAGE", "BD"));
                    if (tipo != null)
                    {
                        string comando = (string)jsonfil["comando"];
                        switch (tipo)
                        {
                            case "SEDE":
                                resulprocesa = procesaSEDE(comando, jsonfil, empresaSAGE, ordenFic);
                                break;
                            case "CLIENTE":
                                resulprocesa = procesaCLIENTE(comando, jsonfil, empresaSAGE, ordenFic);
                                break;
                            case "FACTURA":
                                resulprocesa = procesaFACTURA(comando, jsonfil, empresaSAGE, ordenFic);
                                break;
                        }

                    }
                }
                catch
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

        private static string procesaSEDE(string comando, JObject jsonfil, int empSAGE, int ordenFic)
        {
            return "";
        }

        private static string procesaCLIENTE(string comando, JObject jsonfil, int empSAGE, int ordenFic)
        {
            // Obtenemos claves primarias para tabla de SAGE
            string codCliente = jsonControl(jsonfil, "datos",2,"codigo");
            // FIN CLAVES PRIVAMRIAS
            if (codCliente == "") { return "ERROR#Faltan datos de clave primaria"; }

            List<String> lista = new List<String>();

            lista.Add(jsonControl(jsonfil, "datos", 2, "emailFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoTerminoPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoFormaPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "diasPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "diasEmisionRecibo"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "cuentas"));
            /*
            if (jsonfil["datos"]["cliente"]["cuentas"].HasValues)
            {       // DEL ARRAY DE CUENTAS SOLO COGEMOS EL PRIMERO, SI EXISTE
                //cuentas
                clieCuentacodigoBanco = (string)jsonfil["datos"]["cliente"]["cuentas"].First["codigoBanco"];
                clieCuentaiban = (string)jsonfil["datos"]["cliente"]["cuentas"].First["iban"];
                clieCuentaswiftCode = (string)jsonfil["datos"]["cliente"]["cuentas"].First["swiftCode"];
                clieCuentanombre = (string)jsonfil["datos"]["cliente"]["cuentas"].First["nombre"];
                clieCuentaactiva = (string)jsonfil["datos"]["cliente"]["cuentas"].First["activa"];
                clieCuentacuentaContable = (string)jsonfil["datos"]["cliente"]["cuentas"].First["cuentaContable"];
                bancGenDtonombre = (string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["nombre"];
                bancGenDtoprefijoIban = (string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["prefijoIban"];
                bancGenDtocodigoSwift = (string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["codigoSwift"];
                bancGenDtocif = (string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["cif"];
                bancGenDtoid = (string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["id"];
                bancGenDtoempresaId = (string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["empresaId"];
                bancGenDtoparentId = (string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["parentId"];
                bancGenDtoparentCode = (string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["parentCode"];
                clieCuentaid = (string)jsonfil["datos"]["cliente"]["cuentas"].First["id"];
                clieCuentaempresaId = (string)jsonfil["datos"]["cliente"]["cuentas"].First["empresaId"];
                clieCuentaparentId = (string)jsonfil["datos"]["cliente"]["cuentas"].First["parentId"];
                clieCuentaparentCode = (string)jsonfil["datos"]["cliente"]["cuentas"].First["parentCode"];
                //fin cuentas
            }
            else
            {
                string clieCuentacodigoBanco = "";
                string clieCuentaiban = "";
                string clieCuentaswiftCode = "";
                string clieCuentanombre = "";
                string clieCuentaactiva = "";
                string clieCuentacuentaContable = "";
                string bancGenDtonombre = "";
                string bancGenDtoprefijoIban = "";
                string bancGenDtocodigoSwift = "";
                string bancGenDtocif = "";
                string bancGenDtoid = "";
                string bancGenDtoempresaId = "";
                string bancGenDtoparentId = "";
                string bancGenDtoparentCode = "";
                string clieCuentaid = "";
                string clieCuentaempresaId = "";
                string clieCuentaparentId = "";
                string clieCuentaparentCode = "";
            }*/
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaAlta"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "tipoCliente"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "apellidos"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "observaciones"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaAltaPotencial"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "numeroDeCliente"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoSecundario"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "comoNosHaConocido"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "detallesComoNosHaConocido"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "subCuenta"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "url"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "estado"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "motivoInactivo"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "importadoDesde"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nombreEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "direccionEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "distritoEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoPostalEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "localidadEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nombrePaisEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "telefonoEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoProvinciaEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nombreProvinciaEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "idProvinciaEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "atencionAEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "observacionesFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "datosImportacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "emailsDondeEnviarFacturas"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "riesgoEconomico"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "sepaFirmado"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "coste"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "rentabilidad"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "beneficio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "totalVentas"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "formaCobro"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "diasDePago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "diasDeFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaPuntualFactura"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaEmisionPuntual"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "variasFechasFactura"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo1"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo2"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo3"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo4"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo5"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo6"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo7"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo8"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo9"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo10"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo11"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo12"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "permiteValidarCertificados"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoComoProveedor"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "contadorNumeroDeCliente"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "emailsDondeEnviarPresupuestos"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "formaFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "prorrateoFacturasFechasContrato"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "generarFacturasAPlazoVencido"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "numeroFacturasAEmitir"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "requiereLlevarFacturasImpresos"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "esIntracomunitario"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "zonaComercial"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoZonaComercial"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaBaja"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "perdidoCompetidor"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "tcNecesarios"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigosClasificacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nombresClasificacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "grupoContableClienteCodigo"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "grupoContableClienteCuentaContableCliente"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "grupoContableNegocioCodigo"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "grupoContableNegocioDescripcion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "grupoRegistroIVACodigo"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "grupoRegistroIVADescripcion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "sectorCodigo"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "sectorNombre"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "actividad"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoActividad"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoDelegacion"));
            //gestionadoPor
            lista.Add(jsonControl(jsonfil, "datos", 3, "gestionadoPor", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "gestionadoPor", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "gestionadoPor", "apellidos"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "gestionadoPor", "alias"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "gestionadoPor", "codigoIdentificacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "gestionadoPor", "codigoEmpleado"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoGestionadoPor"));
            //fin gestionadoPor
            //idioma
            lista.Add(jsonControl(jsonfil, "datos", 3, "idioma", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "idioma", "codigo"));
            //fin idioma
            //personaContacto
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "movil"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "fax"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "telefono"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "email"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "cargo"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "esLaPrincipal"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "direccion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "codigoPostal"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "localidad"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "numeroFax"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "personaContacto", "provincia", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "personaContacto", "provincia", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "personaContacto", "provincia", "codigo"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "personaContacto", "provincia", "paisId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "empresaId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "parentId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "personaContacto", "parentCode"));
            //fin personaContacto
            //datosFacturacion
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "nombreFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "codigoIdentificacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "direccion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "codigoPostal"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "localidad"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "provincia"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "codigoProvincia"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "alfa2codepais"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "pais"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "comuna"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacion", "regimenFiscal"));
            //fin datosFacturacion
            //datosClienteFacturae
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_activada"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_canalEnvio"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_customerId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir3"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir31"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir32"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir33"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir34"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_direccionOficinaContable"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_direccionOrganoGestor"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_denominacionUnidadTramitadora"));
            //fin datosClienteFacturae
            lista.Add(jsonControl(jsonfil, "datos", 2, "clienteEdi"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "tipoEDI"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "tipoPersonaReceptor"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "tipoResidencia"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "plataFormaEDI"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "autoFactura"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "subTipoEDI"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "infoExtraEDI"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "referenciaContratoReceptorEDI"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoPagador"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaBloqueo"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "cuentaBancariaPreferida"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "comuna"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "acteco"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "figuraJuridica"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoFiguraJuridica"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoIdentificativoDestinatarioFacturaE"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "partidaIva"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "emailNotificacionesCertificadas"));
            //datosBaseDelegacion
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "codigo"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "cif"));
            //fin datosBaseDelegacion
            lista.Add(jsonControl(jsonfil, "datos", 2, "id"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "empresaId"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "parentId"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "parentCode"));

            BaseDatos bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
            if(bd.estaConectado())
            {
                bd.InsertarDatos(lista);
                bd.desConectarBD();
            }



            return "OK#";
        }

        private static string procesaFACTURA(string comando, JObject jsonfil, int empSAGE, int ordenFic)
        {
            string cif = jsonControl(jsonfil, "datos", 2, "cif");
            string numeroFactura = jsonControl(jsonfil, "datos", 2, "numeroFactura");
            string numero = jsonControl(jsonfil, "datos", 2, "numero");
            string fechaRegistro = jsonControl(jsonfil, "datos", 2, "fechaRegistro");
            string fechaEmision = jsonControl(jsonfil, "datos", 2, "fechaEmision");
            string fechaVencimiento = jsonControl(jsonfil, "datos", 2, "fechaVencimiento");
            string facturaANombreDe = jsonControl(jsonfil, "datos", 2, "facturaANombreDe");
            string facturaANumeroCliente = jsonControl(jsonfil, "datos", 2, "facturaANumeroCliente");
            string importeSinImpuestos = jsonControl(jsonfil, "datos", 2, "importeSinImpuestos");
            string importe = jsonControl(jsonfil, "datos", 2, "importe");
            string formaCobro = jsonControl(jsonfil, "datos", 2, "formaCobro");
            string metodoPago = jsonControl(jsonfil, "datos", 2, "metodoPago");
            string numeroFacturaCorregida = jsonControl(jsonfil, "datos", 2, "numeroFacturaCorregida");
            string nombreLineaNegocio = jsonControl(jsonfil, "datos", 2, "nombreLineaNegocio");
            string codigoLineaNegocio = jsonControl(jsonfil, "datos", 2, "codigoLineaNegocio");
            string cuota = jsonControl(jsonfil, "datos", 2, "cuota");
            //datosFacturacionEmisor
            string datFacEmisnombreFacturacion = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "nombreFacturacion");
            string datFacEmiscodigoIdentificacion = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "codigoIdentificacion");
            string datFacEmisdireccion = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "direccion");
            string datFacEmiscodigoPostal = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "codigoPostal");
            string datFacEmislocalidad = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "localidad");
            string datFacEmisprovincia = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "provincia");
            string datFacEmiscodigoProvincia = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "codigoProvincia");
            string datFacEmisalfa2codepais = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "alfa2codepais");
            string datFacEmispais = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "pais");
            string datFacEmiscomuna = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "comuna");
            string datFacEmisregimenFiscal = jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "regimenFiscal");
            //fin datosFacturacionEmisor
            //datosFacturacionReceptor
            string datFacRecepnombreFacturacion = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "nombreFacturacion");
            string datFacRecepcodigoIdentificacion = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "codigoIdentificacion");
            string datFacRecepdireccion = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "direccion");
            string datFacRecepcodigoPostal = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "codigoPostal");
            string datFacReceplocalidad = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "localidad");
            string datFacRecepprovincia = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "provincia");
            string datFacRecepcodigoProvincia = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "codigoProvincia");
            string datFacRecepalfa2codepais = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "alfa2codepais");
            string datFacReceppais = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "pais");
            string datFacRecepcomuna = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "comuna");
            string datFacRecepregimenFiscal = jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "regimenFiscal");
            //fin datosFacturacionReceptor
            string comuna = jsonControl(jsonfil, "datos", 2, "comuna");
            string acteco = jsonControl(jsonfil, "datos", 2, "acteco");
            string simulaUnicaLineaFactura = jsonControl(jsonfil, "datos", 2, "simulaUnicaLineaFactura");
            string descripcionLineaUnica = jsonControl(jsonfil, "datos", 2, "descripcionLineaUnica");
            string tipoFactura = jsonControl(jsonfil, "datos", 2, "tipoFactura");
            string folioUUID = jsonControl(jsonfil, "datos", 2, "folioUUID");
            string datosPreSellado = jsonControl(jsonfil, "datos", 2, "datosPreSellado");
            string datosPostSellado = jsonControl(jsonfil, "datos", 2, "datosPostSellado");
            string datosSelladoOriginal = jsonControl(jsonfil, "datos", 2, "datosSelladoOriginal");
            //contrato
            string contratonumero = jsonControl(jsonfil, "datos", 3, "contrato", "numero");
            string contratodelegacion = jsonControl(jsonfil, "datos", 3, "contrato", "delegacion");
            string contratofechaInicioPeriodoFacturacion = jsonControl(jsonfil, "datos", 3, "contrato", "fechaInicioPeriodoFacturacion");
            string contratofechaFinPeriodoFacturacion = jsonControl(jsonfil, "datos", 3, "contrato", "fechaFinPeriodoFacturacion");
            string contratofechaInicio = jsonControl(jsonfil, "datos", 3, "contrato", "fechaInicio");
            string contratofechaFin = jsonControl(jsonfil, "datos", 3, "contrato", "fechaFin");
            string contratocodIdentificContratPublica = jsonControl(jsonfil, "datos", 3, "contrato", "codigoIdentificacionContratacionPublica");
            string contratocodigoProyecto = jsonControl(jsonfil, "datos", 3, "contrato", "codigoProyecto");
            string contratonumeroDePedido = jsonControl(jsonfil, "datos", 3, "contrato", "numeroDePedido");
            string contratoid = jsonControl(jsonfil, "datos", 3, "contrato", "id");
            string contratoempresaId = jsonControl(jsonfil, "datos", 3, "contrato", "empresaId");
            string contratoparentId = jsonControl(jsonfil, "datos", 3, "contrato", "parentId");
            string contratoparentCode = jsonControl(jsonfil, "datos", 3, "contrato", "parentCode");
            //fin contrato
            //venta
            string ventanumeroVenta = jsonControl(jsonfil, "datos", 3, "venta", "numeroVenta");
            string ventafechaVenta = jsonControl(jsonfil, "datos", 3, "venta", "fechaVenta");
            string ventadelegacion = jsonControl(jsonfil, "datos", 3, "venta", "delegacion");
            string ventafechaInicioPeriodoFacturacion = jsonControl(jsonfil, "datos", 3, "venta", "fechaInicioPeriodoFacturacion");
            string ventafechaFinPeriodoFacturacion = jsonControl(jsonfil, "datos", 3, "venta", "fechaFinPeriodoFacturacion");
            string ventanumeroDePedido = jsonControl(jsonfil, "datos", 3, "venta", "numeroDePedido");
            string ventaid = jsonControl(jsonfil, "datos", 3, "venta", "id");
            string ventaempresaId = jsonControl(jsonfil, "datos", 3, "venta", "empresaId");
            string ventaparentId = jsonControl(jsonfil, "datos", 3, "venta", "parentId");
            string ventaparentCode = jsonControl(jsonfil, "datos", 3, "venta", "parentCode");
            //fin venta
            //cuentaCliente
            string cuentaClicodigoBanco = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "codigoBanco");
            string cuentaCliiban = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "iban");
            string cuentaCliswiftCode = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "swiftCode");
            string cuentaClinombre = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "nombre");
            string cuentaCliactiva = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "activa");
            string cuentaClicuentaContable = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "cuentaContable");
            string cuClibancGennombre = jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "nombre");
            string cuClibancGenprefijoIban = jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "prefijoIban");
            string cuClibancGencodigoSwift = jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "codigoSwift");
            string cuClibancGencif = jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "cif");
            string cuClibancGenid = jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "id");
            string cuClibancGenempresaId = jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "empresaId");
            string cuClibancGenparentId = jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "parentId");
            string cuClibancGenparentCode = jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "parentCode");
            string cuentaCliid = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "id");
            string cuentaCliempresaId = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "empresaId");
            string cuentaCliparentId = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "parentId");
            string cuentaCliparentCode = jsonControl(jsonfil, "datos", 3, "cuentaCliente", "parentCode");
            //fin cuentaCliente
            //cliente
            string clientecodigo = jsonControl(jsonfil, "datos", 3, "cliente", "codigo");
            //fin cliente
            //serieFacturacion
            string serieFactid = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "id");
            string serieFactempresaId = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "empresaId");
            string serieFactcontador = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "contador");
            string serieFactprefijo = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "prefijo");
            string serieFactsufijo = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "sufijo");
            string serieFactletra = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "letra");
            string serieFactesParaRectificativas = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "esParaRectificativas");
            string serieFactnombre = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "nombre");
            string serieFactimpuestoConcretoForzado = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "impuestoConcretoForzado");
            string serieFactesPrehistorica = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "esPrehistorica");
            string serieFactesLaPrincipal = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "esLaPrincipal");
            string serieFactserieFacturacionRectificativaId = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "serieFacturacionRectificativaId");
            string serieFactreseteoAnual = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "reseteoAnual");
            string serieFacttimbradoConIgeo = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "timbradoConIgeo");
            string serieFacttipoFacturas = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "tipoFacturas");
            string serieFactesParaFacturasCreditoAR = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "esParaFacturasCreditoAR");
            string serieFactidentificadorEnSistemaTimbrado = jsonControl(jsonfil, "datos", 3, "serieFacturacion", "identificadorEnSistemaTimbrado");
            string serieFactdatBasDelID = jsonControl(jsonfil, "datos", 4, "serieFacturacion", "datosBaseDelegacion", "id");
            string serieFactdatBasDelnombre = jsonControl(jsonfil, "datos", 4, "serieFacturacion", "datosBaseDelegacion", "nombre");
            string serieFactdatBasDelcodigo = jsonControl(jsonfil, "datos", 4, "serieFacturacion", "datosBaseDelegacion", "codigo");
            string serieFactdatBasDelcif = jsonControl(jsonfil, "datos", 4, "serieFacturacion", "datosBaseDelegacion", "cif");
            //fin serieFacturacion
            //int contLineas = jsonfil["datos"]["lineas"].co;
            //lineas
            JArray items = (JArray)jsonfil["datos"]["lineas"];
            int countLineas = items.Count;
            int countLinAX = 0;
            while(countLineas > countLinAX)
            {
                string tipoLin = (string)jsonfil["datos"]["lineas"][countLinAX]["tipo"];
                string numLin = (string)jsonfil["datos"]["lineas"][countLinAX]["numero"];

                string descripcionLin = (string)jsonfil["datos"]["lineas"][countLinAX]["descripcion"];
                string dimension1mLin = (string)jsonfil["datos"]["lineas"][countLinAX]["dimension1"];
                string dimension2Lin = (string)jsonfil["datos"]["lineas"][countLinAX]["dimension2"];
                string cantidadLin = (string)jsonfil["datos"]["lineas"][countLinAX]["cantidad"];
                string precioVentaLin = (string)jsonfil["datos"]["lineas"][countLinAX]["precioVenta"];
                string precioUnitarioLin = (string)jsonfil["datos"]["lineas"][countLinAX]["precioUnitario"];
                string importeSinIvaLin = (string)jsonfil["datos"]["lineas"][countLinAX]["importeSinIva"];
                string importeLin = (string)jsonfil["datos"]["lineas"][countLinAX]["importe"];
                string porcentajeImpuestoLin = (string)jsonfil["datos"]["lineas"][countLinAX]["porcentajeImpuesto"];
                string codigoLineaNegocioLin = (string)jsonfil["datos"]["lineas"][countLinAX]["codigoLineaNegocio"];
                string codigoProductoLin = (string)jsonfil["datos"]["lineas"][countLinAX]["codigoProducto"];
                string tipoProductoServicioLin = (string)jsonfil["datos"]["lineas"][countLinAX]["tipoProductoServicio"];
                string claveProductoServicioLin = (string)jsonfil["datos"]["lineas"][countLinAX]["claveProductoServicio"];
                string subCuentaLin = (string)jsonfil["datos"]["lineas"][countLinAX]["subCuenta"];
                string numeroPedidoLin = (string)jsonfil["datos"]["lineas"][countLinAX]["numeroPedido"];
                string unidadDeMedidaLin = (string)jsonfil["datos"]["lineas"][countLinAX]["unidadDeMedida"];
                string descuentoLin = (string)jsonfil["datos"]["lineas"][countLinAX]["descuento"];
                string cuotaLin = (string)jsonfil["datos"]["lineas"][countLinAX]["cuota"];
                string impuestoConcretoImpNombLin = (string)jsonfil["datos"]["lineas"][countLinAX]["impuestoConcreto"]["impuesto"]["nombre"];
                string impuestoConcretoNombLin = (string)jsonfil["datos"]["lineas"][countLinAX]["impuestoConcreto"]["nombre"];
                string impuestoConcretoValorLin = (string)jsonfil["datos"]["lineas"][countLinAX]["impuestoConcreto"]["valor"];
                string esTangibleLin = (string)jsonfil["datos"]["lineas"][countLinAX]["esTangible"];
                string importeBrutoLin = (string)jsonfil["datos"]["lineas"][countLinAX]["importeBruto"];
                //string numLin = (string)jsonfil["datos"]["lineas"][countLinAX]["numero"];     //Retenciones ???
                string codigoSedeLin = (string)jsonfil["datos"]["lineas"][countLinAX]["codigoSede"];
                string nombreSedeLin = (string)jsonfil["datos"]["lineas"][countLinAX]["nombreSede"];
                //Insertar línea en BD teniendo en cuenta las claves primarias de la cabecera
                countLinAX += 1;
            }
            //fin lineas
            //lineasImpuestos
            items = (JArray)jsonfil["datos"]["lineasImpuestos"];
            countLineas = items.Count;
            countLinAX = 0;
            while (countLineas > countLinAX)
            {
                string nombreImpuestoImpLin = (string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["nombreImpuesto"];
                string porcentajeImpuestoImpLin = (string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["porcentajeImpuesto"];

                string totalLineaImpuestoImpLin = (string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["totalLineaImpuesto"];
                string cuotaImpLin = (string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["cuota"];
                string baseImponibleImpLin = (string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["baseImponible"];
                string facturaALaQuePerteneceImpLin = (string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["facturaALaQuePertenece"];
                //Insertar lineasImpuestos en BD teniendo en cuenta las claves primarias de la cabecera
                countLinAX += 1;
            }
            //fin lineasImpuestos
            //recibos
            items = (JArray)jsonfil["datos"]["recibos"];
            countLineas = items.Count;
            countLinAX = 0;
            while (countLineas > countLinAX)
            {
                string idRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["id"];
                string numeroFacturaRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["numeroFactura"];
                string cobradoRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["cobrado"];
                string devueltoRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["devuelto"];
                string metodoPagoRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["metodoPago"];
                string fechaEmisionRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["fechaEmision"];
                string fechaDevolucionRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["fechaDevolucion"];
                string fechaVencimientoRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["fechaVencimiento"];
                string importeRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["importe"];
                string importeSinRetencionesRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["importeSinRetenciones"];
                string notasRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["notas"];
                string importeFacturaRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["importeFactura"];
                string fechaPagoRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["fechaPago"];
                string cuentaBancariaRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancaria"];
                string codigoBancoRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["codigoBanco"];
                string ibanRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["iban"];
                string codigoSwiftRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["codigoSwift"];
                string nombreRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["nombre"];
                string activaRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["activa"];
                string cuentacontableRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["cuentaContable"];

                string nombreRecibCClibGenDto = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["nombre"];
                string prefijoIbanRecibCClibGenDto = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["prefijoIban"];
                string codigoSwiftRecibCClibGenDto = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["codigoSwift"];
                string cifCClibGenDto = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["cif"];
                string idRecibCClibGenDto = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["id"];
                string empresaIdRecibCClibGenDto = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["empresaId"];
                string parentIdRecibCClibGenDto = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["parentId"];
                string parentCodeRecibCClibGenDto = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["parentCode"];

                string idRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["id"];
                string empresaIdRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["empresaId"];
                string parentIdRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["parentId"];
                string parentCodeRecibCCli = (string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["parentCode"];

                string subcuentaClienteRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["subcuentaCliente"];
                string codigoClienteRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["codigoCliente"];
                string nombreClienteRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["nombreCliente"];
                string codigoDelegacionRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["codigoDelegacion"];
                string numeroIdentificacionClienteRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["numeroIdentificacionCliente"];
                string tipoTarjetaRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["tipoTarjeta"];
                string formaCobroRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["id"];
                string nDiasRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["nDias"];
                string ordenRecib = (string)jsonfil["datos"]["recibos"][countLinAX]["orden"];
                //Insertar recibos en BD teniendo en cuenta las claves primarias de la cabecera
                countLinAX += 1;
            }
            //fin recibos
            //datosBaseEmpresa
            string iddatBasEmp = jsonControl(jsonfil, "datos", 3, "datosBaseEmpresa", "id");
            string cifdatBasEmp = jsonControl(jsonfil, "datos", 3, "datosBaseEmpresa", "cif");
            string nombredatBasEmp = jsonControl(jsonfil, "datos", 3, "datosBaseEmpresa", "nombre");
            string dominiodatBasEmp = jsonControl(jsonfil, "datos", 3, "datosBaseEmpresa", "dominio");
            //fin datosBaseEmpresa
            string fechaInicioPeriodoFacturacion = jsonControl(jsonfil, "datos", 2, "fechaInicioPeriodoFacturacion");
            string fechaFinPeriodoFacturacion = jsonControl(jsonfil, "datos", 2, "fechaFinPeriodoFacturacion");

            string puntoDeVenta = jsonControl(jsonfil, "datos", 2, "puntoDeVenta");
            string renovablePuntual = jsonControl(jsonfil, "datos", 2, "renovablePuntual");
            string frecuenciaFacturacion = jsonControl(jsonfil, "datos", 2, "frecuenciaFacturacion");
            string formaFacturacion = jsonControl(jsonfil, "datos", 2, "formaFacturacion");
            string numeroOrdenesPrevistas = jsonControl(jsonfil, "datos", 2, "numeroOrdenesPrevistas");
            string seLeAplicanRetenciones = jsonControl(jsonfil, "datos", 2, "seLeAplicanRetenciones");
            string codigoMotivoAnulacion = jsonControl(jsonfil, "datos", 2, "codigoMotivoAnulacion");
            string cbuArgentina = jsonControl(jsonfil, "datos", 2, "cbuArgentina");
            string referenciaTransferencia = jsonControl(jsonfil, "datos", 2, "referenciaTransferencia");
            string identificadorEnSistemaTimbrado = jsonControl(jsonfil, "datos", 2, "identificadorEnSistemaTimbrado");
            string hashSaft = jsonControl(jsonfil, "datos", 2, "hashSaft");
            string permalink = jsonControl(jsonfil, "datos", 2, "permalink");
            string codigoDivisa = jsonControl(jsonfil, "datos", 2, "codigoDivisa");
            string pieFactura = jsonControl(jsonfil, "datos", 2, "pieFactura");
            string numeroDePedido = jsonControl(jsonfil, "datos", 2, "numeroDePedido");
            string configuracionTimbradoPorFicheroGenericoDto = jsonControl(jsonfil, "datos", 2, "configuracionTimbradoPorFicheroGenericoDto");
            string configuracionTimbradoPorTokenGenericoDto = jsonControl(jsonfil, "datos", 2, "configuracionTimbradoPorTokenGenericoDto");
            string idDatBadDel = jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion","id");
            string nombreDatBadDel = jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "nombre");
            string codigoDatBadDel = jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "codigo");
            string cifDatBadDel = jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "cif");

            string numeroPedido = jsonControl(jsonfil, "datos", 2, "numeroPedido");
            string nDiasEmisionRecibo1 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo1");
            string nDiasEmisionRecibo2 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo2");
            string nDiasEmisionRecibo3 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo3");
            string nDiasEmisionRecibo4 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo4");
            string nDiasEmisionRecibo5 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo5");
            string nDiasEmisionRecibo6 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo6");
            string nDiasEmisionRecibo7 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo7");
            string nDiasEmisionRecibo8 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo8");
            string nDiasEmisionRecibo9 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo9");
            string nDiasEmisionRecibo10 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo10");
            string nDiasEmisionRecibo11 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo11");
            string nDiasEmisionRecibo12 = jsonControl(jsonfil, "datos", 2, "nDiasEmisionRecibo12");
            string estaCobrada = jsonControl(jsonfil, "datos", 2, "estaCobrada");
            string metodoCobroEfectivo = jsonControl(jsonfil, "datos", 2, "metodoCobroEfectivo");
            string anulada = jsonControl(jsonfil, "datos", 2, "anulada");
            string facturaPDF = jsonControl(jsonfil, "datos", 2, "facturaPDF");
            string id = jsonControl(jsonfil, "datos", 2, "id");
            string empresaId = jsonControl(jsonfil, "datos", 2, "empresaId");
            string parentId = jsonControl(jsonfil, "datos", 2, "parentId");
            string parentCode = jsonControl(jsonfil, "datos", 2, "parentCode");

            return "OK#";
        }

        private static string jsonControl(JObject jsonfil, string codClave, int numClaves = 1, string codClave2 = "", string codClave3 = "", string codClave4 = "")
        {
            string resultado = "";
            try
            {
                if(numClaves == 4)
                {
                    resultado = (string)jsonfil[codClave][codClave2][codClave3][codClave4];
                }
                else if(numClaves == 3)
                {
                    resultado = (string)jsonfil[codClave][codClave2][codClave3];
                }
                else if (numClaves == 2)
                {
                    resultado = (string)jsonfil[codClave][codClave2];
                }
                else if (numClaves == 1)
                {
                    resultado = (string)jsonfil[codClave];
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
            if (resultado == null) { resultado = ""; }
            return resultado;
        }

        

    }
}

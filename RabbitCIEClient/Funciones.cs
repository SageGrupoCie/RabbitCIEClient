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
                                resulprocesa = procesaFACTURACli(comando, jsonfil, empresaSAGE, ordenFic);
                                break;
                            case "PROVEEDOR":
                                resulprocesa = procesaPROVEEDOR(comando, jsonfil, empresaSAGE, ordenFic);
                                break;
                            case "COMPRA":
                                resulprocesa = procesaFACTURACli(comando, jsonfil, empresaSAGE, ordenFic);
                                break;
                        }

                    }
                }
                catch(Exception ex)
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
            return "OK#";
        }
        private static string procesaPROVEEDOR(string comando, JObject jsonfil, int empSAGE, int ordenFic)
        {
            // Obtenemos claves primarias para tabla de SAGE
            string codProveedor = jsonControl(jsonfil, "datos", 2, "codigo");

            // FIN CLAVES PRIVAMRIAS
            if (codProveedor == "") { return "ERROR#Faltan datos de clave primaria"; }

            List<String> lista = new List<String>();
            //INSERTAMOS PRIMERO LAS CLAVES PRIMARIAS
            lista.Add(empSAGE.ToString());
            lista.Add(codProveedor);
            lista.Add(ordenFic.ToString());
            // FIN INSERCION CLAVES PRIMARIAS
            lista.Add(jsonControl(jsonfil, "datos", 2, "emailFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoTerminoPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoFormaPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "diasPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "diasEmisionRecibo"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "cuentas"));

            if (jsonfil["datos"]["cliente"]["cuentas"].HasValues)
            {       // DEL ARRAY DE CUENTAS SOLO COGEMOS EL PRIMERO, SI EXISTE
                //cuentas
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["codigoBanco"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["iban"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["swiftCode"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["nombre"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["activa"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["cuentaContable"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["nombre"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["prefijoIban"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["codigoSwift"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["cif"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["id"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["empresaId"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["parentId"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["parentCode"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["id"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["empresaId"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["parentId"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["parentCode"]);
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
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
            }
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
            //lista.Add(jsonControl(jsonfil, "datos", 2, "parentCode"));

            BaseDatos bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
            if (bd.estaConectado())
            {
                string indicesNumericos = ",2,";
                bd.InsertarDatos(lista, indicesNumericos, "CieTmpClienteIGEO");
                bd.desConectarBD();
            }



            return "OK#";
        }
        private static string procesaCLIENTE(string comando, JObject jsonfil, int empSAGE, int ordenFic)
        {
            // Obtenemos claves primarias para tabla de SAGE
            string codCliente = jsonControl(jsonfil, "datos",2,"codigo");

            // FIN CLAVES PRIVAMRIAS
            if (codCliente == "") { return "ERROR#Faltan datos de clave primaria"; }

            List<String> lista = new List<String>();
            //INSERTAMOS PRIMERO LAS CLAVES PRIMARIAS
            lista.Add(empSAGE.ToString());
            lista.Add(codCliente);
            lista.Add(ordenFic.ToString());
            // FIN INSERCION CLAVES PRIMARIAS
            lista.Add(jsonControl(jsonfil, "datos", 2, "emailFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoTerminoPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoFormaPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "diasPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "diasEmisionRecibo"));
            //lista.Add(jsonControl(jsonfil, "datos", 2, "cuentas"));
            JArray items;
            try
            {
                items = (JArray)jsonfil["datos"]["cliente"]["cuentas"];
            }
            catch
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
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
            }
            
            int countLineas = items.Count;
            if (countLineas > 0)
            {       // DEL ARRAY DE CUENTAS SOLO COGEMOS EL PRIMERO, SI EXISTE
                    //cuentas
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["codigoBanco"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["iban"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["swiftCode"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["nombre"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["activa"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["cuentaContable"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["nombre"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["prefijoIban"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["codigoSwift"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["cif"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["id"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["empresaId"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["parentId"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["bancoGenericoDto"]["parentCode"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["id"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["empresaId"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["parentId"]);
                lista.Add((string)jsonfil["datos"]["cliente"]["cuentas"].First["parentCode"]);
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
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
                lista.Add("");
            }
            
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
            //lista.Add(jsonControl(jsonfil, "datos", 2, "parentCode"));

            BaseDatos bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
            if(bd.estaConectado())
            {
                string indicesNumericos = ",2,";
                bd.InsertarDatos(lista, indicesNumericos, "CieTmpClienteIGEO");
                bd.desConectarBD();
            }



            return "OK#";
        }

        private static string procesaFACTURACli(string comando, JObject jsonfil, int empSAGE, int ordenFic)
        {
            BaseDatos bd = new BaseDatos(xServidor, xDataBase, xUser, xPass);
            List<String> lista = new List<String>();
            //CLAVES PRIMARIAS
            lista.Add(empSAGE.ToString());
            lista.Add(jsonControl(jsonfil, "datos", 2, "numeroFactura"));
            lista.Add(ordenFic.ToString());
            //FIN CLAVES PRIMARIAS
            lista.Add(jsonControl(jsonfil, "datos", 2, "cif"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "numero"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaRegistro"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaEmision"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaVencimiento"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "facturaANombreDe"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "facturaANumeroCliente"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "importeSinImpuestos"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "importe"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "formaCobro"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "metodoPago"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "numeroFacturaCorregida"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "nombreLineaNegocio"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoLineaNegocio"));
            //datosFacturacionEmisor
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "nombreFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "codigoIdentificacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "direccion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "codigoPostal"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "localidad"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "provincia"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "codigoProvincia"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "alfa2codepais"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "pais"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "comuna"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionEmisor", "regimenFiscal"));
            //fin datosFacturacionEmisor
            //datosFacturacionReceptor
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "nombreFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "codigoIdentificacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "direccion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "codigoPostal"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "localidad"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "provincia"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "codigoProvincia"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "alfa2codepais"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "pais"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "comuna"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosFacturacionReceptor", "regimenFiscal"));
            //fin datosFacturacionReceptor
            lista.Add(jsonControl(jsonfil, "datos", 2, "comuna"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "acteco"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "simulaUnicaLineaFactura"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "descripcionLineaUnica"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "tipoFactura"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "folioUUID"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "datosPreSellado"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "datosPostSellado"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "datosSelladoOriginal"));
            //contrato
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "numero"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "delegacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "fechaInicioPeriodoFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "fechaFinPeriodoFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "fechaInicio"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "fechaFin"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "codigoIdentificacionContratacionPublica"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "codigoProyecto"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "numeroDePedido"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "empresaId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "parentId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "contrato", "parentCode"));
            //fin contrato
            //venta
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "numeroVenta"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "fechaVenta"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "delegacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "fechaInicioPeriodoFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "fechaFinPeriodoFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "numeroDePedido"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "empresaId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "parentId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "venta", "parentCode"));
            //fin venta
            //cuentaCliente
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "codigoBanco"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "iban"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "swiftCode"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "activa"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "cuentaContable"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "prefijoIban"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "codigoSwift"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "cif"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "empresaId"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "parentId"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "cuentaCliente", "bancoGenericoDto", "parentCode"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "empresaId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "parentId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "cuentaCliente", "parentCode"));
            //fin cuentaCliente
            //cliente
            string clientecodigo = jsonControl(jsonfil, "datos", 3, "cliente", "codigo");
            //fin cliente
            //serieFacturacion
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "empresaId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "contador"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "prefijo"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "sufijo"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "letra"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "esParaRectificativas"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "impuestoConcretoForzado"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "esPrehistorica"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "esLaPrincipal"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "serieFacturacionRectificativaId"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "reseteoAnual"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "timbradoConIgeo"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "tipoFacturas"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "esParaFacturasCreditoAR"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "serieFacturacion", "identificadorEnSistemaTimbrado"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "serieFacturacion", "datosBaseDelegacion", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "serieFacturacion", "datosBaseDelegacion", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "serieFacturacion", "datosBaseDelegacion", "codigo"));
            lista.Add(jsonControl(jsonfil, "datos", 4, "serieFacturacion", "datosBaseDelegacion", "cif"));
            //fin serieFacturacion
            //int contLineas = jsonfil["datos"]["lineas"].co;
            //lineas
            JArray items = (JArray)jsonfil["datos"]["lineas"];
            int countLineas = items.Count;
            int countLinAX = 0;
            while(countLineas > countLinAX)
            {
                List<String> listaLineas = new List<String>();
                //CLAVES PRIMARIAS
                listaLineas.Add(empSAGE.ToString());
                listaLineas.Add(jsonControl(jsonfil, "datos", 2, "numeroFactura"));
                listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["numero"]);
                listaLineas.Add(ordenFic.ToString());
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
                listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["impuestoConcreto"]["impuesto"]["nombre"]);
                listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["impuestoConcreto"]["nombre"]);
                listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["impuestoConcreto"]["valor"]);
                listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["esTangible"]);
                listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["importeBruto"]);
                //listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["numero"]);     //Retenciones ???
                listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["codigoSede"]);
                listaLineas.Add((string)jsonfil["datos"]["lineas"][countLinAX]["nombreSede"]);
                //Insertar línea en BD teniendo en cuenta las claves primarias de la cabecera
                if (bd.estaConectado())
                {
                    string indicesNumericos = ",3,";
                    bd.InsertarDatos(listaLineas, indicesNumericos,"tablalineasalbaran");
                    bd.desConectarBD();
                }
                countLinAX += 1;
            }
            //fin lineas
            //lineasImpuestos
            items = (JArray)jsonfil["datos"]["lineasImpuestos"];
            countLineas = items.Count;
            countLinAX = 0;
            while (countLineas > countLinAX)
            {
                List<String> listaLineas = new List<String>();
                //CLAVES PRIMARIAS
                listaLineas.Add(empSAGE.ToString());
                listaLineas.Add((string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["facturaALaQuePertenece"]);
                listaLineas.Add((string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["nombreImpuesto"]);
                listaLineas.Add(ordenFic.ToString());
                //FIN CLAVES PRIMARIAS
                listaLineas.Add((string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["porcentajeImpuesto"]);
                listaLineas.Add((string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["totalLineaImpuesto"]);
                listaLineas.Add((string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["cuota"]);
                listaLineas.Add((string)jsonfil["datos"]["lineasImpuestos"][countLinAX]["baseImponible"]);
                //Insertar lineasImpuestos en BD teniendo en cuenta las claves primarias de la cabecera
                if (bd.estaConectado())
                {
                    string indicesNumericos = ",3,";
                    bd.InsertarDatos(listaLineas, indicesNumericos, "tablalineasalbaranImpuestos");
                    bd.desConectarBD();
                }
                countLinAX += 1;
            }
            //fin lineasImpuestos
            //recibos
            items = (JArray)jsonfil["datos"]["recibos"];
            countLineas = items.Count;
            countLinAX = 0;
            while (countLineas > countLinAX)
            {
                List<String> listaLineas = new List<String>();
                //CLAVES PRIMARIAS
                listaLineas.Add(empSAGE.ToString());
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["numeroFactura"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["id"]);
                listaLineas.Add(ordenFic.ToString());
                //FIN CLAVES PRIMARIAS
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cobrado"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["devuelto"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["metodoPago"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["fechaEmision"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["fechaDevolucion"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["fechaVencimiento"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["importe"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["importeSinRetenciones"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["notas"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["importeFactura"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["fechaPago"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancaria"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["codigoBanco"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["iban"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["codigoSwift"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["nombre"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["activa"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["cuentaContable"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["nombre"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["prefijoIban"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["codigoSwift"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["cif"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["id"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["empresaId"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["parentId"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["bancoGenericoDto"]["parentCode"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["id"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["empresaId"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["parentId"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["cuentaBancariaCliente"]["parentCode"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["subcuentaCliente"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["codigoCliente"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["nombreCliente"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["codigoDelegacion"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["numeroIdentificacionCliente"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["tipoTarjeta"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["id"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["nDias"]);
                listaLineas.Add((string)jsonfil["datos"]["recibos"][countLinAX]["orden"]);
                //Insertar recibos en BD teniendo en cuenta las claves primarias de la cabecera
                if (bd.estaConectado())
                {
                    string indicesNumericos = ",3,";
                    bd.InsertarDatos(listaLineas, indicesNumericos, "tablalineasalbaranRecibos");
                    bd.desConectarBD();
                }
                countLinAX += 1;
            }
            //fin recibos
            //datosBaseEmpresa
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseEmpresa", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseEmpresa", "cif"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseEmpresa", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseEmpresa", "dominio"));
            //fin datosBaseEmpresa
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaInicioPeriodoFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "fechaFinPeriodoFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "puntoDeVenta"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "renovablePuntual"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "frecuenciaFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "formaFacturacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "numeroOrdenesPrevistas"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "seLeAplicanRetenciones"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoMotivoAnulacion"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "cbuArgentina"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "referenciaTransferencia"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "identificadorEnSistemaTimbrado"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "hashSaft"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "permalink"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "codigoDivisa"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "pieFactura"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "numeroDePedido"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "configuracionTimbradoPorFicheroGenericoDto"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "configuracionTimbradoPorTokenGenericoDto"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "id"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "nombre"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "codigo"));
            lista.Add(jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "cif"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "numeroPedido"));
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
            lista.Add(jsonControl(jsonfil, "datos", 2, "estaCobrada"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "metodoCobroEfectivo"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "anulada"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "facturaPDF"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "id"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "empresaId"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "parentId"));
            lista.Add(jsonControl(jsonfil, "datos", 2, "parentCode"));
            if (bd.estaConectado())
            {
                string indicesNumericos = ",2,";
                bd.InsertarDatos(lista, indicesNumericos, "tablaCabeceraAlbaran");
                bd.desConectarBD();
            }
            countLinAX += 1;
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

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

            string emailFacturacion = (string)jsonfil["datos"]["emailFacturacion"];
            string codigoTerminoPago = (string)jsonfil["datos"]["codigoTerminoPago"];
            string codigoFormaPago = (string)jsonfil["datos"]["codigoFormaPago"];
            string diasPago = (string)jsonfil["datos"]["diasPago"];
            string diasEmisionRecibo = (string)jsonfil["datos"]["diasEmisionRecibo"];
            string cuentas = (string)jsonfil["datos"]["cuentas"];
            string fechaAlta = (string)jsonfil["datos"]["fechaAlta"];
            string tipoCliente = (string)jsonfil["datos"]["tipoCliente"];
            string nombre = (string)jsonfil["datos"]["nombre"];
            string apellidos = (string)jsonfil["datos"]["apellidos"];
            string observaciones = (string)jsonfil["datos"]["observaciones"];
            string fechaAltaPotencial = (string)jsonfil["datos"]["fechaAltaPotencial"];
            string numeroDeCliente = (string)jsonfil["datos"]["numeroDeCliente"];
            string codigoSecundario = (string)jsonfil["datos"]["codigoSecundario"];
            string comoNosHaConocido = (string)jsonfil["datos"]["comoNosHaConocido"];
            string detallesComoNosHaConocido = (string)jsonfil["datos"]["detallesComoNosHaConocido"];
            string subCuenta = (string)jsonfil["datos"]["subCuenta"];
            string url = (string)jsonfil["datos"]["url"];
            string estado = (string)jsonfil["datos"]["estado"];
            string motivoInactivo = (string)jsonfil["datos"]["motivoInactivo"];
            string importadoDesde = (string)jsonfil["datos"]["importadoDesde"];
            string nombreEnvio = (string)jsonfil["datos"]["nombreEnvio"];
            string direccionEnvio = (string)jsonfil["datos"]["direccionEnvio"];
            string distritoEnvio = (string)jsonfil["datos"]["distritoEnvio"];
            string codigoPostalEnvio = (string)jsonfil["datos"]["codigoPostalEnvio"];
            string localidadEnvio = (string)jsonfil["datos"]["localidadEnvio"];
            string nombrePaisEnvio = (string)jsonfil["datos"]["nombrePaisEnvio"];
            string telefonoEnvio = (string)jsonfil["datos"]["telefonoEnvio"];
            string codigoProvinciaEnvio = (string)jsonfil["datos"]["codigoProvinciaEnvio"];
            string nombreProvinciaEnvio = (string)jsonfil["datos"]["nombreProvinciaEnvio"];
            string idProvinciaEnvio = (string)jsonfil["datos"]["idProvinciaEnvio"];
            string atencionAEnvio = (string)jsonfil["datos"]["atencionAEnvio"];
            string observacionesFacturacion = (string)jsonfil["datos"]["observacionesFacturacion"];
            string datosImportacion = (string)jsonfil["datos"]["datosImportacion"];
            string emailsDondeEnviarFacturas = (string)jsonfil["datos"]["emailsDondeEnviarFacturas"];
            string riesgoEconomico = (string)jsonfil["datos"]["riesgoEconomico"];
            string sepaFirmado = (string)jsonfil["datos"]["sepaFirmado"];
            string coste = (string)jsonfil["datos"]["coste"];
            string rentabilidad = (string)jsonfil["datos"]["rentabilidad"];
            string beneficio = (string)jsonfil["datos"]["beneficio"];
            string totalVentas = (string)jsonfil["datos"]["totalVentas"];
            string formaCobro = (string)jsonfil["datos"]["formaCobro"];
            string diasDePago = (string)jsonfil["datos"]["diasDePago"];
            string diasDeFacturacion = (string)jsonfil["datos"]["diasDeFacturacion"];
            string fechaPuntualFactura = (string)jsonfil["datos"]["fechaPuntualFactura"];
            string fechaEmisionPuntual = (string)jsonfil["datos"]["fechaEmisionPuntual"];
            string variasFechasFactura = (string)jsonfil["datos"]["variasFechasFactura"];
            string nDiasEmisionRecibo1 = (string)jsonfil["datos"]["nDiasEmisionRecibo1"];
            string nDiasEmisionRecibo2 = (string)jsonfil["datos"]["nDiasEmisionRecibo2"];
            string nDiasEmisionRecibo3 = (string)jsonfil["datos"]["nDiasEmisionRecibo3"];
            string nDiasEmisionRecibo4 = (string)jsonfil["datos"]["nDiasEmisionRecibo4"];
            string nDiasEmisionRecibo5 = (string)jsonfil["datos"]["nDiasEmisionRecibo5"];
            string nDiasEmisionRecibo6 = (string)jsonfil["datos"]["nDiasEmisionRecibo6"];
            string nDiasEmisionRecibo7 = (string)jsonfil["datos"]["nDiasEmisionRecibo7"];
            string nDiasEmisionRecibo8 = (string)jsonfil["datos"]["nDiasEmisionRecibo8"];
            string nDiasEmisionRecibo9 = (string)jsonfil["datos"]["nDiasEmisionRecibo9"];
            string nDiasEmisionRecibo10 = (string)jsonfil["datos"]["nDiasEmisionRecibo10"];
            string nDiasEmisionRecibo11 = (string)jsonfil["datos"]["nDiasEmisionRecibo11"];
            string nDiasEmisionRecibo12 = (string)jsonfil["datos"]["nDiasEmisionRecibo12"];
            string permiteValidarCertificados = (string)jsonfil["datos"]["permiteValidarCertificados"];
            string codigoComoProveedor = (string)jsonfil["datos"]["codigoComoProveedor"];
            string contadorNumeroDeCliente = (string)jsonfil["datos"]["contadorNumeroDeCliente"];
            string emailsDondeEnviarPresupuestos = (string)jsonfil["datos"]["emailsDondeEnviarPresupuestos"];
            string formaFacturacion = (string)jsonfil["datos"]["formaFacturacion"];
            string prorrateoFacturasFechasContrato = (string)jsonfil["datos"]["prorrateoFacturasFechasContrato"];
            string generarFacturasAPlazoVencido = (string)jsonfil["datos"]["generarFacturasAPlazoVencido"];
            string numeroFacturasAEmitir = (string)jsonfil["datos"]["numeroFacturasAEmitir"];
            string requiereLlevarFacturasImpresos = (string)jsonfil["datos"]["requiereLlevarFacturasImpresos"];
            string esIntracomunitario = (string)jsonfil["datos"]["esIntracomunitario"];
            string zonaComercial = (string)jsonfil["datos"]["zonaComercial"];
            string codigoZonaComercial = (string)jsonfil["datos"]["codigoZonaComercial"];
            string fechaBaja = (string)jsonfil["datos"]["fechaBaja"];
            string perdidoCompetidor = (string)jsonfil["datos"]["perdidoCompetidor"];
            string tcNecesarios = (string)jsonfil["datos"]["tcNecesarios"];
            string codigosClasificacion = (string)jsonfil["datos"]["codigosClasificacion"];
            string nombresClasificacion = (string)jsonfil["datos"]["nombresClasificacion"];
            string grupoContableClienteCodigo = (string)jsonfil["datos"]["grupoContableClienteCodigo"];
            string grupoContableClienteCuentaContableCliente = (string)jsonfil["datos"]["grupoContableClienteCuentaContableCliente"];
            string grupoContableNegocioCodigo = (string)jsonfil["datos"]["grupoContableNegocioCodigo"];
            string grupoContableNegocioDescripcion = (string)jsonfil["datos"]["grupoContableNegocioDescripcion"];
            string grupoRegistroIVACodigo = (string)jsonfil["datos"]["grupoRegistroIVACodigo"];
            string grupoRegistroIVADescripcion = (string)jsonfil["datos"]["grupoRegistroIVADescripcion"];
            string sectorCodigo = (string)jsonfil["datos"]["sectorCodigo"];
            string sectorNombre = (string)jsonfil["datos"]["sectorNombre"];
            string actividad = (string)jsonfil["datos"]["actividad"];
            string codigoActividad = (string)jsonfil["datos"]["codigoActividad"];
            string codigoDelegacion = (string)jsonfil["datos"]["codigoDelegacion"];

            return "";
        }

        private static string procesaFACTURA(string comando, JObject jsonfil, int empSAGE)
        {
            return "";
        }

    }
}

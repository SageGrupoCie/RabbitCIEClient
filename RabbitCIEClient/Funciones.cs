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
            foreach (string nombreFilePath in Directory.EnumerateFiles(folderPath, "RabMQCIE_*.txt"))
            {   //recorremos todos los arhivos de la carpeta
                string nombreFile = Path.GetFileName(nombreFilePath);
                string[] arraOrden = nombreFile.Split('_');
                int ordenFic = int.Parse(arraOrden[1].Substring(2, arraOrden[1].Length-2) + arraOrden[2].Substring(0,arraOrden[2].Length-4));
                string readText = File.ReadAllText(nombreFilePath);
                JObject jsonfil = JObject.Parse(readText);
                string tipo = jsonControl(jsonfil, "claseEntidadIgeo");
                string resulprocesa = "#";
                int empresaSAGE = Int32.Parse(Funciones.obtenerValoresIni("EMPRESA_SAGE"));
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



            string emailFacturacion = jsonControl(jsonfil, "datos", 2, "emailFacturacion");
            string codigoTerminoPago = jsonControl(jsonfil, "datos", 2, "codigoTerminoPago");
            string codigoFormaPago = jsonControl(jsonfil, "datos", 2, "codigoFormaPago");
            string diasPago = jsonControl(jsonfil, "datos", 2, "diasPago");
            string diasEmisionRecibo = jsonControl(jsonfil, "datos", 2, "diasEmisionRecibo");
            string cuentas = jsonControl(jsonfil, "datos", 2, "cuentas");
            string fechaAlta = jsonControl(jsonfil, "datos", 2, "fechaAlta");
            string tipoCliente = jsonControl(jsonfil, "datos", 2, "tipoCliente");
            string nombre = jsonControl(jsonfil, "datos", 2, "nombre");
            string apellidos = jsonControl(jsonfil, "datos", 2, "apellidos");
            string observaciones = jsonControl(jsonfil, "datos", 2, "observaciones");
            string fechaAltaPotencial = jsonControl(jsonfil, "datos", 2, "fechaAltaPotencial");
            string numeroDeCliente = jsonControl(jsonfil, "datos", 2, "numeroDeCliente");
            string codigoSecundario = jsonControl(jsonfil, "datos", 2, "codigoSecundario");
            string comoNosHaConocido = jsonControl(jsonfil, "datos", 2, "comoNosHaConocido");
            string detallesComoNosHaConocido = jsonControl(jsonfil, "datos", 2, "detallesComoNosHaConocido");
            string subCuenta = jsonControl(jsonfil, "datos", 2, "subCuenta");
            string url = jsonControl(jsonfil, "datos", 2, "url");
            string estado = jsonControl(jsonfil, "datos", 2, "estado");
            string motivoInactivo = jsonControl(jsonfil, "datos", 2, "motivoInactivo");
            string importadoDesde = jsonControl(jsonfil, "datos", 2, "importadoDesde");
            string nombreEnvio = jsonControl(jsonfil, "datos", 2, "nombreEnvio");
            string direccionEnvio = jsonControl(jsonfil, "datos", 2, "direccionEnvio");
            string distritoEnvio = jsonControl(jsonfil, "datos", 2, "distritoEnvio");
            string codigoPostalEnvio = jsonControl(jsonfil, "datos", 2, "codigoPostalEnvio");
            string localidadEnvio = jsonControl(jsonfil, "datos", 2, "localidadEnvio");
            string nombrePaisEnvio = jsonControl(jsonfil, "datos", 2, "nombrePaisEnvio");
            string telefonoEnvio = jsonControl(jsonfil, "datos", 2, "telefonoEnvio");
            string codigoProvinciaEnvio = jsonControl(jsonfil, "datos", 2, "codigoProvinciaEnvio");
            string nombreProvinciaEnvio = jsonControl(jsonfil, "datos", 2, "nombreProvinciaEnvio");
            string idProvinciaEnvio = jsonControl(jsonfil, "datos", 2, "idProvinciaEnvio");
            string atencionAEnvio = jsonControl(jsonfil, "datos", 2, "atencionAEnvio");
            string observacionesFacturacion = jsonControl(jsonfil, "datos", 2, "observacionesFacturacion");
            string datosImportacion = jsonControl(jsonfil, "datos", 2, "datosImportacion");
            string emailsDondeEnviarFacturas = jsonControl(jsonfil, "datos", 2, "emailsDondeEnviarFacturas");
            string riesgoEconomico = jsonControl(jsonfil, "datos", 2, "riesgoEconomico");
            string sepaFirmado = jsonControl(jsonfil, "datos", 2, "sepaFirmado");
            string coste = jsonControl(jsonfil, "datos", 2, "coste");
            string rentabilidad = jsonControl(jsonfil, "datos", 2, "rentabilidad");
            string beneficio = jsonControl(jsonfil, "datos", 2, "beneficio");
            string totalVentas = jsonControl(jsonfil, "datos", 2, "totalVentas");
            string formaCobro = jsonControl(jsonfil, "datos", 2, "formaCobro");
            string diasDePago = jsonControl(jsonfil, "datos", 2, "diasDePago");
            string diasDeFacturacion = jsonControl(jsonfil, "datos", 2, "diasDeFacturacion");
            string fechaPuntualFactura = jsonControl(jsonfil, "datos", 2, "fechaPuntualFactura");
            string fechaEmisionPuntual = jsonControl(jsonfil, "datos", 2, "fechaEmisionPuntual");
            string variasFechasFactura = jsonControl(jsonfil, "datos", 2, "variasFechasFactura");
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
            string permiteValidarCertificados = jsonControl(jsonfil, "datos", 2, "permiteValidarCertificados");
            string codigoComoProveedor = jsonControl(jsonfil, "datos", 2, "codigoComoProveedor");
            string contadorNumeroDeCliente = jsonControl(jsonfil, "datos", 2, "contadorNumeroDeCliente");
            string emailsDondeEnviarPresupuestos = jsonControl(jsonfil, "datos", 2, "emailsDondeEnviarPresupuestos");
            string formaFacturacion = jsonControl(jsonfil, "datos", 2, "formaFacturacion");
            string prorrateoFacturasFechasContrato = jsonControl(jsonfil, "datos", 2, "prorrateoFacturasFechasContrato");
            string generarFacturasAPlazoVencido = jsonControl(jsonfil, "datos", 2, "generarFacturasAPlazoVencido");
            string numeroFacturasAEmitir = jsonControl(jsonfil, "datos", 2, "numeroFacturasAEmitir");
            string requiereLlevarFacturasImpresos = jsonControl(jsonfil, "datos", 2, "requiereLlevarFacturasImpresos");
            string esIntracomunitario = jsonControl(jsonfil, "datos", 2, "esIntracomunitario");
            string zonaComercial = jsonControl(jsonfil, "datos", 2, "zonaComercial");
            string codigoZonaComercial = jsonControl(jsonfil, "datos", 2, "codigoZonaComercial");
            string fechaBaja = jsonControl(jsonfil, "datos", 2, "fechaBaja");
            string perdidoCompetidor = jsonControl(jsonfil, "datos", 2, "perdidoCompetidor");
            string tcNecesarios = jsonControl(jsonfil, "datos", 2, "tcNecesarios");
            string codigosClasificacion = jsonControl(jsonfil, "datos", 2, "codigosClasificacion");
            string nombresClasificacion = jsonControl(jsonfil, "datos", 2, "nombresClasificacion");
            string grupoContableClienteCodigo = jsonControl(jsonfil, "datos", 2, "grupoContableClienteCodigo");
            string grupoContableClienteCuentaContableCliente = jsonControl(jsonfil, "datos", 2, "grupoContableClienteCuentaContableCliente");
            string grupoContableNegocioCodigo = jsonControl(jsonfil, "datos", 2, "grupoContableNegocioCodigo");
            string grupoContableNegocioDescripcion = jsonControl(jsonfil, "datos", 2, "grupoContableNegocioDescripcion");
            string grupoRegistroIVACodigo = jsonControl(jsonfil, "datos", 2, "grupoRegistroIVACodigo");
            string grupoRegistroIVADescripcion = jsonControl(jsonfil, "datos", 2, "grupoRegistroIVADescripcion");
            string sectorCodigo = jsonControl(jsonfil, "datos", 2, "sectorCodigo");
            string sectorNombre = jsonControl(jsonfil, "datos", 2, "sectorNombre");
            string actividad = jsonControl(jsonfil, "datos", 2, "actividad");
            string codigoActividad = jsonControl(jsonfil, "datos", 2, "codigoActividad");
            string codigoDelegacion = jsonControl(jsonfil, "datos", 2, "codigoDelegacion");
            //gestionadoPor
            string gesPorID = jsonControl(jsonfil, "datos", 3, "gestionadoPor", "id");
            string gesPorNombre = jsonControl(jsonfil, "datos", 3, "gestionadoPor", "nombre");
            string gesPorApellidos = jsonControl(jsonfil, "datos", 3, "gestionadoPor", "apellidos");
            string gesPorAlias = jsonControl(jsonfil, "datos", 3, "gestionadoPor", "alias");
            string gesPorCodIdent = jsonControl(jsonfil, "datos", 3, "gestionadoPor", "codigoIdentificacion");
            string gesPorcodEmpleado = jsonControl(jsonfil, "datos", 3, "gestionadoPor", "codigoEmpleado");
            string codigoGestionadoPor = jsonControl(jsonfil, "datos", 2, "codigoGestionadoPor");
            //fin gestionadoPor
            //idioma
            string idiomaNombre = jsonControl(jsonfil, "datos", 3, "idioma", "nombre");
            string idiomaCodigo = jsonControl(jsonfil, "datos", 3, "idioma", "codigo");
            //fin idioma
            //personaContacto
            string persContactonombre = jsonControl(jsonfil, "datos", 3, "personaContacto", "nombre");
            string persContactomovil = jsonControl(jsonfil, "datos", 3, "personaContacto", "movil");
            string persContactofax = jsonControl(jsonfil, "datos", 3, "personaContacto", "fax");
            string persContactotelefono = jsonControl(jsonfil, "datos", 3, "personaContacto", "telefono");
            string persContactoemail = jsonControl(jsonfil, "datos", 3, "personaContacto", "email");
            string persContactocargo = jsonControl(jsonfil, "datos", 3, "personaContacto", "cargo");
            string persContactoesLaPrincipal = jsonControl(jsonfil, "datos", 3, "personaContacto", "esLaPrincipal");
            string persContactodireccion = jsonControl(jsonfil, "datos", 3, "personaContacto", "direccion");
            string persContactocodigoPostal = jsonControl(jsonfil, "datos", 3, "personaContacto", "codigoPostal");
            string persContactolocalidad = jsonControl(jsonfil, "datos", 3, "personaContacto", "localidad");
            string persContactonumeroFax = jsonControl(jsonfil, "datos", 3, "personaContacto", "numeroFax");
            string persContactoProvincID = jsonControl(jsonfil, "datos", 4, "personaContacto", "provincia","id");
            string persContactoProvincnombre = jsonControl(jsonfil, "datos", 4, "personaContacto", "provincia", "nombre");
            string persContactoProvinccodigo = jsonControl(jsonfil, "datos", 4, "personaContacto", "provincia", "codigo");
            string persContactoProvincpaisID = jsonControl(jsonfil, "datos", 4, "personaContacto", "provincia", "paisId");
            string persContactoID = jsonControl(jsonfil, "datos", 3, "personaContacto", "id");
            string persContactoempresaID = jsonControl(jsonfil, "datos", 3, "personaContacto", "empresaId");
            string persContactoparentID = jsonControl(jsonfil, "datos", 3, "personaContacto", "parentId");
            string persContactoparentCod = jsonControl(jsonfil, "datos", 3, "personaContacto", "parentCode");
            //fin personaContacto
            //datosFacturacion
            string datFacturacnombreFacturacion = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "nombreFacturacion");
            string datFacturaccodigoIdentificacion = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "codigoIdentificacion");
            string datFacturacdireccion = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "direccion");
            string datFacturaccodigoPostal = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "codigoPostal");
            string datFacturaclocalidad = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "localidad");
            string datFacturacprovincia = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "provincia");
            string datFacturaccodigoProvincia = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "codigoProvincia");
            string datFacturacalfa2codepais = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "alfa2codepais");
            string datFacturacpais = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "pais");
            string datFacturaccomuna = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "comuna");
            string datFacturacregimenFiscal = jsonControl(jsonfil, "datos", 3, "datosFacturacion", "regimenFiscal");
            //fin datosFacturacion
            //datosClienteFacturae
            string facturae_activada = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_activada");
            string facturae_canalEnvio = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_canalEnvio");
            string facturae_customerId = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_customerId");
            string facturae_dir3 = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir3");
            string facturae_dir31 = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir31");
            string facturae_dir32 = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir32");
            string facturae_dir33 = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir33");
            string facturae_dir34 = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_dir34");
            string facturae_direccionOficinaContable = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_direccionOficinaContable");
            string facturae_direccionOrganoGestor = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_direccionOrganoGestor");
            string facturae_denominacionUnidadTramitadora = jsonControl(jsonfil, "datos", 3, "datosClienteFacturae", "facturae_denominacionUnidadTramitadora");
            //fin datosClienteFacturae
            string clienteEdi = jsonControl(jsonfil, "datos", 2, "clienteEdi");
            string tipoEDI = jsonControl(jsonfil, "datos", 2, "tipoEDI");
            string tipoPersonaReceptor = jsonControl(jsonfil, "datos", 2, "tipoPersonaReceptor");
            string tipoResidencia = jsonControl(jsonfil, "datos", 2, "tipoResidencia");
            string plataFormaEDI = jsonControl(jsonfil, "datos", 2, "plataFormaEDI");
            string autoFactura = jsonControl(jsonfil, "datos", 2, "autoFactura");
            string subTipoEDI = jsonControl(jsonfil, "datos", 2, "subTipoEDI");
            string infoExtraEDI = jsonControl(jsonfil, "datos", 2, "infoExtraEDI");
            string referenciaContratoReceptorEDI = jsonControl(jsonfil, "datos", 2, "referenciaContratoReceptorEDI");
            string codigoPagador = jsonControl(jsonfil, "datos", 2, "codigoPagador");
            string fechaBloqueo = jsonControl(jsonfil, "datos", 2, "fechaBloqueo");
            string cuentaBancariaPreferida = jsonControl(jsonfil, "datos", 2, "cuentaBancariaPreferida");
            string comuna = jsonControl(jsonfil, "datos", 2, "comuna");
            string acteco = jsonControl(jsonfil, "datos", 2, "acteco");
            string figuraJuridica = jsonControl(jsonfil, "datos", 2, "figuraJuridica");
            string codigoFiguraJuridica = jsonControl(jsonfil, "datos", 2, "codigoFiguraJuridica");
            string codigoIdentificativoDestinatarioFacturaE = jsonControl(jsonfil, "datos", 2, "codigoIdentificativoDestinatarioFacturaE");
            string partidaIva = jsonControl(jsonfil, "datos", 2, "partidaIva");
            string emailNotificacionesCertificadas = jsonControl(jsonfil, "datos", 2, "emailNotificacionesCertificadas");
            //datosBaseDelegacion
            string datBasDelgid = jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "id");
            string datBasDelgnombre = jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "nombre");
            string datBasDelgcodigo = jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "codigo");
            string datBasDelgcif = jsonControl(jsonfil, "datos", 3, "datosBaseDelegacion", "cif");
            //fin datosBaseDelegacion
            string id = jsonControl(jsonfil, "datos", 2, "id");
            string empresaId = jsonControl(jsonfil, "datos", 2, "empresaId");
            string parentId = jsonControl(jsonfil, "datos", 2, "parentId");
            string parentCode = jsonControl(jsonfil, "datos", 2, "parentCode");

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

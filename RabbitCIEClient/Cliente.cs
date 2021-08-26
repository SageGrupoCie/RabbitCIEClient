using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitCIEClient
{
    class Cliente
    {

        public class Rootobject
        {
            public string claseEntidadIgeo { get; set; }
            public int idEntidadIgeo { get; set; }
            public string codigoEntidadIgeo { get; set; }
            public object parentId { get; set; }
            public object parentCode { get; set; }
            public string comando { get; set; }
            public string fecha { get; set; }
            public Datos datos { get; set; }
        }

        public class Datos
        {
            public string codigo { get; set; }
            public object emailFacturacion { get; set; }
            public object codigoTerminoPago { get; set; }
            public object codigoFormaPago { get; set; }
            public object diasPago { get; set; }
            public object diasEmisionRecibo { get; set; }
            public object[] cuentas { get; set; }
            public string fechaAlta { get; set; }
            public string tipoCliente { get; set; }
            public string nombre { get; set; }
            public string apellidos { get; set; }
            public string observaciones { get; set; }
            public object fechaAltaPotencial { get; set; }
            public string numeroDeCliente { get; set; }
            public string codigoSecundario { get; set; }
            public string comoNosHaConocido { get; set; }
            public string detallesComoNosHaConocido { get; set; }
            public string subCuenta { get; set; }
            public string url { get; set; }
            public string estado { get; set; }
            public object motivoInactivo { get; set; }
            public string importadoDesde { get; set; }
            public string nombreEnvio { get; set; }
            public string direccionEnvio { get; set; }
            public string distritoEnvio { get; set; }
            public string codigoPostalEnvio { get; set; }
            public string localidadEnvio { get; set; }
            public string nombrePaisEnvio { get; set; }
            public string telefonoEnvio { get; set; }
            public string codigoProvinciaEnvio { get; set; }
            public string nombreProvinciaEnvio { get; set; }
            public int idProvinciaEnvio { get; set; }
            public string atencionAEnvio { get; set; }
            public string observacionesFacturacion { get; set; }
            public object datosImportacion { get; set; }
            public string emailsDondeEnviarFacturas { get; set; }
            public object riesgoEconomico { get; set; }
            public bool sepaFirmado { get; set; }
            public float coste { get; set; }
            public float rentabilidad { get; set; }
            public float beneficio { get; set; }
            public float totalVentas { get; set; }
            public object formaCobro { get; set; }
            public string diasDePago { get; set; }
            public string diasDeFacturacion { get; set; }
            public object fechaPuntualFactura { get; set; }
            public object fechaEmisionPuntual { get; set; }
            public object variasFechasFactura { get; set; }
            public object nDiasEmisionRecibo1 { get; set; }
            public object nDiasEmisionRecibo2 { get; set; }
            public object nDiasEmisionRecibo3 { get; set; }
            public object nDiasEmisionRecibo4 { get; set; }
            public object nDiasEmisionRecibo5 { get; set; }
            public object nDiasEmisionRecibo6 { get; set; }
            public object nDiasEmisionRecibo7 { get; set; }
            public object nDiasEmisionRecibo8 { get; set; }
            public object nDiasEmisionRecibo9 { get; set; }
            public object nDiasEmisionRecibo10 { get; set; }
            public object nDiasEmisionRecibo11 { get; set; }
            public object nDiasEmisionRecibo12 { get; set; }
            public object permiteValidarCertificados { get; set; }
            public string codigoComoProveedor { get; set; }
            public object contadorNumeroDeCliente { get; set; }
            public string emailsDondeEnviarPresupuestos { get; set; }
            public object formaFacturacion { get; set; }
            public string prorrateoFacturasFechasContrato { get; set; }
            public bool generarFacturasAPlazoVencido { get; set; }
            public int numeroFacturasAEmitir { get; set; }
            public bool requiereLlevarFacturasImpresos { get; set; }
            public bool esIntracomunitario { get; set; }
            public string zonaComercial { get; set; }
            public object codigoZonaComercial { get; set; }
            public object fechaBaja { get; set; }
            public object perdidoCompetidor { get; set; }
            public bool tcNecesarios { get; set; }
            public object codigosClasificacion { get; set; }
            public object nombresClasificacion { get; set; }
            public object grupoContableClienteCodigo { get; set; }
            public object grupoContableClienteCuentaContableCliente { get; set; }
            public object grupoContableNegocioCodigo { get; set; }
            public object grupoContableNegocioDescripcion { get; set; }
            public object grupoRegistroIVACodigo { get; set; }
            public object grupoRegistroIVADescripcion { get; set; }
            public object sectorCodigo { get; set; }
            public object sectorNombre { get; set; }
            public object actividad { get; set; }
            public object codigoActividad { get; set; }
            public object codigoDelegacion { get; set; }
            public Gestionadopor gestionadoPor { get; set; }
            public object codigoGestionadoPor { get; set; }
            public Idioma idioma { get; set; }
            public object codigoIdioma { get; set; }
            public Datoscontacto datosContacto { get; set; }
            public Personacontacto personaContacto { get; set; }
            public Datosfacturacion datosFacturacion { get; set; }
            public Datosclientefacturae datosClienteFacturae { get; set; }
            public bool clienteEdi { get; set; }
            public object tipoEDI { get; set; }
            public object tipoPersonaReceptor { get; set; }
            public object tipoResidencia { get; set; }
            public object plataFormaEDI { get; set; }
            public bool autoFactura { get; set; }
            public object subTipoEDI { get; set; }
            public object infoExtraEDI { get; set; }
            public object referenciaContratoReceptorEDI { get; set; }
            public object codigoPagador { get; set; }
            public object fechaBloqueo { get; set; }
            public object cuentaBancariaPreferida { get; set; }
            public object comuna { get; set; }
            public object acteco { get; set; }
            public object figuraJuridica { get; set; }
            public object codigoFiguraJuridica { get; set; }
            public object codigoIdentificativoDestinatarioFacturaE { get; set; }
            public object partidaIva { get; set; }
            public object emailNotificacionesCertificadas { get; set; }
            public Datosbasedelegacion datosBaseDelegacion { get; set; }
            public int id { get; set; }
            public int empresaId { get; set; }
            public object parentId { get; set; }
            public object parentCode { get; set; }
        }

        public class Gestionadopor
        {
            public int id { get; set; }
            public string nombre { get; set; }
            public string apellidos { get; set; }
            public object alias { get; set; }
            public string codigoIdentificacion { get; set; }
            public string codigoEmpleado { get; set; }
        }

        public class Idioma
        {
            public object nombre { get; set; }
            public object codigo { get; set; }
        }

        public class Datoscontacto
        {
            public string telefono { get; set; }
            public string movil { get; set; }
            public string email { get; set; }
            public string fax { get; set; }
        }

        public class Personacontacto
        {
            public string nombre { get; set; }
            public object movil { get; set; }
            public object fax { get; set; }
            public string telefono { get; set; }
            public string email { get; set; }
            public string cargo { get; set; }
            public object esLaPrincipal { get; set; }
            public object direccion { get; set; }
            public object codigoPostal { get; set; }
            public object localidad { get; set; }
            public object numeroFax { get; set; }
            public Provincia provincia { get; set; }
            public object id { get; set; }
            public object empresaId { get; set; }
            public int parentId { get; set; }
            public string parentCode { get; set; }
        }

        public class Provincia
        {
            public object id { get; set; }
            public object nombre { get; set; }
            public object codigo { get; set; }
            public object paisId { get; set; }
        }

        public class Datosfacturacion
        {
            public string nombreFacturacion { get; set; }
            public string codigoIdentificacion { get; set; }
            public string direccion { get; set; }
            public string codigoPostal { get; set; }
            public string localidad { get; set; }
            public string provincia { get; set; }
            public string codigoProvincia { get; set; }
            public string alfa2codepais { get; set; }
            public string pais { get; set; }
            public object comuna { get; set; }
            public object regimenFiscal { get; set; }
        }

        public class Datosclientefacturae
        {
            public bool facturae_activada { get; set; }
            public object facturae_canalEnvio { get; set; }
            public object facturae_customerId { get; set; }
            public object facturae_dir3 { get; set; }
            public object facturae_dir31 { get; set; }
            public object facturae_dir32 { get; set; }
            public object facturae_dir33 { get; set; }
            public object facturae_dir34 { get; set; }
            public object facturae_direccionOficinaContable { get; set; }
            public object facturae_direccionOrganoGestor { get; set; }
            public object facturae_denominacionUnidadTramitadora { get; set; }
        }

        public class Datosbasedelegacion
        {
            public object id { get; set; }
            public string nombre { get; set; }
            public string codigo { get; set; }
            public string cif { get; set; }
        }
    }
}







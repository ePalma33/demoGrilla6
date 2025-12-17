namespace demoGrilla6.Models
{
    public class Factura
    {
        public DateTime FechaDocumento { get; set; }
        public string CorrelativoInterno { get; set; }
        public string TipoDocumento { get; set; }
        public string CuentaFacturacion { get; set; }
        public string OrdenCompra { get; set; }
        public string NumFactura { get; set; }
        public string Asiento { get; set; }
        public string Divisa { get; set; }
        public decimal Impuesto { get; set; }
        public decimal ImporteFactura { get; set; }
        public string Empresa { get; set; }
        public string PedidoVentas { get; set; }
        public bool RegistradoTravesEmpresaVinculada { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal MontoPendiente { get; set; }
        
    }
}
namespace demoGrilla6.Models
{

    public class PagoFactura
    {
        /// <summary>
        /// Número de la factura
        /// </summary>
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Fecha de la factura
        /// </summary>
        public DateTime FechaFactura { get; set; }

        /// <summary>
        /// Código del proveedor
        /// </summary>
        public string Proveedor { get; set; }

        /// <summary>
        /// Número del pago (voucher)
        /// </summary>
        public string NumeroPago { get; set; }

        /// <summary>
        /// Fecha del pago
        /// </summary>
        public DateTime FechaPago { get; set; }

        /// <summary>
        /// Monto del pago
        /// </summary>
        public decimal MontoPago { get; set; }
    }

    public class PagoFacturaTotalPendiente
    {
        public string SaldoPendiente { get; set; }
        

    }

    public class UltimoPago
    {
        public DateTime?  FechaPago { get; set; }
        public double? MontoPago { get; set; }
        public string Moneda { get; set; }
        public string Comprobante { get; set; }
        public string Descripcion { get; set; }
    }

}

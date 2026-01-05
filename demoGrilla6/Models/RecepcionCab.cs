namespace demoGrilla6.Models
{
    public class RecepcionCab
    {
        public string CtaFacturacion { get; set; }
        public string PedidoCompra { get; set; }
        public string RecepcionProducto { get; set; }
        public DateTime Fecha { get; set; }

        public string Factura { get; set; }

        public bool EstaFacturado { get; set; }
        public string Hes { get; set; }


    }

    public class RecepcionNoFacturada
    {
        public int Numero { get; set; }
    }

}

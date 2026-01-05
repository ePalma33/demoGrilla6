using System;

namespace demoGrilla6.Models
{
    public class OrdeCompraCab
    {
        public string PurchId { get; set; }
        public string PurchName { get; set; }
        public string OrderAccount { get; set; }
        public string CurrencyCode { get; set; }
        //public string DlvMode { get; set; }
        public int PurchStatus { get; set; }
        public string DocumentStateText { get; set; }
        public string PurchStatusText { get; set; }

        public decimal TotalAmount { get; set; }

        //public DateTime CreatedDateTime { get; set; }
    }

    public class Empresa
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Rut { get; set; }

    }

}
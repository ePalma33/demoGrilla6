namespace demoGrilla6.Models
{
    public class VendInvoiceJour
    {
        public string VendGroup { get; set; }
        public string PurchId { get; set; }
        public string OrderAccount { get; set; }
        public string InvoiceAccount { get; set; }
        public string InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
    }
}
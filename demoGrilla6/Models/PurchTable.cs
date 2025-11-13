using System;

namespace demoGrilla6.Models
{
    public class PurchTable
    {
        public string PurchId { get; set; }
        public string PurchName { get; set; }
        public string OrderAccount { get; set; }
        public string CurrencyCode { get; set; }
        public string DlvMode { get; set; }
        public int PurchStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
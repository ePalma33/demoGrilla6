namespace demoGrilla6.Models
{
    public class OrdenCompraDet
    {
        public string PurchId { get; set; }
        public string ItemId { get; set; }
        public string Name { get; set; }
        public decimal QtyOrdered { get; set; }
        public decimal PurchPrice { get; set; }
        public decimal LineAmount { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal Purchqty { get; set; }
    }
}
namespace WebAPI_HD.Model
{
    public class BillDetails
    {
        public int BillDetailsID { get; set; }
        public int BillID { get; set; }
        public int ProductID { get; set; }
        public string? Unit { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public int VATrate { get; set; }
        public double VAT { get; set; }
        public double Amount { get; set; }
        public string? Note { get; set; }
        public virtual Product? Product { get; set; }

    }
}

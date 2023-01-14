namespace WebAPI_HD.Model
{
    public class BillDetails
    {
        public int BillDetailsID { get; set; }
        public string? BillID { get; set; }
        public int ProductID { get; set; }
        public string? Unit { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public int VATrate { get; set; }
       
        public double AmountNotVat
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }
        public double VAT
        {
            get
            {
                return AmountNotVat * (VATrate *0.01);
            }
        }
        public double Amount
        {
            get
            {
                return AmountNotVat + VAT;
            }
        }
        public string? Note { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Bill? Bill { get; set; }

    }
}

namespace WebAPI_HD.Model
{
    public class Bill
    {
        public Bill()
        {
            BillDetails = new HashSet<BillDetails>();
        }
        public int BillID { get; set; }
        public int CustomerID { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public double TotalAmount { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<BillDetails>? BillDetails { get; set; }
    }
}

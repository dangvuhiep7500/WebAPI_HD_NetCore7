namespace WebAPI_HD.Model
{
    public class Bill
    {
        public Bill()
        {
            BillDetails = new HashSet<BillDetails>();
        }
        public string? BillID { get; set; }
        public int CustomerID { get; set; }
        public string? ApplicationUserId { get; set; }
        public DateTime Date
        {
            get
            {
                return DateTime.Now;
            }
        }
        public double TotalAmount { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<BillDetails>? BillDetails { get; set; }
    }
}

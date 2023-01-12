namespace WebAPI_HD.Model
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string? CustomerCode { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? EmailAddress { get; set; }
        public ICollection<Bill>? Bills { get; set; }
    }
}

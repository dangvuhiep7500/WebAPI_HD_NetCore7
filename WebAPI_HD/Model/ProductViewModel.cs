namespace WebAPI_HD.Model
{
    public class ProductViewModel
    {
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int CategoryID { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double ImportUnitPrice { get; set; }
        public IFormFile[]? Image { get; set; }
    }
}

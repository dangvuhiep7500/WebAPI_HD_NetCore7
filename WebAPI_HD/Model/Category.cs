using System.Text.Json.Serialization;

namespace WebAPI_HD.Model
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        
        public virtual ICollection<Product>? Products { get; set; }
    }
}

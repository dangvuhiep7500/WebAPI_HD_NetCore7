using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI_HD.Model
{
    public class Product
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int CategoryID { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double ImportUnitPrice { get; set; }
        public List<ImageUri>? Picture { get; set; } = new List<ImageUri>();
        [JsonIgnore]
        public virtual Category? Categories { get; set; }
       
    }
    public class ImageUri
    {
        [Key]
        public int Id { get; set; }
        public string Uri { get; set; } = null!;
    }
}

﻿using System.Text.Json.Serialization;

namespace WebAPI_HD.Model
{
    public class Product
    {
        [JsonIgnore]
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int CategoryID { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double ImportUnitPrice { get; set; }
        public string? Image { get; set; }
        [JsonIgnore]
        public virtual Category? Categories { get; set; }

    }
}
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class Product
    {
        public long ID { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        [MaxLength(350)]
        [Required]
        public string Description { get; set; }
        public float AvgSales { get; set; }
        [JsonIgnore]
        public IList<Sale> Sales { get; set; }
        [JsonIgnore]
        public IList<VendingAvaliability> VendingAvaliabilities { get; set; }
    }
}

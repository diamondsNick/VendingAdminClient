using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class VendingAvaliability
    {
        public long ID { get; set; }
        public long VendingMachineID { get; set; }
        public long? ProductID { get; set; }
        public byte Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public VendingMachine? VendingMachine { get; set; }
        [JsonIgnore]
        public Product? Product { get; set; }
    }
}

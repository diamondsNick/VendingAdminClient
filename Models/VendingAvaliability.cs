using System.ComponentModel.DataAnnotations.Schema;

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
        public VendingMachine VendingMachine { get; set; }
        public Product Product { get; set; }
    }
}

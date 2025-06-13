using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class Sale
    {
        public long ID { get; set; }
        public long VendingMachineID { get; set; }
        public long? ProductID { get; set; }
        public long? PaymentMethodID { get; set; }
        public byte ProductAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Deposit { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Change { get; set; }
        public DateTime Date { get; set; }
        [JsonIgnore]
        public PaymentMethod PaymentMethod { get; set; }
        [JsonIgnore]
        public VendingMachine VendingMachine { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
    }
}

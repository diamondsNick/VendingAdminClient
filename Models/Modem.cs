using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class Modem
    {
        public long ID { get; set; }
        [MaxLength (100)]
        public string Model { get; set; }
        [AllowNull]
        public long? SimCardID { get; set; }
        [JsonIgnore]
        public SimCard? SimCard { get; set; }
        [JsonIgnore]
        public VendingMachine VendingMachine { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class Modem
    {
        public long ID { get; set; }
        [MaxLength(100)]
        public string Model { get; set; }
        [AllowNull]
        public long? SimCardID { get; set; }
        public long? CompanyID { get; set; }
        public long? SerialNum { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public SimCard? SimCard { get; set; }
        [JsonIgnore]
        public VendingMachine VendingMachine { get; set; }
        public Company? Company { get; set; }
        public string CompanyName => Company is not null ? $"{Company.ID} - \"{Company.Name}\"" : "Нет записи";
        public string VendingMachineName => VendingMachine is not null ? $"{VendingMachine.ID} - \"{VendingMachine.Name}\"" : "Нет записи";
    }
}

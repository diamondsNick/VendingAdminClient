using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class OperatingMode
    {
        public long ID { get; set; }
        [MaxLength(20)]
        [Required]
        public string Name { get; set; }
        [JsonIgnore]
        public IList<VendingMachine> VendingMachines { get; set; }
    }
}

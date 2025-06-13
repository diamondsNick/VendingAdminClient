using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class Status
    {
        public long ID { get; set; }
        [MaxLength (100)]
        [Required]
        public string Name { get; set; }
        [JsonIgnore]
        public IList<VendingMachine> VendingMachines { get; set; }
    }
}

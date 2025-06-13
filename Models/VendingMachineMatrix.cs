using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class VendingMachineMatrix
    {
        public long ID { get; set; }
        public long? ManufacturerID { get; set; }
        [MaxLength (150)]
        public string ModelName { get; set; }
        [JsonIgnore]
        public IList<VendingMachine> VendingMachines { get; set; }
        [JsonIgnore]
        public Manufacturer Manufacturer { get; set; }
    }
}

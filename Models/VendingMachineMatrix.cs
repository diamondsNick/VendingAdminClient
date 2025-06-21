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
        public IList<VendingMachine> VendingMachines { get; set; }
        public Manufacturer Manufacturer { get; set; }
    }
}

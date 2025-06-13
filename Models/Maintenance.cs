using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class Maintenance
    {
        public long ID { get; set; }
        public long VendingMachineID { get; set; } 
        public long? MaintainerID { get; set; }
        public DateTime MaintenanceDate { get; set; }
        [AllowNull]
        [MaxLength(250)]
        public string WorkDescription { get; set; }
        [MaxLength(250)]
        [AllowNull]
        public string ProblemDescription { get; set; }
        [JsonIgnore]
        public User Maintainer { get; set; }
        [JsonIgnore]
        public VendingMachine VendingMachine { get; set; }
    }
}

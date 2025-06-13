using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class Company
    {
        public long ID { get; set; }
        [MaxLength (200)]
        [Required]
        public string Name { get; set; }
        [Column (TypeName = "decimal(18,2)")]
        [Required]
        public decimal Finances { get; set; }
        [AllowNull]
        public long ParentCompanyID { get; set; }
        [JsonIgnore]
        public IList<User> CompanyUsers { get; set; }
        [JsonIgnore]
        public IList<VendingMachine> VendingMachines { get; set; }
    }
}

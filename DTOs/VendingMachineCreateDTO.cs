using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.DTOs
{
    public class VendingMachineCreateDTO
    {
        public long ID { get; set; }
        public long? StatusID { get; set; }
        public long? OperatingModeID { get; set; }
        public long? CompanyID { get; set; }
        public long? ModelID { get; set; }
        public long? ModemID { get; set; }
        [MaxLength(100)]
        [Required]
        public string? TimeZone { get; set; }
        [MaxLength(100)]
        [Required]
        public string? Name { get; set; }
        [MaxLength(350)]
        [Required]
        public string? Adress { get; set; }
        [MaxLength(100)]
        [AllowNull]
        public string? Coordinates { get; set; }
        [MaxLength(100)]
        [Required]
        public string? PlacementType { get; set; }
        [MaxLength(19)]
        [Required]
        public string? PlacementDate { get; set; }
        [MaxLength(5)]
        [Required]
        public string? StartHours { get; set; }
        [MaxLength(5)]
        [Required]
        public string? EndHours { get; set; }
    }
}

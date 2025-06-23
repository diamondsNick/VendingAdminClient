using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class SimCard
    {
        public long ID { get; set; }
        [MaxLength(11)]
        public string Number { get; set; }
        [MaxLength(25)]
        [Required]
        public string Vendor { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        [JsonIgnore]
        public Modem? Modem { get; set; }
        public long? CompanyID { get; set; }
        [JsonIgnore]
        public Company Company { get; set; }
    }
}

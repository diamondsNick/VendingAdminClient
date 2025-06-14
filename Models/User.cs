using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AdminClient.Models
{
    public class User
    {
        public long? ID { get; set; }
        [MaxLength (100)]
        public string? FullName { get; set; }
        [MaxLength (100)]
        public string? Email { get; set; }
        public long? RoleID { get; set; }
        public string? Role { get; set; }
        public long? CompanyID { get; set; }
        [MaxLength (10)]
        [AllowNull]
        public string Language { get; set; }
        [MaxLength(100)]
        [JsonIgnore]
        public string? Login { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
    }
}

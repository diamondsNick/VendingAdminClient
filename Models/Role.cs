using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdminClient.Models
{
    public class Role
    {
        public long ID { get; set; }
        [MaxLength (30)]
        [Required]
        public string Name { get; set; }
        [JsonIgnore]
        public IList<User> Users { get; set; }
    }
}

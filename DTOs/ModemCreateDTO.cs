using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.DTOs
{
    public class ModemCreateDTO
    {
        public long ID { get; set; }
        [MaxLength(100)]
        public string Model { get; set; }
        [AllowNull]
        public long? SimCardID { get; set; }
        public long? CompanyID { get; set; }
        public long? SerialNum { get; set; }
        public string Password { get; set; }
    }
}

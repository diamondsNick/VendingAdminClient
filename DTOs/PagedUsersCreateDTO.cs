using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;

namespace AdminClient.DTOs
{
    class PagedUsersCreateDTO
    {
        public long? ID { get; set; }
        [MaxLength(100)]
        public string? FullName { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        public long? RoleID { get; set; }
        public string? RegistrationDate { get; set; }
        public string? Phone { get; set; }
        public long? CompanyID { get; set; }
        [MaxLength(10)]
        [AllowNull]
        public string Language { get; set; }
        [MaxLength(100)]
        public string? Login { get; set; }
        public string? Password { get; set; }
    }
}

﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AdminClient.Models
{
    public class User
    {
        public long? ID { get; set; }
        [MaxLength(100)]
        public string? FullName { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        public long? RoleID { get; set; }
        public Role? Role { get; set; }
        public string? RegistrationDate { get; set; }
        public string? Phone { get; set; }
        public long? CompanyID { get; set; }
        public Company? Company { get; set; } 
        [MaxLength(10)]
        [AllowNull]
        public string Language { get; set; }
        [MaxLength(100)]
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string CompanyName => Company is not null ? $"{Company.ID} - \"{Company.Name}\"" : "Нет записи";

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;

namespace AdminClient.DTOs
{
    class PagedCompanies
    {
        public int TotalCount { get; set; }
        public List<Company>? Companies { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;

namespace AdminClient.DTOs
{
    class PagedModems
    {
        public int TotalCount { get; set; }
        public List<Modem> Modems { get; set; }
    }
}

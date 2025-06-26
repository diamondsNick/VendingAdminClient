using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;

namespace AdminClient.DTOs
{
    public class PagedSimCards
    {
        public int TotalCount { get; set; }
        public List<SimCard>? Sims { get; set; }
    }
}

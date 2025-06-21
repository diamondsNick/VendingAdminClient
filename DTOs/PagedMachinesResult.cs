using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;

namespace AdminClient.DTOs
{
    public class PagedMachinesResult
    {
        public int TotalCount { get; set; }
        public List<VendingMachine>? VendingMachines { get; set; }
    }
}

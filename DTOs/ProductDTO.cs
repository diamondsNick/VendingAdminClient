using System.Collections.Generic;
using AdminClient.Models;

namespace AdminClient.DTOs
{
    public class ProductDTO
    {
        public int TotalCount { get; set; }
        public List<Product> Products { get; set; }
    }
}

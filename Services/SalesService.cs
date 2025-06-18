using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    internal class SalesService
    {
        public static async Task<Sale[]> GetCompanySalesAsync(long CompanyId)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Sale/company/{CompanyId}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var sales = JsonConvert.DeserializeObject<Sale[]>(jsonResponse);
                    if (sales != null) return sales;
                    return Array.Empty<Sale>();
                }
                else
                {
                    return Array.Empty<Sale>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<Sale>();
            }
        }
    }
}

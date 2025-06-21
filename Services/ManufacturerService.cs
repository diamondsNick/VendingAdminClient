using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    class ManufacturerService
    {
        public static async Task<Manufacturer[]> GetManufacturersAsync()
        {
            try
            {
                string url = APILinking.BaseUrl + $"Manufacturer";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var manufacturers = JsonConvert.DeserializeObject<Manufacturer[]>(jsonResponse);
                    if (manufacturers != null) return manufacturers;
                    return Array.Empty<Manufacturer>();
                }
                else
                {
                    return Array.Empty<Manufacturer>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<Manufacturer>();
            }
        }
    }
}

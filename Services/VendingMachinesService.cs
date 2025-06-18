using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    internal class VendingMachinesService
    {
        public static async Task<VendingMachine[]> GetVendingMachinesAsync()
        {
            try
            {
                string url = APILinking.BaseUrl + $"VendingMachine";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var vendingMachines = JsonConvert.DeserializeObject<VendingMachine[]>(jsonResponse);
                    if (vendingMachines != null) return vendingMachines;
                    return Array.Empty<VendingMachine>();
                }
                else
                {
                    return Array.Empty<VendingMachine>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<VendingMachine>();
            }
        }
    }
}

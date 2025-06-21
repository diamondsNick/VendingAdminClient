using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    class MachineModelService
    {
        public static async Task<VendingMachineMatrix[]> GetVendingMachineMatricesAsync()
        {
            try
            {
                string url = APILinking.BaseUrl + $"VendingMachineMatrix";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var matrices = JsonConvert.DeserializeObject<VendingMachineMatrix[]>(jsonResponse);
                    if (matrices != null) return matrices;
                    return Array.Empty<VendingMachineMatrix>();
                }
                else
                {
                    return Array.Empty<VendingMachineMatrix>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<VendingMachineMatrix>();
            }
        }
    }
}

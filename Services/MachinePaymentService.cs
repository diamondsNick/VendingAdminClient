using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    class MachinePaymentService
    {
        public static async Task<MachinePaymentMethod[]> GetMachinePaymentMethodesAsync(long ID)
        {
            try
            {
                string url = APILinking.BaseUrl + $"MachinePaymentMethod";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var paymentMethods  = JsonConvert.DeserializeObject<MachinePaymentMethod[]>(jsonResponse);
                    if (paymentMethods != null)
                    {
                        var filteredMethods = paymentMethods.Where(pm => pm.VendingMachineID == ID).ToArray();
                        return filteredMethods;
                    }
                    return Array.Empty<MachinePaymentMethod>();
                }
                else
                {
                    return Array.Empty<MachinePaymentMethod>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<MachinePaymentMethod>();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    internal class MachineMoneyService
    {
        public static async Task<VendingMachineMoney[]?> GetMachineMoneyAsync(long Id)
        {
            try
            {
                string url = APILinking.BaseUrl + $"VendingMachineMoney/{Id}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var vendingMachineMoney = JsonConvert.DeserializeObject<VendingMachineMoney[]>(jsonResponse);
                    return vendingMachineMoney;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}

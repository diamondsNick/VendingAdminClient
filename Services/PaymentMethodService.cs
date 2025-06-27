using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    internal class PaymentMethodService
    {
        public static async Task<PaymentMethod[]> GetPaymentMethodesAsync()
        {
            try
            {
                string url = APILinking.BaseUrl + $"PaymentMethod";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var paymentMethods = JsonConvert.DeserializeObject<PaymentMethod[]>(jsonResponse);
                    if (paymentMethods != null) return paymentMethods;
                    return Array.Empty<PaymentMethod>();
                }
                else
                {
                    return Array.Empty<PaymentMethod>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<PaymentMethod>();
            }
        }
    }
}

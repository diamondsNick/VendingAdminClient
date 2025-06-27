using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AdminClient.DTOs;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    class MachinePaymentService
    {
        public static async Task<MachinePaymentMethod[]> GetMachinePaymentMethodesAsync(long ID = 0)
        {
            try
            {
                string url = APILinking.BaseUrl + $"MachinePaymentMethod";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var paymentMethods  = JsonConvert.DeserializeObject<MachinePaymentMethod[]>(jsonResponse);
                    if (paymentMethods != null && ID != 0)
                    {
                        var filteredMethods = paymentMethods.Where(pm => pm.VendingMachineID == ID).ToArray();
                        return filteredMethods;
                    }
                    if (paymentMethods != null) return paymentMethods;
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
        public static async Task<bool> CreateMachinePaymentMethodeAsync(long VendingMachineId, long PaymentMethodId)
        {
            try
            {
                string url = APILinking.BaseUrl + "MachinePaymentMethod";

                var body = new
                {
                    vendingMachineId = VendingMachineId,
                    paymentMethodId = PaymentMethodId
                };

                var json = JsonConvert.SerializeObject(body);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<bool> DeleteMachinePaymentMethodeAsync(long VendingMachineId, long PaymentMethodId)
        {
            try
            {
                string url = APILinking.BaseUrl + $"MachinePaymentMethod/{VendingMachineId}/{PaymentMethodId}";
                var response = await APIHttpClient.Instance.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}

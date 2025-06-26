using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using AdminClient.DTOs;
using AdminClient.Models;
using Newtonsoft.Json;
using OxyPlot;

namespace AdminClient.Services
{
    internal class VendingAvaliabilityService
    {
        public static async Task<VendingAvaliability[]> GetVendingAvaliabilitiesAsync(long vendingMachineId = 0)
        {
            try
            {
                string url = APILinking.BaseUrl + $"VendingAvaliability?VendingMachineId={vendingMachineId}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var vending = JsonConvert.DeserializeObject<VendingAvaliability[]>(jsonResponse);
                    if (vending != null) return vending;
                    return Array.Empty<VendingAvaliability>();
                }
                else
                {
                    return Array.Empty<VendingAvaliability>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<VendingAvaliability>();
            }
        }
        
        public static async Task<bool> DeleteVendingAvaliabilityAsync(long ID)
        {
            try
            {
                string url = APILinking.BaseUrl + $"VendingAvaliability/{ID}";

                var response = await APIHttpClient.Instance.DeleteAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static async Task<VendingAvaliability> UpdateVendingAvaliabilityAsync(long Id, VendingAvaliability vendingAv)
        {
            try
            {
                string url = APILinking.BaseUrl + $"VendingAvaliability/{Id}";

                var json = JsonConvert.SerializeObject(vendingAv);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var updatedVA = JsonConvert.DeserializeObject<VendingAvaliability>(responseBody);
                return updatedVA;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<VendingAvaliability?> CreateVendingAvaliabilityAsync(VendingAvaliability va)
        {
            try
            {
                string url = APILinking.BaseUrl + "VendingAvaliability";
                var json = JsonConvert.SerializeObject(va);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var createdVA = JsonConvert.DeserializeObject<VendingAvaliability>(responseBody);
                return createdVA;
            }
            catch
            {
                return null;
            }
        }
    }
}

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
        public static async Task<PagedMachinesResult> GetVendingMachinesPagedAsync(long CompanyId, int amount, int page)
        {
            try
            {
                string url = APILinking.BaseUrl + $"VendingMachine/{CompanyId}/{amount}/{page}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var vendingMachines = JsonConvert.DeserializeObject<PagedMachinesResult>(jsonResponse);
                    if (vendingMachines != null) return vendingMachines;
                    return new PagedMachinesResult();
                }
                else
                {
                    return new PagedMachinesResult();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static async Task<bool> DeleteVendingMachineAsync(long ID)
        {
            try
            {
                string url = APILinking.BaseUrl + $"VendingMachine/{ID}";

                var response = await APIHttpClient.Instance.DeleteAsync(url);

                return response.IsSuccessStatusCode;
                
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static async Task<VendingMachineCreateDTO?> UpdateVendingMachineAsync(long id, VendingMachineCreateDTO machineDto)
        {
            try
            {
                string url = APILinking.BaseUrl + $"VendingMachine/{id}";

                var json = JsonConvert.SerializeObject(machineDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var updatedMachine = JsonConvert.DeserializeObject<VendingMachineCreateDTO>(responseBody);
                return updatedMachine;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<VendingMachineCreateDTO?> CreateVendingMachineAsync(VendingMachineCreateDTO machine)
        {
            try
            {
                string url = APILinking.BaseUrl + "VendingMachine";
                var json = JsonConvert.SerializeObject(machine);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var createdMachine = JsonConvert.DeserializeObject<VendingMachineCreateDTO>(responseBody);
                return createdMachine;
            }
            catch
            {
                return null;
            }
        }

    }
}

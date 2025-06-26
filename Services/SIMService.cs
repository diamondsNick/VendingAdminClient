using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AdminClient.DTOs;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    class SIMService
    {
        public static async Task<SimCard[]> GetSimCardsAsync()
        {
            try
            {
                string url = APILinking.BaseUrl + $"SimCard";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var simCards = JsonConvert.DeserializeObject<SimCard[]>(jsonResponse);
                    if (simCards != null) return simCards;
                    return Array.Empty<SimCard>();
                }
                else
                {
                    return Array.Empty<SimCard>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<SimCard>();
            }
        }
        public static async Task<PagedSimCards?> GetPagedSimsAsync(long Amount, long Page, long CompanyID = 0, bool linked = false)
        {
            try
            {
                string url = APILinking.BaseUrl + $"SimCard/{Amount}/{Page}?CompanyId={CompanyID}&linked={linked}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var sims = JsonConvert.DeserializeObject<PagedSimCards>(jsonResponse);
                    return sims;
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
        public static async Task<bool> DeleteSimAsync(long id)
        {
            try
            {
                string url = APILinking.BaseUrl + $"SimCard/{id}";
                var response = await APIHttpClient.Instance.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool?> UpdateSIMAsync(long id, SimCard simInfo)
        {
            try
            {
                string url = APILinking.BaseUrl + $"SimCard/{id}";
                var json = JsonConvert.SerializeObject(simInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return false;

                var responseBody = await response.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


        public static async Task<SimCard?> CreateSIMAsync(SimCard simInfo)
        {
            try
            {
                string url = APILinking.BaseUrl + "SimCard";
                var json = JsonConvert.SerializeObject(simInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SimCard>(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}

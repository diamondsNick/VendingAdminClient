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
    class ModemService
    {
        public static async Task<PagedModems?> GetPagedModemsAsync(long Amount, long Page, long ParentCompany = 0)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Modem/{Amount}/{Page}?CompanyId={ParentCompany}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var modems = JsonConvert.DeserializeObject<PagedModems>(jsonResponse);
                    return modems;
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

        public static async Task<bool> DeleteModemAsync(long id)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Modem/{id}";
                var response = await APIHttpClient.Instance.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool?> UpdateModemAsync(long id, Modem modemInfo)
        {
            ModemCreateDTO modem = new()
            {
                ID = modemInfo.ID,
                Model = modemInfo.Model,
                SimCardID = modemInfo.SimCardID,
                SerialNum = modemInfo.SerialNum,
                Password = modemInfo.Password,
                CompanyID = modemInfo.Company.ID
            };

            try
            {
                string url = APILinking.BaseUrl + $"Modem/{id}";
                var json = JsonConvert.SerializeObject(modem);
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


        public static async Task<Modem?> CreateModemAsync(Modem modemInfo)
        {
            ModemCreateDTO modem = new()
            {
                ID = modemInfo.ID,
                Model = modemInfo.Model,
                SimCardID = modemInfo.SimCardID,
                SerialNum = modemInfo.SerialNum,
                Password = modemInfo.Password,
                CompanyID = modemInfo.Company.ID
            };

            try
            {
                string url = APILinking.BaseUrl + "Modem";
                var json = JsonConvert.SerializeObject(modem);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Modem>(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

    }
}

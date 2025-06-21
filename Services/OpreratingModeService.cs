using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    class OpreratingModeService
    {
        public static async Task<OperatingMode[]> GetOperatingModesAsync()
        {
            try
            {
                string url = APILinking.BaseUrl + $"OperatingMode";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var operatingModes = JsonConvert.DeserializeObject<OperatingMode[]>(jsonResponse);
                    if (operatingModes != null) return operatingModes;
                    return Array.Empty<OperatingMode>();
                }
                else
                {
                    return Array.Empty<OperatingMode>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<OperatingMode>();
            }
        }
    }
}

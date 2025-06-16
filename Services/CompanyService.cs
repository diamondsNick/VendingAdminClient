using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    class CompanyService
    {
        public static async Task<Company?> GetCompanyInfoAsync(long Id)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Companies/{Id}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var company = JsonConvert.DeserializeObject<Company>(jsonResponse);
                    return company;
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

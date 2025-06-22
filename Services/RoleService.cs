using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    class RoleService
    {
        public static async Task<Role[]> GetRolesAsync()
        {
            try
            {
                string url = APILinking.BaseUrl + $"Role";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var roles = JsonConvert.DeserializeObject<Role[]>(jsonResponse);
                    if (roles != null) return roles;
                    return Array.Empty<Role>();
                }
                else
                {
                    return Array.Empty<Role>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Array.Empty<Role>();
            }
        }
    }
}

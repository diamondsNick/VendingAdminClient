using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AdminClient.Models;
using Newtonsoft.Json;

namespace AdminClient.Services
{
    internal class LoginOperation
    {
        public async Task<User?> AuthenticateUserAsync(string login, string password)
        {
            try
            {
                string url = APILinking.BaseUrl+$"authentification/{login}/{password}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(jsonResponse);
                    return user;
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

        private static User? _currentUser;

        public static User InitializeCurrentUser(User user)
        {
            _currentUser = user;
            return _currentUser;
        }

        public static async Task<User?> GetCurrentUserAsync()
        {
            return await Task.FromResult(_currentUser);
        }
    }

}

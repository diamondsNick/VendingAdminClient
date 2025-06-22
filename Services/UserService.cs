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
    class UserService
    {
        public static async Task<PagedUsers?> GetPagedUsersAsync(long Amount, long Page, long CompanyId = 0)
        {
            try
            {
                string url = APILinking.BaseUrl + $"User/{Amount}/{Page}?CompanyId={CompanyId}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<PagedUsers>(jsonResponse);
                    return users;
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
        public static async Task<bool> DeleteUserAsync(long Id)
        {
            try
            {
                string url = APILinking.BaseUrl + $"User/{Id}";
                var response = await APIHttpClient.Instance.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                    return true;
                else throw new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public static async Task<PagedUsersCreateDTO?> UpdateUserAsync(long id, PagedUsersCreateDTO userDTO)
        {

            try
            {
                string url = APILinking.BaseUrl + $"User/{id}";

                var json = JsonConvert.SerializeObject(userDTO);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var userReturned = JsonConvert.DeserializeObject<PagedUsersCreateDTO>(responseBody);
                return userReturned;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<PagedUsersCreateDTO?> CreateUserAsync(PagedUsersCreateDTO user)
        {
            try
            {
                string url = APILinking.BaseUrl + "User";
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var createdUser = JsonConvert.DeserializeObject<PagedUsersCreateDTO>(responseBody);
                return createdUser;
            }
            catch
            {
                return null;
            }
        }
    }
}

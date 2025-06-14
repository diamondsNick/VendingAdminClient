using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.Services
{
    class RoleOperation
    {
        public static async void GetRole(long Id)
        {
            var response = await APIHttpClient.Instance.GetAsync(APILinking.BaseUrl + $"Role/{Id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var role = System.Text.Json.JsonSerializer.Deserialize<Models.Role>(jsonResponse);

                if (role == null)
                {
                    throw new Exception("Произошло исключение преобразования роли.");
                }

                var curUser = await LoginOperation.GetCurrentUserAsync();

                if (curUser != null && curUser.RoleID.HasValue && curUser.RoleID.Value == role.ID)
                {
                    curUser.Role = role.Name;
                    LoginOperation.InitializeCurrentUser(curUser);
                }
                else
                {
                    throw new Exception("У вас нет прав для просмотра этой роли.");
                }
            }
            else
            {
                throw new Exception($"Ошибка поиска роли: {response.ReasonPhrase}");
            }
        }
    }
}

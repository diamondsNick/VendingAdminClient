using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.Services
{
    public class APIHttpClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static HttpClient Instance => _httpClient;
    }
}

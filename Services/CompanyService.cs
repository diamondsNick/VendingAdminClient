﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AdminClient.DTOs;
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

        public static async Task<PagedCompanies?> GetPagedCompaniesAsync(long Amount, long Page, long ParentCompany = 0)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Companies/{Amount}/{Page}?parentCompanyId={ParentCompany}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var companies = JsonConvert.DeserializeObject<PagedCompanies>(jsonResponse);
                    return companies;
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
        public static async Task<bool> DeleteCompanyAsync(long Id)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Companies/{Id}";
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
        public static async Task<Company?> UpdateCompanyAsync(long id, Company company)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Companies/{id}";

                var json = JsonConvert.SerializeObject(company);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var companyReturned = JsonConvert.DeserializeObject<Company>(responseBody);
                return companyReturned;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Company?> CreateCompanyAsync(Company company)
        {
            try
            {
                string url = APILinking.BaseUrl + "Companies";
                var json = JsonConvert.SerializeObject(company);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
                return createdCompany;
            }
            catch
            {
                return null;
            }
        }

    }
}

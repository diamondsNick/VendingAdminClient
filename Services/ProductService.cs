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
    internal class ProductService
    {
        public static async Task<ProductDTO> GetProductsAsync(int amount = 0, int page = 0)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Product?amount={amount}&page={page}";

                var response = await APIHttpClient.Instance.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ProductDTO>(jsonResponse);
                    if (result != null) return result;
                    return new ProductDTO();
                }
                else
                {
                    return new ProductDTO();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new ProductDTO();
            }
        }

        public static async Task<bool> DeleteProductAsync(long ID)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Product/{ID}";

                var response = await APIHttpClient.Instance.DeleteAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static async Task<Product> UpdateProductAsync(long Id, Product product)
        {
            try
            {
                string url = APILinking.BaseUrl + $"Product/{Id}";

                var json = JsonConvert.SerializeObject(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var updatedProduct = JsonConvert.DeserializeObject<Product>(responseBody);
                return updatedProduct;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Product?> CreateProductAsync(Product product)
        {
            try
            {
                string url = APILinking.BaseUrl + "Product";
                var json = JsonConvert.SerializeObject(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await APIHttpClient.Instance.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var createdProduct = JsonConvert.DeserializeObject<Product>(responseBody);
                return createdProduct;
            }
            catch
            {
                return null;
            }
        }
    }
}

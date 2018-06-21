using BasketManagerWebApi.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class BasketManagerLibrary
    {
        public async static Task<List<BasketItem>> GetCartProducts()
        {
            List<BasketItem> cartItems = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await httpClient.GetAsync("https://localhost:44336/api/CartItems/");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    cartItems = JsonConvert.DeserializeObject<List<BasketItem>>(data);
                }
            }
            return cartItems;
        }

        public async static Task<List<BasketItem>> GetCartProduct(int productId)
        {
            List<BasketItem> cartItems = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await httpClient.GetAsync("https://localhost:44336/api/CartItems/" + productId);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    cartItems = JsonConvert.DeserializeObject<List<BasketItem>>(data);
                }
            }
            return cartItems;
        }
    }
}
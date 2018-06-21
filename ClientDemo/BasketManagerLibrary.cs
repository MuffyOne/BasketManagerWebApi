using BasketManagerWebApi.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class BasketManagerLibrary
    {

        private static readonly string _baseAddress = "https://localhost:44336/";

        public async static Task<List<BasketItem>> GetCartProducts()
        {
            List<BasketItem> cartItems = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(_baseAddress);
                HttpResponseMessage response = await httpClient.GetAsync("api/CartItems/");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    cartItems = JsonConvert.DeserializeObject<List<BasketItem>>(data);
                }
            }
            return cartItems;
        }

        public async static Task<BasketItem> GetCartProduct(int productId)
        {
            BasketItem cartItem = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(_baseAddress);
                HttpResponseMessage response = await httpClient.GetAsync("api/CartItems/" + productId);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    cartItem = JsonConvert.DeserializeObject<BasketItem>(data);
                }
            }
            return cartItem;
        }

        public async static Task<bool> PostCartItem(BasketItem basketItem, int basketId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(_baseAddress);
                var json = JsonConvert.SerializeObject(basketItem);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("api/CartItems/?cartId=" + basketId, content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
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

        /// <summary>
        /// Gets all the products in the baskets.
        /// If the basketId parameter is set just get the products into that particular basket
        /// </summary>
        /// <param name="basketId">The basket identifier. Can be null</param>
        /// <returns>Task&lt;List&lt;BasketItem&gt;&gt;.</returns>
        public async static Task<List<BasketItem>> GetCartProducts(int? basketId)
        {
            List<BasketItem> cartItems = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(_baseAddress);
                string uri = "api/CartItems/";
                if (basketId != null)
                {
                    uri += "?cartId=" + basketId.ToString();
                }
                HttpResponseMessage response = await httpClient.GetAsync(uri);
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

        public async static Task<bool> PutCartItem(BasketItem basketItem, int basketItemId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(_baseAddress);
                var json = JsonConvert.SerializeObject(basketItem);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, "api/CartItems/" + basketItemId);
                httpRequestMessage.Content = content;
                HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }

        public async static Task<bool> DeleteCartItem(int basketItemId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(_baseAddress);
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, "api/CartItems/" + basketItemId);
                HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }

        public async static Task<List<Basket>> GetBaskets()
        {
            List<Basket> baskets = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(_baseAddress);

                HttpResponseMessage response = await httpClient.GetAsync("api/Baskets");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    baskets = JsonConvert.DeserializeObject<List<Basket>>(data);
                }
            }
            return baskets;
        }

        public async static Task<Basket> GetBasket(int basketID)
        {
            Basket basket = null;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(_baseAddress);

                HttpResponseMessage response = await httpClient.GetAsync("api/Baskets/" + basketID);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    basket = JsonConvert.DeserializeObject<Basket>(data);
                }
            }
            return basket;
        }

        public async static Task<bool> DeleteBasketAndAllElements(int basketId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(_baseAddress);
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, "api/Baskets/" + basketId);
                HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
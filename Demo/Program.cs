using BasketManagerWebApi.Common.Models;
using ClientLibrary;
using System;
using System.Threading.Tasks;

namespace Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BeginProcess().Wait();
        }

        private static async Task BeginProcess()
        {
            //var listOfProducts = await BasketManagerLibrary.GetCartProducts();
            //var product = await BasketManagerLibrary.GetCartProduct(1);
            var productToSend = new BasketItem() { ProductId = 1, Quantity = 1 };
            var result = await BasketManagerLibrary.PostCartItem(productToSend,123);
        }
    }
}
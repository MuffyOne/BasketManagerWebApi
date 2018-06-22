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
            var productToSend = new BasketItem() { ProductId = 1, Quantity = 1 };
            var result = await BasketManagerLibrary.PostCartItem(productToSend, 123);
            var productToModify = new BasketItem() { Id = 1, BasketId = 123, ProductId = 1, Quantity = 2 };
            result = await BasketManagerLibrary.PutCartItem(productToModify, 1);
            var listOfProducts = await BasketManagerLibrary.GetCartProducts(null);
            listOfProducts = await BasketManagerLibrary.GetCartProducts(123);
            var product = await BasketManagerLibrary.GetCartProduct(1);
            result = await BasketManagerLibrary.DeleteCartItem(1);
            var baskets = await BasketManagerLibrary.GetBaskets();
            var basket = await BasketManagerLibrary.GetBasket(123);

            result = await BasketManagerLibrary.DeleteBasketAndAllElements(123);
        }
    }
}
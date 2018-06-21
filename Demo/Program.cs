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
            var result = await BasketManagerLibrary.GetCartProducts();
        }
    }
}
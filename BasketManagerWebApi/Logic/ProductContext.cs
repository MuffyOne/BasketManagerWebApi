using BasketManagerWebApi.Interfaces;
using BasketManagerWebApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BasketManagerWebApi.Logic
{
    public class ProductContext : IProductContext
    {

        private IEnumerable<Product> _products;

        #region constructor
        public ProductContext()
        {
            _products = LoadProductsFromStore();
        }
        #endregion

        #region Methods
        public IEnumerable<Product> GetProducts()
        {
            return _products;
        }

        public IEnumerable<Product> LoadProductsFromStore()
        {
            var jsonFile = @"..\BasketManagerWebApi\Resources\ProductList.json";
            var stringToDeserialize = File.ReadAllText(jsonFile);
            var products = JsonConvert.DeserializeObject<List<Product>>(stringToDeserialize);
            return products;
        }
        #endregion

    }
}

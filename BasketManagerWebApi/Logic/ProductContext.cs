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
            LoadProducts();
        }
        #endregion

        #region Methods
        public IEnumerable<Product> GetProducts()
        {
            return _products;
        }

        public void LoadProducts()
        {
            var jsonFile = @"..\BasketManagerWebApi\Resources\ProductList.json";
            var stringToDeserialize = File.ReadAllText(jsonFile);
            _products = JsonConvert.DeserializeObject<List<Product>>(stringToDeserialize);
        }
        #endregion

    }
}

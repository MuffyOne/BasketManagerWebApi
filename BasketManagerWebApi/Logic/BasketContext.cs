using BasketManagerWebApi.Common.Models;
using BasketManagerWebApi.Enums;
using BasketManagerWebApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BasketManagerWebApi.Models
{
    public class BasketContext : DbContext
    {
        private IProductContext _productContext;

        public BasketContext(DbContextOptions<BasketContext> options, IProductContext productContext) : base(options)
        {
            _productContext = productContext;
        }

        public DbSet<BasketItem> BasketProducts { get; set; }
        public DbSet<Basket> Baskets { get; set; }

        /// <summary>
        /// Adds the passed product into the basket with the specified ID.
        /// If the basket with tha ID is null a new basket it will be automatically created before adding the item
        /// </summary>
        /// <param name="basketItem">The basket item.</param>
        /// <param name="basketId">The basket identifier.</param>
        public void AddProduct(BasketItem basketItem, int basketId)
        {
            var basket = Baskets.FirstOrDefault(i => i.Id == basketId);
            if (basket == null)
            {
                CreateNewBasket(basketId);
            }
            basketItem.BasketId = basketId;
            BasketItem existingItem = BasketProducts.FirstOrDefault(i => i.ProductId == basketItem.ProductId && i.BasketId == basketId);
            if (existingItem == null)
            {
                BasketProducts.Add(basketItem);
                SaveChanges();
            }
            else
            {
                existingItem.Quantity += basketItem.Quantity;
            }
            UpdateBasketProperty(basketItem, basketId);
            UpdateProductStockQuantity(basketItem, null);
        }

        /// <summary>
        /// Checks if the product is existing in the current store and if the passed quantity is present.
        /// Returns ProductInjuryResult.NotFound in case the product does not exists
        /// Returns ProductInjuryResult.QuantityNotAvailable in case the passed quantity is greater than the stock
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="quantity">The quantity of the product.</param>
        /// <returns>ProductInjuryResult.</returns>
        public ProductInjuryResult CheckProduct(int productId, int quantity)
        {
            var availableProducts = _productContext.GetProducts();
            var product = availableProducts.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return ProductInjuryResult.NotFound;
            }
            else if (product.StockQuantity < quantity)
            {
                return ProductInjuryResult.QuantityNotAvailable;
            }
            return ProductInjuryResult.Ok;
        }

        /// <summary>
        /// Deletes the basket and all elements inside the basket.
        /// Returns BasketDeleteResult.NotFound In case the basket with the passed Id is non existent
        /// </summary>
        /// <param name="basketId">The basket identifier.</param>
        /// <returns>BasketDeleteResult.</returns>
        public BasketDeleteResult DeleteBasketAndAllElements(int basketId)
        {
            var basket = Baskets.FirstOrDefault(i => i.BasketId == basketId);
            if (basket == null)
            {
                return BasketDeleteResult.NotFound;
            }
            else
            {
                var products = BasketProducts.FirstOrDefault(i => i.BasketId == basketId);
                BasketProducts.RemoveRange(products);
                Baskets.Remove(basket);
                SaveChanges();
            }
            return BasketDeleteResult.Ok;
        }

        /// <summary>
        /// Gets the basket items with the passed Id.
        /// If the ID is null or -1 returns the products inside all baskets
        /// </summary>
        /// <param name="basketId">The basket identifier.</param>
        /// <returns>IEnumerable&lt;BasketItem&gt;.</returns>
        public IEnumerable<BasketItem> GetBasketItems(int? basketId)
        {
            if (basketId == -1 || basketId == null)
            {
                return BasketProducts;
            }
            else
            {
                return BasketProducts.Where(i => i.BasketId == basketId);
            }
        }

        /// <summary>
        /// Modifies the quantity of the passed basket item.
        /// </summary>
        /// <param name="id">The identifier of the item you want to modify.</param>
        /// <param name="basketItem">The new basket item.</param>
        /// <returns>ProductInjuryResult.</returns>
        public ProductInjuryResult ModifyCartItem(int id, BasketItem basketItem)
        {
            BasketItem existingItem = BasketProducts.FirstOrDefault(i => i.Id == id);
            var newQuantity = basketItem.Quantity - existingItem.Quantity;
            if (newQuantity > 0)
            {
                var result = CheckProduct(basketItem.ProductId, newQuantity);
                if (result != ProductInjuryResult.Ok)
                {
                    return result;
                }
            }
            existingItem.Quantity = basketItem.Quantity;
            UpdateProductStockQuantity(basketItem, newQuantity);
            return ProductInjuryResult.Ok;
        }

        private void CreateNewBasket(int cartId)
        {
            var basket = new Basket() { BasketId = cartId };
            Baskets.Add(basket);
            SaveChanges();
        }

        private void UpdateBasketProperty(BasketItem cartItem, int cartId)
        {
            var basket = Baskets.FirstOrDefault(i => i.BasketId == cartId);
            basket.ItemQuantity += cartItem.Quantity;
            var product = _productContext.GetProducts().FirstOrDefault(p => p.Id == cartItem.ProductId);
            basket.TotalPrice += (product.Price * cartItem.Quantity);
        }

        private void UpdateProductStockQuantity(BasketItem cartItem, int? quantity)
        {
            var product = _productContext.GetProducts().First(i => i.Id == cartItem.ProductId);
            int modifyQuantity = quantity == null ? cartItem.Quantity : (int)quantity;
            product.StockQuantity -= modifyQuantity;
        }
    }
}
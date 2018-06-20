using BasketManagerWebApi.Enums;
using BasketManagerWebApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BasketManagerWebApi.Models
{
    public class BasketContext : DbContext
    {
        public DbSet<BasketItem> BasketProducts { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        private IProductContext _productContext;


        public BasketContext(DbContextOptions<BasketContext> options, IProductContext productContext) : base(options)
        {
            _productContext = productContext;
        }

        public ProductInjuiryResult CheckProduct(int productId, int quantity)
        {
            var availableProducts = _productContext.GetProducts();
            var product = availableProducts.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return ProductInjuiryResult.NotFound;
            }
            else if (product.StockQuantity < quantity)
            {
                return ProductInjuiryResult.QuantityNotAvailable;
            }
            return ProductInjuiryResult.Ok;
        }

        internal IEnumerable<BasketItem> GetBasketItems(int? cartId)
        {
            if (cartId == -1)
            {
                return BasketProducts;
            }
            else
            {
                return BasketProducts.Where(i => i.BasketId == cartId);
            }
        }

        public void AddProduct(BasketItem cartItem, int cartId)
        {
            var basket = Baskets.FirstOrDefault(i => i.Id == cartId);
            if (basket == null)
            {
                CreateNewBasket(cartId);
            }
            cartItem.BasketId = cartId;
            BasketItem existingItem = BasketProducts.FirstOrDefault(i => i.ProductId == cartItem.ProductId && i.BasketId == cartId);
            if (existingItem == null)
            {
                BasketProducts.Add(cartItem);
                SaveChanges();
            }
            else
            {
                existingItem.Quantity += cartItem.Quantity;
            }
            UpdateBasketProperty(cartItem, cartId);
            UpdateProductStockQuantity(cartItem, null);
        }

        public ProductInjuiryResult ModifyCartItem(int id, BasketItem cartItem)
        {
            BasketItem existingItem = BasketProducts.FirstOrDefault(i => i.Id == id);
            var newQuantity = cartItem.Quantity - existingItem.Quantity;
            if(newQuantity > 0)
            {
                var result = CheckProduct(cartItem.ProductId, newQuantity);
                if(result != ProductInjuiryResult.Ok)
                {
                    return result;
                }
            }
            existingItem.Quantity = cartItem.Quantity;
            UpdateProductStockQuantity(cartItem, newQuantity);
            return ProductInjuiryResult.Ok;
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
    }
}

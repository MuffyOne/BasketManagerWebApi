﻿using BasketManagerWebApi.Enums;
using BasketManagerWebApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
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
           if(cartId == -1)
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
            }
            else
            {
                existingItem.Quantity += cartItem.Quantity;
            }
            UpdateBasketProperty(cartItem, cartId);
            UpdateProductStockQuantity(cartItem);

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

        private void UpdateProductStockQuantity(BasketItem cartItem)
        {
            var product = _productContext.GetProducts().First(i => i.Id == cartItem.ProductId);
            product.StockQuantity -= cartItem.Quantity;
        }
    }
}

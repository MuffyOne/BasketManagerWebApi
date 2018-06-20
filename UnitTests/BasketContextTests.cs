using BasketManagerWebApi.Controllers;
using BasketManagerWebApi.Enums;
using BasketManagerWebApi.Interfaces;
using BasketManagerWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace UnitTests
{
    public class BasketContextTests
    {

        private List<Product> _oneProduct = new List<Product>
        {
            new Product {
                Id = 1,
                Description = "This is a test product",
                Price = 0.5,
                StockQuantity = 1,
                Title = "Test Product"
            }
        };

        private BasketContext CreateBasketContextAndMocks()
        {
            var options = new DbContextOptionsBuilder<BasketContext>()
              .UseInMemoryDatabase(databaseName: "InMemoryTestDb")
              .Options;
            var mockedProductContext = new Mock<IProductContext>();
            mockedProductContext.Setup(foo => foo.GetProducts()).Returns(_oneProduct);
            return new BasketContext(options, mockedProductContext.Object);
        }

        #region CheckProductTests
        [Fact]
        public void CheckProduct_ProductExist_QuantityAvailable_ReturnsOk()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();

            //Act
            var result = basketContext.CheckProduct(1, 1);

            //Assert
            Assert.Equal(ProductInjuiryResult.Ok, result);
        }

        [Fact]
        public void CheckProduct_ProductDontExist_ReturnsNotFound()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();

            //Act
            var result = basketContext.CheckProduct(3, 1);

            //Assert
            Assert.Equal(ProductInjuiryResult.NotFound, result);
        }

        [Fact]
        public void CheckProduct_ProductExist_QuantityNotAvailable_ReturnsQuantityNotAvailable()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();

            //Act
            var result = basketContext.CheckProduct(1, 10);

            //Assert
            Assert.Equal(ProductInjuiryResult.QuantityNotAvailable, result);
        }
        #endregion

        #region AddProduct
        [Fact]
        public void AddProduct_ProductExist_QuantityAvailable_ItemAdded()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();
            BasketItem item = new BasketItem { Id = 1, ProductId = 1, Quantity = 1 };

            //Act
            basketContext.AddProduct(item,123);

            //Assert
            Assert.Contains(item, basketContext.BasketProducts);
        }
        #endregion
    }
}

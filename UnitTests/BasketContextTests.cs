using BasketManagerWebApi.Common.Models;
using BasketManagerWebApi.Enums;
using BasketManagerWebApi.Interfaces;
using BasketManagerWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests
{
    public class BasketContextTests
    {
        private readonly List<Product> _oneProduct = new List<Product>
        {
            new Product
            {
                Id = 1,
                Description = "This is a test product",
                Price = 0.5,
                StockQuantity = 10,
                Title = "Test Product"
            }
        };

        private Mock<IProductContext> _mockedProductContext;

        private BasketContext CreateBasketContextAndMocks()
        {
            var options = new DbContextOptionsBuilder<BasketContext>()
              .UseInMemoryDatabase(databaseName: "InMemoryTestDb")
              .Options; ;
            _mockedProductContext = new Mock<IProductContext>();
            _mockedProductContext.Setup(foo => foo.GetProducts()).Returns(_oneProduct);
            return new BasketContext(options, _mockedProductContext.Object);
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
            Assert.Equal(ProductInjuryResult.Ok, result);
        }

        [Fact]
        public void CheckProduct_ProductDontExist_ReturnsNotFound()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();

            //Act
            var result = basketContext.CheckProduct(3, 1);

            //Assert
            Assert.Equal(ProductInjuryResult.NotFound, result);
        }

        [Fact]
        public void CheckProduct_ProductExist_QuantityNotAvailable_ReturnsQuantityNotAvailable()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();

            //Act
            var result = basketContext.CheckProduct(1, 100);

            //Assert
            Assert.Equal(ProductInjuryResult.QuantityNotAvailable, result);
        }

        #endregion CheckProductTests

        #region AddProductTests

        [Fact]
        public void AddProduct_ProductExist_QuantityAvailable_ItemAdded()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();
            BasketItem item = new BasketItem { Id = 1, ProductId = 1, Quantity = 1 };

            //Act
            basketContext.AddProduct(item, 123);

            //Assert
            Assert.Contains(item, basketContext.BasketProducts);

            //TearDown
            basketContext.Database.EnsureDeleted();
        }

        [Fact]
        public void AddProduct_AddProductTwice_ProductExist_QuantityAvailable_ItemAddedWitchQuantityTwo()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();
            BasketItem item = new BasketItem { Id = 1, ProductId = 1, Quantity = 1 };

            //Act
            basketContext.AddProduct(item, 123);

            //Assert
            Assert.Contains(item, basketContext.BasketProducts);

            //Act
            basketContext.AddProduct(item, 123);
            var basketItems = basketContext.GetBasketItems(-1);

            //Assert
            Assert.Single(basketItems);
            Assert.Equal(2, basketItems.First().Quantity);

            //TearDown
            basketContext.Database.EnsureDeleted();
        }

        #endregion AddProductTests

        #region ModifyCartItemTests

        [Fact]
        public void ModifyCartItem_NewQuantityIsLess_BasketItemModifiedCorrectly()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();
            BasketItem item = new BasketItem { Id = 1, ProductId = 1, Quantity = 3 };
            basketContext.AddProduct(item, 123);

            //Pre act
            var basketItems = basketContext.GetBasketItems(-1);

            //Pre Assert
            Assert.Equal(3, basketItems.First().Quantity);

            //Act
            BasketItem modifiedItem = new BasketItem { Id = 1, ProductId = 1, Quantity = 1 };
            basketContext.ModifyCartItem(1, modifiedItem);
            basketItems = basketContext.GetBasketItems(-1);

            //Assert
            Assert.Equal(1, basketItems.First().Quantity);

            //TearDown
            basketContext.Database.EnsureDeleted();
        }

        [Fact]
        public void ModifyCartItem_NewQuantityIsMore_NotEnoughStock_BasketItemIsNotModified()
        {
            //Arrange
            var basketContext = CreateBasketContextAndMocks();
            BasketItem item = new BasketItem { Id = 1, ProductId = 1, Quantity = 1 };
            basketContext.AddProduct(item, 123);

            //Pre act
            var basketItems = basketContext.GetBasketItems(-1);

            //Pre Assert
            Assert.Equal(1, basketItems.First().Quantity);

            //Act
            BasketItem modifiedItem = new BasketItem { Id = 1, ProductId = 1, Quantity = 100 };
            var result = basketContext.ModifyCartItem(1, modifiedItem);
            basketItems = basketContext.GetBasketItems(-1);

            //Assert
            Assert.Equal(1, basketItems.First().Quantity);
            Assert.Equal(ProductInjuryResult.QuantityNotAvailable, result);

            //TearDown
            basketContext.Database.EnsureDeleted();
        }

        #endregion ModifyCartItemTests

        #region DeleteBasketAndAllElementsTests

        [Fact]
        public void DeleteBasketAndAllElementsTests_BasketExists_BasketDeletedAndElementDeleted()
        {
            var basketContext = CreateBasketContextAndMocks();
            BasketItem item = new BasketItem { Id = 1, ProductId = 1, Quantity = 1 };
            basketContext.AddProduct(item, 123);

            //Act
            var result = basketContext.DeleteBasketAndAllElements(123);
            var basket = basketContext.Baskets.FirstOrDefault(i => i.BasketId == 123);
            var basketItems = basketContext.GetBasketItems(123);

            //Assert
            Assert.Equal(BasketDeleteResult.Ok, result);
            Assert.Null(basket);
            Assert.Empty(basketItems);

            //TearDown
            basketContext.Database.EnsureDeleted();
        }

        [Fact]
        public void DeleteBasketAndAllElementsTests_BasketDontExists_ResultBasketNotFound()
        {
            var basketContext = CreateBasketContextAndMocks();

            //Act
            var result = basketContext.DeleteBasketAndAllElements(999);

            //Assert
            Assert.Equal(BasketDeleteResult.NotFound, result);
        }

        [Fact]
        public void DeleteBasketAndAllElementsTests_TwoBasketsExists_JustTheCorrectBasketDeletedAndElementDeleted()
        {
            var basketContext = CreateBasketContextAndMocks();
            BasketItem item = new BasketItem { Id = 1, ProductId = 1, Quantity = 1 };
            BasketItem itemTwo = new BasketItem { Id = 2, ProductId = 1, Quantity = 10 };
            basketContext.AddProduct(item, 123);
            basketContext.AddProduct(itemTwo, 124);

            //Act
            var result = basketContext.DeleteBasketAndAllElements(123);
            var basket123 = basketContext.Baskets.FirstOrDefault(i => i.BasketId == 123);
            var basketItems = basketContext.GetBasketItems(123);
            var basket124 = basketContext.Baskets.FirstOrDefault(i => i.BasketId == 124);
            var secondBasketItems = basketContext.GetBasketItems(124);

            //Assert
            Assert.Equal(BasketDeleteResult.Ok, result);
            Assert.Null(basket123);
            Assert.Empty(basketItems);
            Assert.NotNull(basket124);
            Assert.NotEmpty(secondBasketItems);

            //TearDown
            basketContext.Database.EnsureDeleted();
        }

        #endregion DeleteBasketAndAllElementsTests
    }
}
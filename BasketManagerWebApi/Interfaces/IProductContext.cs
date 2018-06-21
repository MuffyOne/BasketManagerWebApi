﻿using BasketManagerWebApi.Common.Models;
using BasketManagerWebApi.Models;
using System.Collections.Generic;

namespace BasketManagerWebApi.Interfaces
{
    public interface IProductContext
    {
        IEnumerable<Product> GetProducts();

        IEnumerable<Product> LoadProductsFromStore();
    }
}
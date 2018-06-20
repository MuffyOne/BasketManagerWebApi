using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketManagerWebApi.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public int BasketId { get; set; }
        public int ItemQuantity { get; set; }
        public double TotalPrice { get; set; }
    }
}

namespace BasketManagerWebApi.Common.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public int BasketId { get; set; }
        public int ItemQuantity { get; set; }
        public double TotalPrice { get; set; }
    }
}
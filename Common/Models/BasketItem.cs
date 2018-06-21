namespace BasketManagerWebApi.Common.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int BasketId { get; set; }
        public int Quantity { get; set; }
    }
}
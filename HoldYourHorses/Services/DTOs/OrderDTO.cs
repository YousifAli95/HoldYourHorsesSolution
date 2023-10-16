namespace HoldYourHorses.ViewModels.Account
{
    public class OrderDTO
    {
        public OrderLineDTO[] OrderLines { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderId { get; set; }
    }

    public class OrderLineDTO
    {
        public int ArticleNr { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public string ArticleName { get; set; }
    }
}

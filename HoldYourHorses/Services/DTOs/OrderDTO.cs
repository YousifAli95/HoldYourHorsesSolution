namespace HoldYourHorses.ViewModels.Account
{
    public class OrderDTO
    {
        public int Amount { get; set; }
        public string ArticleName { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
    }
}

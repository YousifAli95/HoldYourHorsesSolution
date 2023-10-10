namespace HoldYourHorses.Services.DTOs
{
    public class ShoppingCartProductDTO
    {
        public decimal Price { get; set; }
        public string ArticleName { get; set; }
        public int Amount { get; set; }
        public int ArticleNr { get; set; }

    }
}

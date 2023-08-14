namespace HoldYourHorses.ViewModels.Shop
{
    public class ShoppingCartVM
    {

        public string ArticleName { get; set; }
        public int Price { get; set; }
        public string FormattedPrice { get; set; }
        public int Amount { get; set; }
        public int ArticleNr { get; set; }
        public string GetPictureUrl()
        {
            return $"/Produktbilder/{ArticleName}.jpg";
        }
    }
}

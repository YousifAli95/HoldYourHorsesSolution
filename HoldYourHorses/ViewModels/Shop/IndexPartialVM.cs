namespace HoldYourHorses.ViewModels.Shop
{
    public class IndexPartialVM
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public string FormattedPrice { get; set; }

        public string Image { get; set; }

        public int ArticleNr { get; set; }

        public string GetPictureUrl()
        {
            return $"/Produktbilder/{Name}.jpg";
        }
    }


}

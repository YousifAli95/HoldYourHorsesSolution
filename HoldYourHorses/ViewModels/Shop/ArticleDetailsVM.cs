namespace HoldYourHorses.ViewModels.Shop
{
    public class ArticleDetailsVM
    {
        public int ArticleNr { get; set; }
        public decimal Price { get; set; }
        public int HorsePowers { get; set; }
        public int WoodDensity { get; set; }
        public string ArticleName { get; set; }
        public string Material { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ProductionCountry { get; set; }
        public bool AbsBrake { get; set; }

        public string GetAbsBrakeAsString()
        {
            return AbsBrake ? "Ja" : "Nej";
        }
        public string GetPictureUrl()
        {
            return $"/Produktbilder/{ArticleName}.jpg";
        }

    }
}

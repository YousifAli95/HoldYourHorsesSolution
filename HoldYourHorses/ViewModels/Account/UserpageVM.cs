namespace HoldYourHorses.ViewModels.Account
{
    public class UserpageVM
    {
        public string Username { get; set; }
        public string FirstName { get; set; }

        public Card[] Cards { get; set; }


    }

    public class Card
    {
        public string ArticleName { get; set; }
        public int Price { get; set; }
        public int ArticleNr { get; set; }

        public string GetPictureUrl()
        {
            return $"/Produktbilder/{ArticleName}.jpg";
        }
    }
}

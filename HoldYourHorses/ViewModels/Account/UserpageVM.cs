namespace HoldYourHorses.ViewModels.Account
{
    public class UserpageVM
    {
        public string Username { get; set; }
        public string FirstName { get; set; }

        public UserPageCard[] Cards { get; set; }


    }

    public class UserPageCard
    {
        public string ArticleName { get; set; }
        public decimal Price { get; set; }
        public int ArticleNr { get; set; }

    }
}

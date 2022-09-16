namespace HoldYourHorses.Views.Accounts
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

		public string getPriceFormatted()
		{
			var nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
			nfi.NumberGroupSeparator = " ";
			return Price.ToString("#,0", nfi);
		}


		public string GetPictureUrl()
		{
			return $"/Produktbilder/{ArticleName}.jpg";
		}
	}
}

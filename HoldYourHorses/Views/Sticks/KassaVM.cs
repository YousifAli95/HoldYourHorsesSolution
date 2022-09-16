namespace HoldYourHorses.Views.Sticks
{
	public class KassaVM
	{
		
		public string ArtikelNamn { get; set; }
		public int Pris { get; set; }
		public int Antal { get; set; }
		public int ArtikelNr { get; set; }

		public string getPriceFormatted()
		{
			var nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
			nfi.NumberGroupSeparator = " ";
			return Pris.ToString("#,0", nfi);
		}

		public static string getPriceFormatted(int pris)
		{
			var nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
			nfi.NumberGroupSeparator = " ";
			return pris.ToString("#,0", nfi);
		}

		public string GetPictureUrl()
		{
			return $"/Produktbilder/{ArtikelNamn}.jpg";
		}
	}
}

namespace HoldYourHorses.Views.Sticks
{
	public class CompareVM
	{
		public string ArtikelNamn { get; set; }
		public int Pris { get; set; }
		public int ArtikelNr { get; set; }

		public string Material { get; set; }

		public string Kategori { get; set; }
		public string Land { get; set; }

		public int Hästkrafter { get; set; }

		public int Trädensitet { get; set; }


		public string GetPictureUrl()
		{
			return $"/Produktbilder/{ArtikelNamn}.jpg";
		}

	}
}

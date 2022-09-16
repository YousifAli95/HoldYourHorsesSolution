namespace HoldYourHorses.Views.Sticks
{
    public class DetailsVM
    {
        public int Artikelnr { get; set; }
        public decimal Pris { get; set; }
        public int Hästkrafter { get; set; }
        public int Trädensitet { get; set; }
        public string Artikelnamn { get; set; } 
        public string Material { get; set; } 
        public string Kategori { get; set; } 
        public string Beskrivning { get; set; } 
        public string Tillverkningsland { get; set; } 
        public bool AbsBroms { get; set; }
        public string Bild { get; set; } = "https://sisselblom.se/wp-content/uploads/2021/03/Kapphasten-Bruno.png";

        public string getPriceFormatted()
        {
            var nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            return Pris.ToString("#,0", nfi);
        }

        public string BoolCheck()
        {
            return AbsBroms ? "Ja" : "Nej";
        }
        public string GetPictureUrl()
        {
            return $"/Produktbilder/{Artikelnamn}.jpg";
        }

    }
}

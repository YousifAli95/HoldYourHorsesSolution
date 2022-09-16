namespace HoldYourHorses.Views.Shared
{
    public class IndexPartialVM
    {
            public string Namn { get; set; }
            public decimal Pris { get; set; }
            public string Bild { get; set; }

            public int ArtikelNr { get; set; }

        public string getPriceFormatted()
        {
                var nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
                nfi.NumberGroupSeparator = " ";
                return Pris.ToString("#,0", nfi);
        }

        public string GetPictureUrl()
        {
            return $"/Produktbilder/{Namn}.jpg";
        }
    }


}

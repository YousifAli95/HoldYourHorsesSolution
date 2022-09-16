using HoldYourHorses.Models.Entities;

namespace HoldYourHorses.Views.Accounts
{
    public class OrderhistoryVM
    {
        public Order[] Historik { get; set; }

        public string getPriceFormatted(decimal pris)
        {
            var nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            return pris.ToString("#,0", nfi);
        }

       
        public class Order
        {
            public int Antal { get; set; }
            public string ArtikelNamn { get; set; }
            public decimal Pris { get; set; }
            public int OrderId { get; set; }
        }

        public int[] OrderHej { get; set; }

    }
}

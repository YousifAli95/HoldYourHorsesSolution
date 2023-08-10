namespace HoldYourHorses.ViewModels.Account
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

        public int[] OrderHej { get; set; }
    }
}

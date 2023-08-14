namespace HoldYourHorses.Utils
{
    public class ShopUtils
    {
        public static string FormatPrice(int price)
        {
            var nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            return price.ToString("#,0", nfi);
        }
    }
}

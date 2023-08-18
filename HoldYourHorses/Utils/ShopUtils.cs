namespace HoldYourHorses.Utils
{
    public class ShopUtils
    {
        public static string FormatPrice(decimal price)
        {
            var nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            return price.ToString("#,0", nfi);
        }

        public static string GetPictureUrl(string articleName)
        {
            return $"/Produktbilder/{articleName}.jpg";
        }
    }
}

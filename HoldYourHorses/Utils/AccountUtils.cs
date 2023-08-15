using System.Text.RegularExpressions;

namespace HoldYourHorses.Utils
{
    public class AccountUtils
    {
        public static string ConvertErrorMessageToSwedish(string errorMessage)
        {
            string pattern = @"^Username '(.+)' is already taken\.$";
            string replacement = "Användarnamnet '{0}' är redan upptaget.";

            if (Regex.IsMatch(errorMessage, pattern))
            {
                Match match = Regex.Match(errorMessage, pattern);
                string username = match.Groups[1].Value;

                string convertedMessage = Regex.Replace(errorMessage, pattern, string.Format(replacement, username));
                return convertedMessage;
            }
            else
            {
                return errorMessage;
            }
        }
    }
}
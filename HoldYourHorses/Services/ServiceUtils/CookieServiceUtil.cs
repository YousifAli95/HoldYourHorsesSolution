using HoldYourHorses.Services.DTOs;
using System.Text.Json;

namespace HoldYourHorses.Services.ServiceUtils
{
    public class CookieServiceUtil
    {
        readonly IHttpContextAccessor _accessor;
        readonly string _shoppingCartCookieKey = "ShoppingCart";
        readonly string _compareCookieKey = "compareString";

        public CookieServiceUtil(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public void ClearShoppingCartInCookie()
        {
            // Clear the shopping cart from all articles
            _accessor.HttpContext.Response.Cookies.Append(_compareCookieKey, "");
        }

        public void ClearCompareListInCookie()
        {
            // Clear the shopping cart list in Cookie
            _accessor.HttpContext.Response.Cookies.Append(_shoppingCartCookieKey, "");
        }


        public int GetShoppingCartCount()
        {
            // Gets a JSON string containing the shopping cart information
            var shoppingCartJson = _accessor.HttpContext.Request.Cookies[_shoppingCartCookieKey];

            if (string.IsNullOrEmpty(shoppingCartJson))
            {
                return 0;
            }

            // Deserialize the shoppingCartJson into ShoppingCartDTO
            var products = JsonSerializer.Deserialize<List<ShoppingCartDTO>>(shoppingCartJson);

            return products.Count();
        }

        public void SaveShoppingCartInCookie(List<ShoppingCartDTO> shoppingCart)
        {
            string json = JsonSerializer.Serialize(shoppingCart);
            _accessor.HttpContext.Response.Cookies.Append(_shoppingCartCookieKey, json);
        }

        public void SaveCompareListInCookie(List<int> compareList)
        {
            string json = JsonSerializer.Serialize(compareList);
            _accessor.HttpContext.Response.Cookies.Append(_compareCookieKey, json);
        }

        public List<ShoppingCartDTO> GetShoppingCartFromCookie()
        {
            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCartCookieKey];

            if (!string.IsNullOrEmpty(cookieContent))
            {
                return JsonSerializer.Deserialize<List<ShoppingCartDTO>>(cookieContent);
            }
            else
            {
                return new List<ShoppingCartDTO>();
            }
        }


        public List<int> GetCompareListFromCookie()
        {
            // Get compare list as a json string from cookie
            string json = _accessor.HttpContext.Request.Cookies[_compareCookieKey];

            if (!string.IsNullOrEmpty(json))
            {
                // If there is a compare list stored in cookie, deserialize it and return it as an array
                return JsonSerializer.Deserialize<List<int>>(json);

            }

            // return empty array if there is no compare list stored in cookie
            return new List<int> { };
        }

        public void RemoveAllComparisonsInCookie()
        {
            // Clear the compare list in cookie
            _accessor.HttpContext.Response.Cookies.Append(_compareCookieKey, "");
        }


    }
}

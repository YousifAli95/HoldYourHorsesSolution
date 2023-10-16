using HoldYourHorses.Models.Entities;
using HoldYourHorses.Services.DTOs;
using HoldYourHorses.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HoldYourHorses.Services.Implementations
{
    public class ApiServiceDB : IApiService
    {
        readonly ShopDBContext _shopContext;
        readonly IHttpContextAccessor _accessor;
        readonly IAccountService _accountService;
        readonly string _shoppingCart = "ShoppingCart";
        readonly string _compareString = "compareString";

        public ApiServiceDB(ShopDBContext shopContext, IHttpContextAccessor accessor, IAccountService accountService)
        {
            _shopContext = shopContext;
            _accessor = accessor;
            _accountService = accountService;
        }

        public void AddToCart(int articleNr, int amount)
        {
            List<ShoppingCartDTO> shoppingCartList;
            var shoppingCartDTO = new ShoppingCartDTO()
            {
                ArticleNr = articleNr,
                Amount = amount
            };

            // Get shoppingCartList as a json string from cookie
            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            // If shopping cart is empty
            if (string.IsNullOrEmpty(cookieContent))
                shoppingCartList = new List<ShoppingCartDTO> { shoppingCartDTO };

            else
            {
                // Convert the cookie string to a shoppingCartList
                shoppingCartList = JsonSerializer.Deserialize<List<ShoppingCartDTO>>(cookieContent);

                // Check if article already exists in the shopping cart
                var article = shoppingCartList.SingleOrDefault(p => p.ArticleNr == articleNr);
                if (article == null)
                    shoppingCartList.Add(shoppingCartDTO);
                else
                    // If article alreadye exist, just add the new amount with the old one
                    article.Amount += amount;
            }

            // Save the updated shoppingCartList in cookie
            string json = JsonSerializer.Serialize(shoppingCartList);
            _accessor.HttpContext.Response.Cookies.Append(_shoppingCart, json);
        }

        public void RemoveItemFromShoppingCart(int articleNr)
        {
            // Get shoppingCartList as a json string from cookie
            var cookieString = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            if (!string.IsNullOrEmpty(cookieString))
            {
                // Convert the cookie string to a shoppingCartList
                var shoppingCartList = JsonSerializer.Deserialize<List<ShoppingCartDTO>>(cookieString);

                // Check if article that is going to be deleted is in the shopping cart
                var itemToBeDeleted = shoppingCartList.SingleOrDefault(p => p.ArticleNr == articleNr);
                if (itemToBeDeleted == null)
                {
                    throw new BadHttpRequestException("ArticleNr not found");
                }

                // Remove the article form shopping cart and save the update shopping cart in cookie
                shoppingCartList.Remove(itemToBeDeleted);
                string json = JsonSerializer.Serialize(shoppingCartList);
                _accessor.HttpContext.Response.Cookies.Append(_shoppingCart, json);
            }
        }

        public void ClearCart()
        {
            // Clear the shopping cart from all articles
            _accessor.HttpContext.Response.Cookies.Append(_shoppingCart, "");
        }

        public int GetNumberOfItemsInCart()
        {
            // Get shoppingCartList as a json string from cookie
            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            if (string.IsNullOrEmpty(cookieContent))
            {
                return 0;
            }

            var shoppingCart = new List<ShoppingCartDTO>();
            shoppingCart = JsonSerializer.Deserialize<List<ShoppingCartDTO>>(cookieContent);


            return shoppingCart.Sum(o => o.Amount);
        }

        public bool AddOrRemoveCompare(int articleNr)
        {
            // Get compare list as a json string from cookie
            var compareString = _accessor.HttpContext.Request.Cookies[_compareString];

            List<int> compareList;
            string jsonString;

            // If there is no compare list saved in cookie
            if (string.IsNullOrEmpty(compareString))
            {
                // Create new list and save it in cookie
                compareList = new List<int> { articleNr };
                jsonString = JsonSerializer.Serialize(compareList);
                _accessor.HttpContext.Response.Cookies.Append(_compareString, jsonString);
                return true;
            }
            compareList = JsonSerializer.Deserialize<List<int>>(compareString);
            //If there is a saved compareList in cookie and it contains the articleNr
            if (compareList.Contains(articleNr))
            {
                //If compareList contains the articleNr remove it from the list and update the saved cookie
                compareList.Remove(articleNr);
                jsonString = JsonSerializer.Serialize(compareList);
                _accessor.HttpContext.Response.Cookies.Append(_compareString, jsonString);
                return false;
            }

            // if compareList doesn't contain articleNr
            // Add the the articleNr to the compare list and save the list in a cookie
            compareList.Add(articleNr);
            jsonString = JsonSerializer.Serialize(compareList);
            _accessor.HttpContext.Response.Cookies.Append(_compareString, jsonString);
            return true;
        }


        public int[] GetCompare()
        {
            // Get compare list as a json string from cookie
            string compareArticles = _accessor.HttpContext.Request.Cookies[_compareString];

            if (!string.IsNullOrEmpty(compareArticles))
            {
                // If there is a compare list stored in cookie, deserialize it and return it as an array
                int[] intValues = JsonSerializer.Deserialize<int[]>(compareArticles);
                return intValues;
            }

            // return empty array if there is no compare list stored in cookie
            return new int[] { };
        }


        public void RemoveAllComparisons()
        {
            _accessor.HttpContext.Response.Cookies.Append(_compareString, "");
        }

        public async Task<bool> AddOrRemoveFavourite(int articleNr)
        {
            string userId = await _accountService.GetUserId();
            var article = _shopContext.Favourites.SingleOrDefault(o => o.User == userId && o.ArticleNr == articleNr);

            // if the user has not already added the article to the favourite list 
            if (article == null)
            {
                // Add the article to the user's favourite list and return true
                _shopContext.Favourites.Add(new Favourite
                {
                    ArticleNr = articleNr,
                    User = userId
                }); ;
                _shopContext.SaveChanges();
                return true;
            }
            // if the user has already added the article to the favourite list 
            else
            {
                // Remove the article from the user's favourite list and return false
                _shopContext.Favourites.Remove(article);
                _shopContext.SaveChanges();
                return false;
            }
        }

        public async Task<string> GetFavourites()
        {
            var userName = _accessor.HttpContext.User.Identity.Name;

            // If the user is not logged in return an empty string
            if (string.IsNullOrEmpty(userName))
            {
                return "";
            }

            string userId = await _accountService.GetUserId();

            // Get a list containing article numbers of of all the articles in the user favourite list
            var favourites = await _shopContext.Favourites.Where(o => o.User == userId).Select(o => o.ArticleNr).ToArrayAsync();


            return JsonSerializer.Serialize(favourites);
        }

        public async Task<string[]> GetArticles()
        {
            // Get an array of the names of all the articles in the database
            return await _shopContext.Articles.Select(o => o.ArticleName).ToArrayAsync();
        }

    }
}

using HoldYourHorses.Models.Entities;
using HoldYourHorses.Services.DTOs;
using HoldYourHorses.Services.Interfaces;
using HoldYourHorses.Services.ServiceUtils;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HoldYourHorses.Services.Implementations
{
    public class ApiServiceDB : IApiService
    {
        readonly ShopDBContext _shopContext;
        readonly IHttpContextAccessor _accessor;
        readonly IAccountService _accountService;
        readonly CookieServiceUtil _cookieServiceUtil;


        public ApiServiceDB(ShopDBContext shopContext, IHttpContextAccessor accessor, IAccountService accountService, CookieServiceUtil cookieServiceUtil)
        {
            _shopContext = shopContext;
            _accessor = accessor;
            _accountService = accountService;
            _cookieServiceUtil = cookieServiceUtil;
        }

        public void AddArticleToShoppingCart(int articleNr, int amount)
        {
            // Get shopping cart from cookie as a List of ShoppingCartDTO
            List<ShoppingCartDTO> shoppingCartList = _cookieServiceUtil.GetShoppingCartFromCookie();

            // Create a new ShoppingCartDTO object from the function arguments
            var shoppingCartDTO = new ShoppingCartDTO()
            {
                ArticleNr = articleNr,
                Amount = amount
            };

            // If shopping cart is empty
            if (shoppingCartList.Count == 0)
                shoppingCartList = new List<ShoppingCartDTO> { shoppingCartDTO };

            else
            {
                // Check if article already exists in the shopping cart
                var article = shoppingCartList.SingleOrDefault(p => p.ArticleNr == articleNr);
                if (article == null)
                    shoppingCartList.Add(shoppingCartDTO);
                else
                    // If article alreadye exist, just add the new amount with the old one
                    article.Amount += amount;
            }

            // Save the the updated shopping cart in Cookie
            _cookieServiceUtil.SaveShoppingCartInCookie(shoppingCartList);
        }

        public void RemoveArticleFromShoppingCart(int articleNr)
        {
            // Get shopping cart from cookie as a List of ShoppingCartDTO
            List<ShoppingCartDTO> shoppingCartList = _cookieServiceUtil.GetShoppingCartFromCookie();

            // If List is not empty 
            if (shoppingCartList.Count == 0)
                throw new BadHttpRequestException("ArticleNr not found");

            // Check if article that is going to be deleted is in the shopping cart already
            var itemToBeDeleted = shoppingCartList.SingleOrDefault(p => p.ArticleNr == articleNr);
            if (itemToBeDeleted == null)
            {
                throw new BadHttpRequestException("ArticleNr not found");
            }

            // If the to be deleted article is in the list, remove the article from the shopping cart
            shoppingCartList.Remove(itemToBeDeleted);

            // Save the the updated shopping cart in Cookie
            _cookieServiceUtil.SaveShoppingCartInCookie(shoppingCartList);
        }

        public void ClearShoppingCart()
        {
            _cookieServiceUtil.ClearShoppingCartInCookie();
        }

        public int GetNumberOfItemsInCart()
        {
            // Get shopping cart from cookie as a List of ShoppingCartDTO
            List<ShoppingCartDTO> shoppingCartList = _cookieServiceUtil.GetShoppingCartFromCookie();

            // If List is empty return 0
            if (shoppingCartList.Count == 0)
            {
                return 0;
            }

            // Return the sum of all articles in the ShoppingCartList
            return shoppingCartList.Sum(o => o.Amount);
        }

        public bool AddOrRemoveCompare(int articleNr)
        {
            // Get compareList from cookie
            List<int> compareList = _cookieServiceUtil.GetCompareListFromCookie();

            //If there is a saved compareList in cookie and it contains the articleNr
            if (compareList.Contains(articleNr))
            {
                //Remove the article from the list and save the update compareList cookie
                compareList.Remove(articleNr);
                _cookieServiceUtil.SaveCompareListInCookie(compareList);

                // Return false due to removing the article
                return false;
            }

            // Add the the articleNr to the compareList and save the updated list in a cookie
            compareList.Add(articleNr);
            _cookieServiceUtil.SaveCompareListInCookie(compareList);

            // Return false due to adding the article
            return true;
        }

        public int[] GetCompareArticleNrArray()
        {
            return _cookieServiceUtil.GetCompareListFromCookie().ToArray();
        }

        public void RemoveAllComparisons()
        {
            _cookieServiceUtil.RemoveAllComparisonsInCookie();
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

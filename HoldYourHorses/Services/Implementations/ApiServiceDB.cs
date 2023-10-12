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
        readonly string _shoppingCart = "ShoppingCart";
        readonly string _compareString = "compareString";

        public ApiServiceDB(ShopDBContext shopContext, IHttpContextAccessor accessor)
        {
            _shopContext = shopContext;
            _accessor = accessor;
        }

        public void AddToCart(int articleNr, int amount)
        {
            List<ShoppingCartDTO> products;

            var newItem = new ShoppingCartDTO()
            {
                ArticleNr = articleNr,
                Amount = amount
            };

            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            if (string.IsNullOrEmpty(cookieContent))
                products = new List<ShoppingCartDTO> { newItem };

            else
            {
                products = JsonSerializer.Deserialize<List<ShoppingCartDTO>>(cookieContent);
                var article = products.SingleOrDefault(p => p.ArticleNr == articleNr);

                if (article == null)
                    products.Add(newItem);
                else
                    article.Amount += amount;
            }

            string json = JsonSerializer.Serialize(products);
            _accessor.HttpContext.Response.Cookies.Append(_shoppingCart, json);
        }

        public void RemoveItemFromShoppingCart(int articleNr)
        {
            if (!string.IsNullOrEmpty(_accessor.HttpContext.Request.Cookies[_shoppingCart]))
            {
                var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];
                var products = JsonSerializer.Deserialize<List<ShoppingCartDTO>>(cookieContent);

                var itemToBeDeleted = products.SingleOrDefault(p => p.ArticleNr == articleNr);

                if (itemToBeDeleted == null)
                {
                    throw new BadHttpRequestException("ArticleNr not found");
                }

                products.Remove(itemToBeDeleted);
                string json = JsonSerializer.Serialize(products);
                _accessor.HttpContext.Response.Cookies.Append(_shoppingCart, json);
            }
        }

        public void ClearCart()
        {
            _accessor.HttpContext.Response.Cookies.Append(_shoppingCart, "");
        }

        public int GetNumberOfItemsInCart()
        {
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
            var compareString = _accessor.HttpContext.Request.Cookies[_compareString];
            List<int> compareList;

            if (string.IsNullOrEmpty(compareString))
            {
                compareList = new List<int> { articleNr };
                string json = JsonSerializer.Serialize(compareList);
                _accessor.HttpContext.Response.Cookies.Append(_compareString, json);
                return true;
            }
            else
            {
                compareList = JsonSerializer.Deserialize<List<int>>(compareString);
                if (compareList.Contains(articleNr))
                {
                    compareList.Remove(articleNr);
                    string json = JsonSerializer.Serialize(compareList);
                    _accessor.HttpContext.Response.Cookies.Append(_compareString, json);
                    return false;
                }
                else
                {
                    if (compareList.Count < 4)
                    {
                        compareList.Add(articleNr);
                        string json = JsonSerializer.Serialize(compareList);
                        _accessor.HttpContext.Response.Cookies.Append(_compareString, json);
                    }
                    return true;
                }
            }

        }


        public int[] GetCompare()
        {
            string compareArticles = _accessor.HttpContext.Request.Cookies[_compareString];
            if (!string.IsNullOrEmpty(compareArticles))
            {
                try
                {
                    int[] intValues = JsonSerializer.Deserialize<int[]>(compareArticles);
                    return intValues;
                }
                catch (JsonException)
                {
                    // Handle the case where the JSON data cannot be deserialized into an int array
                }
            }

            return new int[] { };
        }


        public void RemoveAllComparisons()
        {
            _accessor.HttpContext.Response.Cookies.Append(_compareString, "");
        }

        public bool AddOrRemoveFavourite(int artikelnr)
        {
            var userName = _accessor.HttpContext.User.Identity.Name;
            string id = _shopContext.AspNetUsers.Where(o => o.UserName == userName).Select(o => o.Id).Single();
            var article = _shopContext.Favourites.SingleOrDefault(o => o.User == id && o.ArticleNr == artikelnr);
            if (article == null)
            {
                _shopContext.Favourites.Add(new Favourite
                {
                    ArticleNr = artikelnr,
                    User = id
                }); ;
                _shopContext.SaveChanges();
                return true;
            }
            else
            {
                _shopContext.Favourites.Remove(article);
                _shopContext.SaveChanges();
                return false;
            }
        }

        public string GetFavourites()
        {
            var userName = _accessor.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return "";
            }

            string id = _shopContext.AspNetUsers.Where(o => o.UserName == userName).Select(o => o.Id).Single();
            var favourites = _shopContext.Favourites.Where(o => o.User == id).Select(o => o.ArticleNr).ToList();
            return JsonSerializer.Serialize(favourites);
        }

        public async Task<string[]> GetArticles()
        {
            return await _shopContext.Articles.Select(o => o.ArticleName).ToArrayAsync();
        }

    }
}

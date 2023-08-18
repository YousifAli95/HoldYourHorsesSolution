using HoldYourHorses.Models.Entities;
using HoldYourHorses.Services.DTOs;
using HoldYourHorses.Services.Interfaces;
using System.Text.Json;

namespace HoldYourHorses.Services.Implementations
{
    public class ApiServiceDB : IApiService
    {
        readonly SticksDBContext _shopContext;
        readonly IHttpContextAccessor _accessor;
        readonly string _shoppingCart = "ShoppingCart";
        readonly string _compareString = "compareString";

        public ApiServiceDB(SticksDBContext shopContext, IHttpContextAccessor accessor)
        {
            _shopContext = shopContext;
            _accessor = accessor;
        }

        public void AddToCart(int articleNr, int amount, string articleName, int price)
        {
            List<ShoppingCartProduct> products;

            var newItem = new ShoppingCartProduct()
            {
                ArticleName = articleName,
                Price = price,
                ArticleNr = articleNr,
                Amount = amount
            };

            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            if (string.IsNullOrEmpty(cookieContent))
                products = new List<ShoppingCartProduct> { newItem };

            else
            {
                products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);
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
                var products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);

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
            if (!string.IsNullOrEmpty(_accessor.HttpContext.Request.Cookies[_shoppingCart]))
            {
                var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];
                var products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);
                products.Clear();

                string json = JsonSerializer.Serialize(products);
                _accessor.HttpContext.Response.Cookies.Append(_shoppingCart, json);
            }
        }

        public int GetCart()
        {
            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            if (string.IsNullOrEmpty(cookieContent))
            {
                return 0;
            }
            var shoppingCart = new List<ShoppingCartProduct>();
            shoppingCart = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);


            return shoppingCart.Sum(o => o.Amount);
        }

        public bool AddCompare(int artikelnr)
        {
            var key = "compareString";
            var compareString = _accessor.HttpContext.Request.Cookies[key];
            if (string.IsNullOrEmpty(compareString))
            {
                var compareList = new List<int> { artikelnr };
                string json = JsonSerializer.Serialize(compareList);
                _accessor.HttpContext.Response.Cookies.Append(key, json);
                return true;

            }
            else
            {
                var compareList = JsonSerializer.Deserialize<List<int>>(compareString);
                if (compareList.Contains(artikelnr))
                {
                    compareList.Remove(artikelnr);
                    string json = JsonSerializer.Serialize(compareList);
                    _accessor.HttpContext.Response.Cookies.Append(key, json);
                    return false;

                }
                else
                {
                    if (compareList.Count < 4)
                    {
                        compareList.Add(artikelnr);
                        string json = JsonSerializer.Serialize(compareList);
                        _accessor.HttpContext.Response.Cookies.Append(key, json);
                    }
                    return true;
                }
            }

        }


        public string GetCompare()
        {
            return _accessor.HttpContext.Request.Cookies[_compareString];
        }


        public void RemoveCompare()
        {
            _accessor.HttpContext.Response.Cookies.Append(_compareString, "");
        }

        public bool AddFavourite(int artikelnr)
        {
            var userName = _accessor.HttpContext.User.Identity.Name;
            string id = _shopContext.AspNetUsers.Where(o => o.UserName == userName).Select(o => o.Id).Single();
            var article = _shopContext.Favourites.SingleOrDefault(o => o.User == id && o.Artikelnr == artikelnr);
            if (article == null)
            {
                _shopContext.Favourites.Add(new Favourite
                {
                    Artikelnr = artikelnr,
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
            var favourites = _shopContext.Favourites.Where(o => o.User == id).Select(o => o.Artikelnr).ToList();
            return JsonSerializer.Serialize(favourites);
        }

    }
}

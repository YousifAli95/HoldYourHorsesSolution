using HoldYourHorses.Models.Entities;
using HoldYourHorses.Services.DTOs;
using HoldYourHorses.Services.Interfaces;
using HoldYourHorses.ViewModels.Shop;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HoldYourHorses.Services.Implementations
{
    public class ShopServiceDB : IShopService
    {
        readonly ShopDBContext _shopContext;
        readonly ITempDataDictionaryFactory _tempFactory;
        readonly IHttpContextAccessor _accessor;
        readonly string _shoppingCart = "ShoppingCart";

        public ShopServiceDB(ShopDBContext shopContext, IHttpContextAccessor accessor, ITempDataDictionaryFactory tempFactory)
        {
            this._shopContext = shopContext;
            this._accessor = accessor;
            this._tempFactory = tempFactory;
        }

        public void SaveOrder(CheckoutVM model)
        {
            Order order;
            string? userId = null;

            // if User is logged in
            if (_accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userName = _accessor.HttpContext.User.Identity.Name;
                userId = _shopContext.AspNetUsers.Where(o => o.UserName == userName)
                    .Select(o => o.Id)
                    .Single();
            }

            order = new Order
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                City = model.City,
                ZipCode = model.ZipCode,
                Address = model.Address,
                Country = model.Country,
                User = userId
            };

            using (var transaction = _shopContext.Database.BeginTransaction())
            {
                _shopContext.Orders.Add(order);
                _shopContext.SaveChanges();

                AddOrderLines(order.Id);

                transaction.Commit();
            }

            // Saving temporary data 
            _tempFactory.GetTempData(_accessor.HttpContext)[nameof(OrderConfirmationVM.FirstName)] = model.FirstName;
            _tempFactory.GetTempData(_accessor.HttpContext)[nameof(OrderConfirmationVM.Email)] = model.Email;
        }



        public void AddOrderLines(int id)
        {
            List<ShoppingCartDTO> shoppingCart;
            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];
            shoppingCart = JsonSerializer.Deserialize<List<ShoppingCartDTO>>(cookieContent);
            foreach (var item in shoppingCart)
            {
                var article = _shopContext.Articles.Find(item.ArticleNr);

                OrderLine orderLine = new OrderLine()
                {
                    Amount = item.Amount,
                    ArticleNr = article.ArticleNr,
                    Price = article.Price,
                    OrderId = id,
                    ArticleName = article.ArticleName
                };

                _shopContext.OrderLines.Add(orderLine);
                _shopContext.SaveChanges();
            }
        }

        public ArticleDetailsVM GetArticleDetailsVM(int artikelNr)
        {
            return _shopContext.Articles
                 .Where(o => o.ArticleNr == artikelNr)
                 .Select(o => new ArticleDetailsVM()
                 {
                     ArticleNr = o.ArticleNr,
                     Price = o.Price,
                     HorsePowers = o.HorsePowers,
                     WoodDensity = o.TreeDensity,
                     ArticleName = o.ArticleName,
                     Material = o.Material.Name,
                     Category = o.Category.Name,
                     Description = o.Description,
                     ProductionCountry = o.OriginCountry.Name,
                     AbsBrake = o.AbsBrake,
                 })
                 .Single();
        }


        public OrderConfirmationVM GetOrderConfirmationVM()
        {
            return new OrderConfirmationVM
            {
                FirstName = (string)_tempFactory.GetTempData(_accessor.HttpContext)[nameof(OrderConfirmationVM.FirstName)],
                Email = (string)_tempFactory.GetTempData(_accessor.HttpContext)[nameof(OrderConfirmationVM.Email)]
            };
        }

        public ShoppingCartVM[] GetShoppingCartVM()
        {
            var model = new List<ShoppingCartVM>();

            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            if (string.IsNullOrEmpty(cookieContent))
            {
                return model.ToArray();
            }

            var products = JsonSerializer.Deserialize<List<ShoppingCartDTO>>(cookieContent);

            var articleNumbers = products.Select(p => p.ArticleNr).ToList();
            var articles = _shopContext.Articles
                .Where(article => articleNumbers.Contains(article.ArticleNr))
                .ToList();

            foreach (var product in products)
            {
                var article = articles.FirstOrDefault(a => a.ArticleNr == product.ArticleNr);
                if (article != null)
                {
                    model.Add(new ShoppingCartVM
                    {
                        ArticleNr = article.ArticleNr,
                        ArticleName = article.ArticleName,
                        Price = article.Price,
                        Amount = product.Amount
                    });
                }
            }

            return model.ToArray();
        }

        public async Task<IndexVM> GetIndexVMAsync(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                _accessor.HttpContext.Session.SetString("search", search);
            }
            else
            {
                _accessor.HttpContext.Session.SetString("search", string.Empty);
            }

            var articles = await _shopContext.Articles.Select(o => new
            {
                o.Price,
                o.HorsePowers,
                o.Material,
                Typ = o.Category.Name
            }).ToArrayAsync();

            var indexVM = new IndexVM
            {
                PriceMax = decimal.ToInt32(articles.Max(o => o.Price)),
                PriceMin = decimal.ToInt32(articles.Min(o => o.Price)),
                HorsePowersMax = articles.Max(o => o.HorsePowers),
                HorsePowersMin = articles.Min(o => o.HorsePowers),
                Material = articles.DistinctBy(o => o.Material.Name).Select(o => o.Material.Name).ToArray(),
                Categories = articles.DistinctBy(o => o.Typ).Select(o => o.Typ).ToArray(),
            };

            return indexVM;
        }

        public IndexPartialVM[] GetIndexPartial(int minPrice, int maxPrice, int minHorsePowers, int maxHorsePowers, string category,
            string materials, bool isAscending, string sortOn)
        {
            string searchString = _accessor.HttpContext.Session.GetString("search");

            var articlesQuery = _shopContext.Articles
                .Where(o => o.Price >= minPrice && o.Price <= maxPrice)
                .Where(o => o.HorsePowers >= minHorsePowers && o.HorsePowers <= maxHorsePowers)
                .Where(o => category.Contains(o.Category.Name) && materials.Contains(o.Material.Name))
                .Where(o => string.IsNullOrEmpty(searchString) || o.ArticleName.Contains(searchString))
                .Select(o => new IndexPartialVM
                {
                    ArticleName = o.ArticleName,
                    Price = o.Price,
                    ArticleNr = o.ArticleNr,
                })
                .AsEnumerable();

            IEnumerable<IndexPartialVM> sortedQuery;

            // Sort the query using reflection based on the property name 
            if (isAscending)
                sortedQuery = articlesQuery.OrderBy(o => o.GetType().GetProperty(sortOn).GetValue(o, null));
            else
                sortedQuery = articlesQuery.OrderByDescending(o => o.GetType().GetProperty(sortOn).GetValue(o, null));

            return sortedQuery.ToArray();
        }


        public async Task<CompareVM[]> getCompareVMAsync()
        {
            var key = "compareString";
            var compareString = _accessor.HttpContext.Request.Cookies[key];
            var compareList = JsonSerializer.Deserialize<List<int>>(compareString);
            var model = await _shopContext.Articles.Where(o => compareList.Contains(o.ArticleNr)).Select(o => new CompareVM
            {
                ArticleName = o.ArticleName,
                ArticleNr = o.ArticleNr,
                HorsePowers = o.HorsePowers,
                Country = o.OriginCountry.Name,
                Material = o.Material.Name,
                Category = o.Category.Name,
                WoodDensity = o.TreeDensity
            }).ToArrayAsync();

            return model;
        }

    }


}


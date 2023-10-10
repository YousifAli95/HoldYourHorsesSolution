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
        readonly SticksDBContext _shopContext;
        readonly ITempDataDictionaryFactory _tempFactory;
        readonly IHttpContextAccessor _accessor;
        readonly string _shoppingCart = "ShoppingCart";

        public ShopServiceDB(SticksDBContext shopContext, IHttpContextAccessor accessor, ITempDataDictionaryFactory tempFactory)
        {
            this._shopContext = shopContext;
            this._accessor = accessor;
            this._tempFactory = tempFactory;
        }

        public void SaveOrder(CheckoutVM checkoutVM)
        {
            var o = checkoutVM;

            if (_accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _accessor.HttpContext.User.Identity.Name;
                var clientInfo = _shopContext.AspNetUsers.Where(o => o.UserName == userId)
                    .Select(o => o.Id)
                    .Single();
                _shopContext.Ordrars.Add(
                new Ordrar
                {
                    Förnamn = o.FirstName,
                    Efternamn = o.LastName,
                    Epost = o.Email,
                    Stad = o.City,
                    Postnummer = o.ZipCode,
                    Adress = o.Address,
                    Land = o.Country,
                    User = clientInfo
                });

                _shopContext.SaveChanges();

                AddToOrderrader(_shopContext.Ordrars.OrderBy(o => o.Id)
                    .Select(o => o.Id)
                    .Last());
                _shopContext.SaveChanges();
            }
            else
            {
                _shopContext.Ordrars.Add(
                new Ordrar
                {
                    Förnamn = o.FirstName,
                    Efternamn = o.LastName,
                    Epost = o.Email,
                    Stad = o.City,
                    Postnummer = o.ZipCode,
                    Adress = o.Address,
                    Land = o.Country
                });

                _shopContext.SaveChanges();

            }
            _tempFactory.GetTempData(_accessor.HttpContext)[nameof(OrderConfirmationVM.FirstName)] = o.FirstName;
            _tempFactory.GetTempData(_accessor.HttpContext)[nameof(OrderConfirmationVM.Email)] = o.Email;
        }

        public void AddToOrderrader(int id)
        {
            List<ShoppingCartProduct> products;
            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];
            products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);
            foreach (var item in products)
            {
                Orderrader orderrad = new Orderrader()
                {
                    Antal = item.Amount,
                    ArtikelNr = item.ArticleNr,
                    Pris = item.Price,
                    OrderId = id,
                    ArtikelNamn = item.ArticleName
                };

                _shopContext.Orderraders.Add(orderrad);
            }
        }

        public ArticleDetailsVM GetArticleDetailsVM(int artikelNr)
        {
            return _shopContext.Sticks
                 .Where(o => o.Artikelnr == artikelNr)
                 .Select(o => new ArticleDetailsVM()
                 {
                     ArticleNr = o.Artikelnr,
                     Price = o.Pris,
                     HorsePowers = o.Hästkrafter,
                     WoodDensity = o.Trädensitet,
                     ArticleName = o.Artikelnamn,
                     Material = o.Material.Namn,
                     Category = o.Kategori.Namn,
                     Description = o.Beskrivning,
                     ProductionCountry = o.Tillverkningsland.Namn,
                     AbsBrake = o.AbsBroms,
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

            var products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);

            var articleNumbers = products.Select(p => p.ArticleNr).ToList();
            var articles = _shopContext.Sticks
                .Where(article => articleNumbers.Contains(article.Artikelnr))
                .ToList();

            foreach (var item in products)
            {
                var article = articles.FirstOrDefault(a => a.Artikelnr == item.ArticleNr);
                if (article != null)
                {
                    model.Add(new ShoppingCartVM
                    {
                        ArticleNr = article.Artikelnr,
                        ArticleName = article.Artikelnamn,
                        Price = article.Pris,
                        Amount = item.Amount
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
            var sticks = await _shopContext.Sticks.Select(o => new
            {
                o.Artikelnamn,
                o.Pris,
                o.Artikelnr,
                o.Hästkrafter,
                o.Material,
                Typ = o.Kategori.Namn
            }).ToArrayAsync();

            var indexVM = new IndexVM
            {
                PrisMax = decimal.ToInt32(sticks.Max(o => o.Pris)),
                PrisMin = decimal.ToInt32(sticks.Min(o => o.Pris)),
                HästkrafterMax = sticks.Max(o => o.Hästkrafter),
                HästkrafterMin = sticks.Min(o => o.Hästkrafter),
                Material = sticks.DistinctBy(o => o.Material.Namn).Select(o => o.Material.Namn).ToArray(),
                Kategorier = sticks.DistinctBy(o => o.Typ).Select(o => o.Typ).ToArray(),
            };
            return indexVM;
        }

        public IndexPartialVM[] GetIndexPartial(int minPrice, int maxPrice, int minHK, int maxHK, string typer,
            string materials, bool isAscending, string sortOn)
        {
            string searchString = _accessor.HttpContext.Session.GetString("search");

            var cards = _shopContext.Sticks.Where(o =>
            o.Pris >= minPrice &&
            o.Pris <= maxPrice &&
            o.Hästkrafter >= minHK &&
            o.Hästkrafter <= maxHK &&
            typer.Contains(o.Kategori.Namn) &&
            materials.Contains(o.Material.Namn)
            && (string.IsNullOrEmpty(searchString)
            || o.Artikelnamn.Contains(searchString))).
            Select(o => new IndexPartialVM
            {
                ArticleName = o.Artikelnamn,
                Price = o.Pris,
                ArticleNr = o.Artikelnr,
            });
            IndexPartialVM[] model;

            if (isAscending)
            {
                model = cards.ToList().OrderBy(o => o.GetType().GetProperty(sortOn).GetValue(o, null)).ToArray();
            }
            else
            {
                model = cards.ToList().OrderByDescending(o => o.GetType().GetProperty(sortOn).GetValue(o, null)).ToArray();
            }

            return model;
        }


        public async Task<CompareVM[]> getCompareVMAsync()
        {
            var key = "compareString";
            var compareString = _accessor.HttpContext.Request.Cookies[key];
            var compareList = JsonSerializer.Deserialize<List<int>>(compareString);
            var model = await _shopContext.Sticks.Where(o => compareList.Contains(o.Artikelnr)).Select(o => new CompareVM
            {
                ArticleName = o.Artikelnamn,
                ArticleNr = o.Artikelnr,
                HorsePowers = o.Hästkrafter,
                Country = o.Tillverkningsland.Namn,
                Material = o.Material.Namn,
                Category = o.Kategori.Namn,
                WoodDensity = o.Trädensitet
            }).ToArrayAsync();

            return model;
        }

    }


}


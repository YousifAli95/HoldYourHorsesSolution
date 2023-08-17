using HoldYourHorses.Models.Entities;
using HoldYourHorses.Services.DTOs;
using HoldYourHorses.Services.Interfaces;
using HoldYourHorses.Utils;
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
                    Antal = item.Antal,
                    ArtikelNr = item.ArtikelNr,
                    Pris = item.Pris,
                    OrderId = id,
                    ArtikelNamn = item.Artikelnamn
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

        public void AddToCart(int articleNr, int amount, string articleName, int price)
        {
            List<ShoppingCartProduct> products;

            var newItem = new ShoppingCartProduct()
            {
                Artikelnamn = articleName,
                Pris = price,
                ArtikelNr = articleNr,
                Antal = amount
            };

            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            if (string.IsNullOrEmpty(cookieContent))
                products = new List<ShoppingCartProduct> { newItem };

            else
            {
                products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);
                var article = products.SingleOrDefault(p => p.ArtikelNr == articleNr);

                if (article == null)
                    products.Add(newItem);
                else
                    article.Antal += amount;
            }

            string json = JsonSerializer.Serialize(products);
            _accessor.HttpContext.Response.Cookies.Append(_shoppingCart, json);
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

        public void RemoveItemFromShoppingCart(int articleNr)
        {
            if (!string.IsNullOrEmpty(_accessor.HttpContext.Request.Cookies[_shoppingCart]))
            {
                var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];
                var products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);

                var itemToBeDeleted = products.SingleOrDefault(p => p.ArtikelNr == articleNr);

                if (itemToBeDeleted == null)
                {
                    throw new BadHttpRequestException("ArticleNr not found");
                }

                products.Remove(itemToBeDeleted);
                string json = JsonSerializer.Serialize(products);
                _accessor.HttpContext.Response.Cookies.Append(_shoppingCart, json);
            }
        }

        public ShoppingCartVM[] GetShoppingCartVM()
        {
            List<ShoppingCartProduct> products;

            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            if (cookieContent == null)
            {
                return null;
            }

            products = new List<ShoppingCartProduct>();
            products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);

            ShoppingCartVM[] shoppingCartVM = products
                .Select(o => new ShoppingCartVM
                {
                    Amount = o.Antal,
                    ArticleName = o.Artikelnamn,
                    Price = decimal.ToInt32(o.Pris),
                    FormattedPrice = ShopUtils.FormatPrice(decimal.ToInt32(o.Pris)),
                    ArticleNr = o.ArtikelNr,
                }).ToArray();

            return shoppingCartVM;
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
                Name = o.Artikelnamn,
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

        public int GetCart()
        {
            var cookieContent = _accessor.HttpContext.Request.Cookies[_shoppingCart];

            if (string.IsNullOrEmpty(cookieContent))
            {
                return 0;
            }
            var shoppingCart = new List<ShoppingCartProduct>();
            shoppingCart = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);


            return shoppingCart.Sum(o => o.Antal);
        }

        public bool addCompare(int artikelnr)
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
        public async Task<CompareVM[]> getCompareVMAsync()
        {
            var key = "compareString";
            var compareString = _accessor.HttpContext.Request.Cookies[key];
            var compareList = JsonSerializer.Deserialize<List<int>>(compareString);
            var model = await _shopContext.Sticks.Where(o => compareList.Contains(o.Artikelnr)).Select(o => new CompareVM
            {
                ArtikelNamn = o.Artikelnamn,
                ArtikelNr = o.Artikelnr,
                Hästkrafter = o.Hästkrafter,
                Land = o.Tillverkningsland.Namn,
                Material = o.Material.Namn,
                Kategori = o.Kategori.Namn,
                Trädensitet = o.Trädensitet
            }).ToArrayAsync();

            return model;
        }
        public string getCompare()
        {
            return _accessor.HttpContext.Request.Cookies["compareString"];
        }


        public void removeCompare()
        {
            _accessor.HttpContext.Response.Cookies.Append("compareString", "");
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


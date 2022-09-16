using HoldYourHorses.Models.Entities;
using HoldYourHorses.Views.Accounts;
using HoldYourHorses.Views.Shared;
using HoldYourHorses.Views.Sticks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using static HoldYourHorses.Views.Accounts.OrderhistoryVM;

namespace HoldYourHorses.Models
{
    public class DataService
    {
        private readonly SticksDBContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManagere;
        private readonly ITempDataDictionaryFactory tempFactory;

        public IHttpContextAccessor Accessor { get; }
        public IdentityDbContext IdentityDBContext { get; }

        public DataService(SticksDBContext context, IHttpContextAccessor accessor, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManagere, IdentityDbContext identityDBContext, ITempDataDictionaryFactory tempFactory)
        {
            this.context = context;
            this.Accessor = accessor;
            this.userManager = userManager;
            this.signInManagere = signInManagere;
            this.IdentityDBContext = identityDBContext;
            this.tempFactory = tempFactory;
        }

        internal void SaveOrder(CheckoutVM checkoutVM)
        {
            var o = checkoutVM;

            if (Accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = Accessor.HttpContext.User.Identity.Name;
                var clientInfo = context.AspNetUsers.Where(o => o.UserName == userId)
                    .Select(o => o.Id)
                    .Single();
                context.Ordrars.Add(
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

                context.SaveChanges();

                AddToOrderrader(context.Ordrars.OrderBy(o => o.Id)
                    .Select(o => o.Id)
                    .Last());
                context.SaveChanges();
            }
            else
            {
                context.Ordrars.Add(
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

                //AddToOrderrader(context.Ordrars.OrderBy(o => o.Id)
                //    .Select(o => o.Id)
                //    .Last());
                context.SaveChanges();

            }
            tempFactory.GetTempData(Accessor.HttpContext)[nameof(KvittoVM.FirstName)] = o.FirstName;
            tempFactory.GetTempData(Accessor.HttpContext)[nameof(KvittoVM.Epost)] = o.Email;

        }

        internal UserpageVM getUserPageVM(string? name)
        {
            var userName = Accessor.HttpContext.User.Identity.Name;
            string id = context.AspNetUsers.Where(o => o.UserName == userName).Select(o => o.Id).Single();
            var articleNumbers = context.Favourites.Where(o => o.User == id).Select(o => o.Artikelnr);
            var cards = context.Sticks.Where(o => articleNumbers.Contains(o.Artikelnr)).Select(o =>
            new Card
            {
                ArticleName = o.Artikelnamn,
                ArticleNr = o.Artikelnr,
                Price = decimal.ToInt32(o.Pris)
            }).ToArray();

            return new UserpageVM { Cards = cards, Username = name };
        }

        private void AddToOrderrader(int id)
        {
            List<ShoppingCartProduct> products;
            var cookieContent = Accessor.HttpContext.Request.Cookies["ShoppingCart"];
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

                context.Orderraders.Add(orderrad);
                //context.Orderraders.Select(o => new ShoppingCartProduct { Antal = item.Antal, ArtikelNr = item.ArtikelNr, Pris = item.Pris });
            }
        }

        internal DetailsVM GetDetailsVM(int artikelNr)
        {
            return context.Sticks
                 .Where(o => o.Artikelnr == artikelNr)
                 .Select(o => new DetailsVM()
                 {
                     Artikelnr = o.Artikelnr,
                     Pris = o.Pris,
                     Hästkrafter = o.Hästkrafter,
                     Trädensitet = o.Trädensitet,
                     Artikelnamn = o.Artikelnamn,
                     Material = o.Material.Namn,
                     Kategori = o.Kategori.Namn,
                     Beskrivning = o.Beskrivning,
                     Tillverkningsland = o.Tillverkningsland.Namn,
                     AbsBroms = o.AbsBroms,
                 })
                 .Single();
        }


        internal KvittoVM GetReceipt()
        {
            return new KvittoVM
            {
                FirstName = (string)tempFactory.GetTempData(Accessor.HttpContext)[nameof(KvittoVM.FirstName)],
                Epost = (string)tempFactory.GetTempData(Accessor.HttpContext)[nameof(KvittoVM.Epost)]
            };
        }

        internal int AddToCart(int artikelNr, int antalVaror, string arikelNamn, int pris)
        {
            List<ShoppingCartProduct> products;

            var newItem = new ShoppingCartProduct()
            {
                Artikelnamn = arikelNamn,
                Pris = pris,
                ArtikelNr = artikelNr,
                Antal = antalVaror
            };
            if (string.IsNullOrEmpty(Accessor.HttpContext.Request.Cookies["ShoppingCart"]))
            {
                products = new List<ShoppingCartProduct>();
                products.Add(newItem);

                string json = JsonSerializer.Serialize(products);

                Accessor.HttpContext.Response.Cookies.Append("ShoppingCart", json);

                return antalVaror;
            }
            else
            {
                var cookieContent = Accessor.HttpContext.Request.Cookies["ShoppingCart"];
                products = new List<ShoppingCartProduct>();
                products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);
                var temp = products.SingleOrDefault(p => p.ArtikelNr == artikelNr);
                if (temp == null)
                {
                    products.Add(newItem);
                }
                else
                {
                    temp.Antal += antalVaror;
                }
                string json = JsonSerializer.Serialize(products);

                Accessor.HttpContext.Response.Cookies.Append("ShoppingCart", json);
                return products.Sum(o => o.Antal);
            }
        }


        internal void ClearCart()
        {
            if (!string.IsNullOrEmpty(Accessor.HttpContext.Request.Cookies["ShoppingCart"]))
            {
                var cookieContent = Accessor.HttpContext.Request.Cookies["ShoppingCart"];
                var products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);
                products.Clear();

                string json = JsonSerializer.Serialize(products);
                Accessor.HttpContext.Response.Cookies.Append("ShoppingCart", json);
            }
        }

        internal void DeleteItem(int artikelNr)
        {
            if (!string.IsNullOrEmpty(Accessor.HttpContext.Request.Cookies["ShoppingCart"]))
            {
                var cookieContent = Accessor.HttpContext.Request.Cookies["ShoppingCart"];
                var products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);

                var itemToBeDeleted = products.SingleOrDefault(p => p.ArtikelNr == artikelNr);
                if (itemToBeDeleted != null)
                {
                    products.Remove(itemToBeDeleted);
                }

                string json = JsonSerializer.Serialize(products);
                Accessor.HttpContext.Response.Cookies.Append("ShoppingCart", json);
            }
        }


        internal KassaVM[] GetKassaVM()
        {
            List<ShoppingCartProduct> products;

            var cookieContent = Accessor.HttpContext.Request.Cookies["ShoppingCart"];

            if (cookieContent == null)
            {
                return null;
            }

            products = new List<ShoppingCartProduct>();
            products = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);

            KassaVM[] kassaVM = products
                .Select(o => new KassaVM
                {
                    Antal = o.Antal,
                    ArtikelNamn = o.Artikelnamn,
                    Pris = Decimal.ToInt32(o.Pris),
                    ArtikelNr = o.ArtikelNr,
                }).ToArray();

            return kassaVM;
        }

        internal async Task<IndexVM> GetIndexVMAsync(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                Accessor.HttpContext.Session.SetString("search", search);
            }
            else
            {
                Accessor.HttpContext.Session.SetString("search", String.Empty);
            }
            var sticks = await context.Sticks.Select(o => new
            {
                Artikelnamn = o.Artikelnamn,
                Pris = o.Pris,
                Artikelnr = o.Artikelnr,
                Hästkrafter = o.Hästkrafter,
                Material = o.Material,
                Typ = o.Kategori.Namn
            }).ToArrayAsync();

            var indexVM = new IndexVM
            {
                PrisMax = Decimal.ToInt32(sticks.Max(o => o.Pris)),
                PrisMin = Decimal.ToInt32(sticks.Min(o => o.Pris)),
                HästkrafterMax = sticks.Max(o => o.Hästkrafter),
                HästkrafterMin = sticks.Min(o => o.Hästkrafter),
                Material = sticks.DistinctBy(o => o.Material.Namn).Select(o => o.Material.Namn).ToArray(),
                Kategorier = sticks.DistinctBy(o => o.Typ).Select(o => o.Typ).ToArray(),
            };
            return indexVM;
        }

        internal IndexPartialVM[] GetIndexPartial(int minPrice, int maxPrice, int minHK, int maxHK, string typer,
            string materials, bool isAscending, string sortOn)
        {
            string searchString = Accessor.HttpContext.Session.GetString("search");

            var cards = context.Sticks.Where(o =>
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
                Namn = o.Artikelnamn,
                Pris = o.Pris,
                ArtikelNr = o.Artikelnr,
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
            var cookieContent = Accessor.HttpContext.Request.Cookies["ShoppingCart"];

            if (string.IsNullOrEmpty(cookieContent))
            {
                return 0;
            }
            var shoppingCart = new List<ShoppingCartProduct>();
            shoppingCart = JsonSerializer.Deserialize<List<ShoppingCartProduct>>(cookieContent);


            return shoppingCart.Sum(o => o.Antal);
        }

        public async Task<string> TryRegister(RegisterVM viewModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = viewModel.Username
            };
            IdentityResult createResult = await userManager.CreateAsync(user, viewModel.Password);

            if (createResult.Succeeded)
            {
                await signInManagere.PasswordSignInAsync(
                viewModel.Username,
                viewModel.Password,
                isPersistent: false,
                lockoutOnFailure: false);
            }
            return createResult.Succeeded ? null : createResult.Errors.First().Description;
        }
        public async Task<bool> TryLogin(LoginVM viewModel)
        {
            SignInResult signInResult = await signInManagere.PasswordSignInAsync(
                viewModel.Username,
                viewModel.Password,
                isPersistent: false,
                lockoutOnFailure: false);
            return signInResult.Succeeded;
        }
        internal bool addCompare(int artikelnr)
        {
            var key = "compareString";
            var compareString = Accessor.HttpContext.Request.Cookies[key];
            if (string.IsNullOrEmpty(compareString))
            {
                var compareList = new List<int> { artikelnr };
                string json = JsonSerializer.Serialize(compareList);
                Accessor.HttpContext.Response.Cookies.Append(key, json);
                return true;

            }
            else
            {
                var compareList = JsonSerializer.Deserialize<List<int>>(compareString);
                if (compareList.Contains(artikelnr))
                {
                    compareList.Remove(artikelnr);
                    string json = JsonSerializer.Serialize(compareList);
                    Accessor.HttpContext.Response.Cookies.Append(key, json);
                    return false;

                }
                else
                {
                    if (compareList.Count < 4)
                    {
                        compareList.Add(artikelnr);
                        string json = JsonSerializer.Serialize(compareList);
                        Accessor.HttpContext.Response.Cookies.Append(key, json);
                    }
                    return true;
                }
            }

        }
        internal async Task<CompareVM[]> getCompareVMAsync()
        {
            var key = "compareString";
            var compareString = Accessor.HttpContext.Request.Cookies[key];
            var compareList = JsonSerializer.Deserialize<List<int>>(compareString);
            var model = await context.Sticks.Where(o => compareList.Contains(o.Artikelnr)).Select(o => new CompareVM
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
        internal string getCompare()
        {
            return Accessor.HttpContext.Request.Cookies["compareString"];
        }


        internal void removeCompare()
        {
            Accessor.HttpContext.Response.Cookies.Append("compareString", "");
        }

        internal async Task LogOutUserAsync()
        {
            await signInManagere.SignOutAsync();
        }
        internal OrderhistoryVM GetOrderHistory()
        {


            var userName = Accessor.HttpContext.User.Identity.Name;

            var userId = context.AspNetUsers.Where(o => o.UserName == userName).Select(o => o.Id).Single();

            var allOrders = context.Ordrars.Where(o => o.User == userId)
                .Select(o => o.Id)
                .ToArray();

            List<Order> array = new List<Order>();
            foreach (var id in allOrders)
            {
                var q = context.Orderraders.Where(o => o.OrderId == id)
                   .Select(o => new Order
                   {
                       Antal = o.Antal,
                       Pris = o.Pris,
                       ArtikelNamn = o.ArtikelNamn,
                       OrderId = id
                   }).ToList();

                foreach (var item in q)
                {
                    array.Add(item);
                }

            }

            var p = array.Select(o => o.OrderId).DistinctBy(o => o).ToArray();
            var t = new OrderhistoryVM { Historik = array.ToArray(), OrderHej = p };
            return new OrderhistoryVM { Historik = array.ToArray(), OrderHej = p };
        }


        internal bool AddFavourite(int artikelnr)
        {
            var userName = Accessor.HttpContext.User.Identity.Name;
            string id = context.AspNetUsers.Where(o => o.UserName == userName).Select(o => o.Id).Single();
            var article = context.Favourites.SingleOrDefault(o => o.User == id && o.Artikelnr == artikelnr);
            if (article == null)
            {
                context.Favourites.Add(new Favourite
                {
                    Artikelnr = artikelnr,
                    User = id
                }); ;
                context.SaveChanges();
                return true;
            }
            else
            {
                context.Favourites.Remove(article);
                context.SaveChanges();
                return false;
            }
        }

        internal string GetFavourites()
        {
            var userName = Accessor.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return "";
            }
            string id = context.AspNetUsers.Where(o => o.UserName == userName).Select(o => o.Id).Single();
            var favourites = context.Favourites.Where(o => o.User == id).Select(o => o.Artikelnr).ToList();
            return JsonSerializer.Serialize(favourites);
        }

    }


}


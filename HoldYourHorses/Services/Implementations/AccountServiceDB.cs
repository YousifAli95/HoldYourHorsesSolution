using HoldYourHorses.Models.Entities;
using HoldYourHorses.Services.Interfaces;
using HoldYourHorses.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace HoldYourHorses.Services.Implementations
{
    public class AccountServiceDB : IAccountService
    {
        readonly ShopDBContext _shopContext;
        readonly UserManager<IdentityUser> _userManager;
        readonly SignInManager<IdentityUser> _signInManager;

        readonly IHttpContextAccessor _accessor;

        public AccountServiceDB(ShopDBContext shopContext, IHttpContextAccessor accessor, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManagere, ITempDataDictionaryFactory tempFactory)
        {
            this._shopContext = shopContext;
            this._accessor = accessor;
            this._userManager = userManager;
            this._signInManager = signInManagere;
        }

        public async Task<UserpageVM> GetUserPageVMAsync(string name)
        {
            string? userId = await GetUserId();

            var articleNumbers = await _shopContext.Favourites
                .Where(fav => fav.User == userId)
                .Select(fav => fav.ArticleNr)
                .ToListAsync();

            var cards = await _shopContext.Articles
                .Where(stick => articleNumbers.Contains(stick.ArticleNr))
                .Select(stick => new Card
                {
                    ArticleName = stick.ArticleName,
                    ArticleNr = stick.ArticleNr,
                    Price = decimal.ToInt32(stick.Price)
                })
                .ToArrayAsync();

            return new UserpageVM { Cards = cards, Username = name };
        }

        public async Task<string> TryRegisterAsync(RegisterVM viewModel)
        {
            var user = new IdentityUser
            {
                UserName = viewModel.Username,
                Email = viewModel.Email,
            };

            var createResult = await _userManager.CreateAsync(user, viewModel.Password);

            if (createResult.Succeeded)
            {
                await _signInManager.PasswordSignInAsync(
                    viewModel.Username,
                    viewModel.Password,
                    isPersistent: false,
                    lockoutOnFailure: false);
            }

            return createResult.Succeeded ? null : createResult.Errors.First().Description;
        }


        public async Task<bool> TryLogin(LoginVM viewModel)
        {
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(
                viewModel.Username,
                viewModel.Password,
                isPersistent: false,
                lockoutOnFailure: false);
            return signInResult.Succeeded;
        }

        public async Task LogOutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        //public async Task<OrderhistoryVM> GetOrderHistory()
        //{
        //    string? userId = await GetUserId();

        //    var ordersQuery = _shopContext.Orders
        //        .Where(o => o.User == userId)
        //        .Select(o => o.Id);

        //    var orderItems = await _shopContext.OrderLines
        //        .Where(o => ordersQuery.Contains(o.OrderId))
        //        .Select(o => new Order
        //        {
        //            Amount = o.Amount,
        //            Price = o.Price,
        //            ArticleName = o.ArticleName,
        //            OrderId = o.OrderId
        //        })
        //        .ToListAsync();

        //    var uniqueOrderIds = orderItems.Select(o => o.OrderId).Distinct().ToArray();

        //    var orderhistoryVM = new OrderhistoryVM
        //    {
        //        //{
        //        //    Historik = orderItems.ToArray(),
        //        //    OrderHej = uniqueOrderIds
        //    };

        //    return orderhistoryVM;
        //}

        private async Task<string> GetUserId()
        {
            var userName = _accessor.HttpContext.User.Identity.Name;
            var userId = await _shopContext.AspNetUsers
                .Where(o => o.UserName == userName)
                .Select(o => o.Id)
                .SingleOrDefaultAsync();
            return userId;
        }
    }

}

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

        public async Task<UserpageVM> GetUserPageVM(string name)
        {
            string? userId = await GetUserId();

            // Retrieve user's favorite articles directly using the navigation property
            var user = await _shopContext.AspNetUsers
                .Include(u => u.Favourites)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            var articleNumbers = user.Favourites.Select(fav => fav.ArticleNr).ToList();

            // Retrieve only the necessary information from the Articles table
            UserPageCard[] cards = await _shopContext.Articles
                .Where(stick => articleNumbers.Contains(stick.ArticleNr))
                .Select(stick => new UserPageCard
                {
                    ArticleName = stick.ArticleName,
                    ArticleNr = stick.ArticleNr,
                    Price = stick.Price
                })
                .ToArrayAsync();

            return new UserpageVM { Cards = cards, Username = name };
        }

        public async Task<string> TryRegister(RegisterVM viewModel)
        {
            // Create a new IdentityUser object with the provided username and email
            var user = new IdentityUser
            {
                UserName = viewModel.Username,
                Email = viewModel.Email,
            };

            // Attempt to create the user with the provided password using the UserManager
            var createResult = await _userManager.CreateAsync(user, viewModel.Password);

            if (createResult.Succeeded)
            {
                // If successful, sign in the user using the provided credentials
                await _signInManager.PasswordSignInAsync(
                    viewModel.Username,
                    viewModel.Password,
                    isPersistent: false,
                    lockoutOnFailure: false);
            }

            // Return null if user creation and sign-in were successful; otherwise, return the first error description
            return createResult.Succeeded ? null : createResult.Errors.First().Description;
        }

        public async Task<bool> TryLogin(LoginVM viewModel)
        {
            // Attempt to sign in the user using the provided username and password
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(
                viewModel.Username,
                viewModel.Password,
                isPersistent: false,  // Indicates whether the sign-in should be persisted
                lockoutOnFailure: false);  // Indicates whether the account should be locked out on failure

            // Return 'true' if the sign-in was successful, 'false' otherwise
            return signInResult.Succeeded;
        }

        public async Task LogOutUserAsync()
        {
            // Sign out the user
            await _signInManager.SignOutAsync();
        }

        public async Task<OrderhistoryVM> GetOrderHistoryVM()
        {
            // Get the user's ID
            string? userId = await GetUserId();

            // Query orders with associated order lines for the given user
            var ordersQuery = _shopContext.Orders
                .Include(o => o.OrderLines)
                .Where(o => o.User == userId)
                .Select(o => new OrderDTO
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    OrderLines = o.OrderLines.Select(orderLine => new OrderLineDTO
                    {
                        ArticleName = orderLine.ArticleName,
                        Amount = orderLine.Amount,
                        Price = orderLine.Price,
                        ArticleNr = orderLine.ArticleNr,
                    }).ToArray()
                });

            // Create an OrderhistoryVM and populate it with the retrieved orders
            var orderHistoryVM = new OrderhistoryVM()
            {
                Orders = await ordersQuery.ToArrayAsync()
            };

            return orderHistoryVM;
        }

        public async Task<string> GetUserId()
        {
            // Get the currently authenticated user's username from HttpContext
            var userName = _accessor.HttpContext.User.Identity.Name;

            // Query the database to retrieve the user's ID based on their username
            var userId = await _shopContext.AspNetUsers
                .Where(o => o.UserName == userName)
                .Select(o => o.Id)
                .SingleOrDefaultAsync();

            return userId;
        }
    }

}

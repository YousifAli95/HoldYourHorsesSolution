using HoldYourHorses.Models;
using HoldYourHorses.Views.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Controllers
{
    public class AccountsController : Controller
    {
        private readonly DataService dataService;

        public AccountsController(DataService dataService)
        {
            this.dataService = dataService;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterVM viewModel)
        {
            if (!ModelState.IsValid)
                return View();

            // Try to register user
            var errorMessage = await dataService.TryRegister(viewModel);
            if (errorMessage != null)
            {
                // Show error
                ModelState.AddModelError(string.Empty, errorMessage);
                return View();
            }

            // Redirect user
            //return RedirectToAction("IndexAsync", "SticksController", new { area = "" });

            return RedirectToAction(nameof(Userpage));
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginVM viewModel)
        {
            if (!ModelState.IsValid)
                return View();

            // Check if credentials is valid (and set auth cookie)
            var success = await dataService.TryLogin(viewModel);
            if (!success)
            {
                // Show error
                ModelState.AddModelError(string.Empty, "Login failed");
                return View();
            }

            // Redirect user
            return RedirectToAction(nameof(Userpage));
        }
        [Authorize]
        [HttpGet("Userpage")]
        public IActionResult Userpage()
        {
            UserpageVM model = dataService.getUserPageVM(User.Identity.Name);
            return View(model);
        }
        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await dataService.LogOutUserAsync();
            return RedirectToAction(nameof(Login));
        }
        [Authorize]
        [HttpGet("orderhistory")]
        public IActionResult Orderhistory()
        {
            return View(dataService.GetOrderHistory());
        }
        [Authorize]
        [HttpGet("orderhistoryget")]
        public IActionResult Orderhistoryget()
        {
            var order = dataService.GetOrderHistory();

            //dataService.SortHistory()
            return View(order);
        }
    }
}

using HoldYourHorses.Services.Interfaces;
using HoldYourHorses.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService dataService;

        public AccountController(IAccountService dataService)
        {
            this.dataService = dataService;
            Console.WriteLine("account");

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

            var errorMessage = await dataService.TryRegisterAsync(viewModel);
            if (errorMessage != null)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View();
            }

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

            var success = await dataService.TryLogin(viewModel);
            if (!success)
            {
                ModelState.AddModelError(nameof(LoginVM.Username), "Felaktigt användarnamn eller lösernord");
                return View();
            }

            return RedirectToAction(nameof(Userpage));
        }

        [Authorize]
        [HttpGet("Userpage")]
        public async Task<IActionResult> Userpage()
        {
            UserpageVM model = await dataService.GetUserPageVMAsync(User.Identity.Name);
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

            return View(order);
        }
    }
}

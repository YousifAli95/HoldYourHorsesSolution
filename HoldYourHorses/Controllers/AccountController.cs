using HoldYourHorses.Services.Interfaces;
using HoldYourHorses.Utils;
using HoldYourHorses.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Controllers
{
	public class AccountController : Controller
	{
		private readonly IAccountService _accountService;

		public AccountController(IAccountService accountService)
		{
			this._accountService = accountService;
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

			var errorMessage = await _accountService.TryRegisterAsync(viewModel);
			if (errorMessage != null)
			{
				ModelState.AddModelError(string.Empty, AccountUtils.ConvertErrorMessageToSwedish(errorMessage));
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

			var success = await _accountService.TryLogin(viewModel);
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
			UserpageVM model = await _accountService.GetUserPageVMAsync(User.Identity.Name);
			return View(model);
		}
		[Authorize]
		[HttpGet("logout")]
		public async Task<IActionResult> LogoutAsync()
		{
			await _accountService.LogOutUserAsync();
			return RedirectToAction(nameof(Login));
		}

		[Authorize]
		[HttpGet("orderhistory")]
		public async Task<IActionResult> OrderhistoryAsync()
		{
			var model = await _accountService.GetOrderHistoryVM();
			return View(model);
		}
	}
}

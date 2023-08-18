using HoldYourHorses.Services.Interfaces;
using HoldYourHorses.ViewModels.Shop;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Controllers
{
    public class ShopController : Controller
    {
        private readonly IShopService _shopService;
        private readonly IApiService _ApiService;

        public ShopController(IShopService shopService, IApiService apiService)
        {
            _shopService = shopService;
            _ApiService = apiService;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> IndexAsync(string search)
        {
            IndexVM model = await _shopService.GetIndexVMAsync(search);
            return View(model);
        }

        [HttpGet("article/{articleNr}")]
        public IActionResult ArticleDetails(int articleNr)
        {
            ArticleDetailsVM model = _shopService.GetArticleDetailsVM(articleNr);
            return View(model);
        }

        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost("checkout")]
        public IActionResult Checkout(CheckoutVM checkoutVM)
        {
            if (!ModelState.IsValid)
                return View();

            _shopService.SaveOrder(checkoutVM);
            _ApiService.ClearCart();
            return RedirectToAction(nameof(OrderConfirmation));
        }

        [HttpGet("IndexPartial")]
        public IActionResult IndexPartial(int minPrice, int maxPrice, int maxHK, int minHK, string typer, string materials, bool isAscending, string sortOn)
        {
            IndexPartialVM[] model = _shopService.GetIndexPartial(minPrice, maxPrice, minHK, maxHK, typer, materials, isAscending, sortOn);
            return PartialView("_IndexPartial", model);
        }

        [HttpGet("shopping-cart")]
        public IActionResult ShoppingCart()
        {
            ShoppingCartVM[] model = _shopService.GetShoppingCartVM();
            return View(model);
        }

        [HttpGet("order-confirmation")]
        public IActionResult OrderConfirmation()
        {
            return View(_shopService.GetOrderConfirmationVM());
        }

        [HttpGet("Compare")]
        public async Task<IActionResult> CompareAsync()
        {
            CompareVM[] model = await _shopService.getCompareVMAsync();
            return View(model);
        }
    }
}

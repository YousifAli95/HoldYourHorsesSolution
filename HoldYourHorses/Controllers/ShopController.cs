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
            IndexVM model = await _shopService.GetIndexVM(search);
            return View(model);
        }

        [HttpGet("article/{articleNr}")]
        public IActionResult ArticleDetails(int articleNr)
        {

            var model = _shopService.GetArticleDetailsVM(articleNr);

            if (model == null)
                return BadRequest("Article is not found");

            return View(model);
        }

        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            int ShoppingCartCount = _shopService.GetShoppingCartCount();

            if (ShoppingCartCount < 1)
            {
                return RedirectToAction(nameof(ShoppingCart));
            }

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            if (!ModelState.IsValid)
                return View();

            await _shopService.SaveOrder(model);
            _ApiService.ClearShoppingCart();
            return RedirectToAction(nameof(OrderConfirmation));
        }

        [HttpGet("IndexPartial")]
        public IActionResult IndexPartial(int minPrice, int maxPrice, int maxHK, int minHK, string typer, string materials, bool isAscending, string sortOn)
        {
            IndexPartialVM[] model = _shopService.GetIndexPartial(minPrice, maxPrice, minHK, maxHK, typer, materials, isAscending, sortOn);
            return PartialView("_IndexPartial", model);
        }

        [HttpGet("shopping-cart")]
        public async Task<IActionResult> ShoppingCart()
        {
            ShoppingCartVM[] model = await _shopService.GetShoppingCartVM();
            return View(model);
        }

        [HttpGet("order-confirmation")]
        public IActionResult OrderConfirmation()
        {
            return View(_shopService.GetOrderConfirmationVM());
        }

        [HttpGet("compare")]
        public async Task<IActionResult> CompareAsync()
        {
            CompareVM[] model = await _shopService.GetCompareVM();
            return View(model);
        }
    }
}

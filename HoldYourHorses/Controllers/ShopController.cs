using HoldYourHorses.Services.Interfaces;
using HoldYourHorses.ViewModels.Shop;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Controllers
{
    public class ShopController : Controller
    {
        private readonly IShopService dataService;

        public ShopController(IShopService dataService)
        {
            this.dataService = dataService;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> IndexAsync(string search)
        {
            IndexVM model = await dataService.GetIndexVMAsync(search);
            return View(model);
        }

        [HttpGet("article/{articleNr}")]
        public IActionResult ArticleDetails(int articleNr)
        {
            ArticleDetailsVM model = dataService.GetArticleDetailsVM(articleNr);
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

            dataService.SaveOrder(checkoutVM);
            dataService.ClearCart();
            return RedirectToAction(nameof(OrderConfirmation));
        }

        [HttpGet("IndexPartial")]
        public IActionResult IndexPartial(int minPrice, int maxPrice, int maxHK, int minHK, string typer, string materials, bool isAscending, string sortOn)
        {
            IndexPartialVM[] model = dataService.GetIndexPartial(minPrice, maxPrice, minHK, maxHK, typer, materials, isAscending, sortOn);
            return PartialView("_IndexPartial", model);
        }

        [HttpGet("shopping-cart")]
        public IActionResult ShoppingCart()
        {
            ShoppingCartVM[] model = dataService.GetShoppingCartVM();
            return View(model);
        }

        [HttpGet("order-confirmation")]
        public IActionResult OrderConfirmation()
        {
            return View(dataService.GetOrderConfirmationVM());
        }

        [HttpGet("Compare")]
        public async Task<IActionResult> CompareAsync()
        {
            CompareVM[] model = await dataService.getCompareVMAsync();
            return View(model);
        }
    }
}

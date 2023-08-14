using HoldYourHorses.Exceptions;
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

        [HttpGet("product/{articleNr}")]
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
            else
                dataService.SaveOrder(checkoutVM);
            return RedirectToAction(nameof(Kvitto));
        }

        [HttpGet("update-shopping-cart")]
        public IActionResult updateShoppingCart(int articleNr, int amount, string articleName, string price)
        {
            // Remove all non-numeric characters from the price string
            string sanitizedPrice = new string(price.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(sanitizedPrice))
            {
                return BadRequest("Invalid price parameter.");
            }

            if (!int.TryParse(sanitizedPrice, out int priceInt))
            {
                return BadRequest("Invalid price parameter format.");
            }

            if (amount < 0 || amount > 100)
            {
                return BadRequest("Invalid amount parameter. Amount must be between 1 and 99.");
            }

            dataService.AddToCart(articleNr, amount, articleName, priceInt);
            return Ok();
        }


        [HttpDelete("remove-from-shopping-cart/{articleNr}")]
        public IActionResult Kassa(int articleNr)
        {
            try
            {
                dataService.RemoveItemFromShoppingCart(articleNr);
                return Ok();
            }
            catch (BadRequestException)
            {
                return NotFound();
            }
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

        [HttpDelete("clear-cart")]
        public IActionResult ClearCart()
        {
            dataService.ClearCart();
            return Ok();
        }

        [HttpGet("kvitto")]
        public IActionResult Kvitto()
        {
            return View(dataService.GetReceipt());
        }
        [HttpGet("jämför")]
        public async Task<IActionResult> CompareAsync()
        {
            CompareVM[] model = await dataService.getCompareVMAsync();
            return View(model);
        }

        [HttpGet("compareAdd")]
        public IActionResult CompareAdd(int artikelnr)
        {
            return Content(dataService.addCompare(artikelnr).ToString());
        }
        [HttpGet("getCompare")]
        public IActionResult GetCompare()
        {
            string model = dataService.getCompare();
            return Content(model);
        }
        [HttpGet("removeCompare")]
        public IActionResult removeCompare()
        {
            dataService.removeCompare();
            return Ok();
        }

        [HttpGet("addFavourite")]
        public IActionResult AddFavourite(int artikelnr)
        {
            return Content(dataService.AddFavourite(artikelnr).ToString());
        }

        [HttpGet("getFavourites")]
        public IActionResult GetFavourites()
        {
            var model = dataService.GetFavourites();
            return Content(model);
        }
    }
}

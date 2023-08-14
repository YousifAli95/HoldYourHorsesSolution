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
        public IActionResult Details(int articleNr)
        {
            DetailsVM model = dataService.GetDetailsVM(articleNr);
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

        [HttpGet("/uppdateravarukorg/")]
        public IActionResult Details(int artikelNr, int antalVaror, string artikelNamn, string price)
        {
            var p = price.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .Select(c => c.ToString())
            .Aggregate((a, b) => a + b);

            var pris = int.Parse(p);
            dataService.AddToCart(artikelNr, antalVaror, artikelNamn, pris);
            int numberOfProducts = dataService.AddToCart(artikelNr, antalVaror, artikelNamn, pris);
            return Content(numberOfProducts.ToString());
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

using HoldYourHorses.Exceptions;
using HoldYourHorses.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IShopService dataService;

        public ApiController(IShopService dataService)
        {
            this.dataService = dataService;
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
        public IActionResult RemoveArticleFromShoppingCart(int articleNr)
        {
            try
            {
                dataService.RemoveItemFromShoppingCart(articleNr);
            }
            catch (BadRequestException)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("clear-cart")]
        public IActionResult ClearCart()
        {
            dataService.ClearCart();
            return Ok();
        }

        [HttpGet("compare-add")]
        public IActionResult CompareAdd(int artikelnr)
        {
            return Content(dataService.addCompare(artikelnr).ToString());
        }

        [HttpGet("get-compare")]
        public IActionResult GetCompare()
        {
            string model = dataService.getCompare();
            return Content(model);
        }
        [HttpGet("remove-compare")]
        public IActionResult removeCompare()
        {
            dataService.removeCompare();
            return Ok();
        }

        [HttpGet("add-favourite")]
        public IActionResult AddFavourite(int artikelnr)
        {
            return Content(dataService.AddFavourite(artikelnr).ToString());
        }

        [HttpGet("get-favourites")]
        public IActionResult GetFavourites()
        {
            var model = dataService.GetFavourites();
            return Content(model);
        }
    }
}

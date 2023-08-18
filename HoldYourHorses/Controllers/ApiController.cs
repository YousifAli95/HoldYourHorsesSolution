using HoldYourHorses.Exceptions;
using HoldYourHorses.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IApiService _apiService;

        public ApiController(IApiService dataService)
        {
            this._apiService = dataService;
        }

        [HttpGet("update-shopping-cart")]
        public IActionResult UpdateShoppingCart(int articleNr, int amount, string articleName, string price)
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

            _apiService.AddToCart(articleNr, amount, articleName, priceInt);
            return Ok();
        }

        [HttpDelete("remove-from-shopping-cart/{articleNr}")]
        public IActionResult RemoveArticleFromShoppingCart(int articleNr)
        {
            try
            {
                _apiService.RemoveItemFromShoppingCart(articleNr);
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
            _apiService.ClearCart();
            return Ok();
        }

        [HttpGet("compare-add")]
        public IActionResult CompareAdd(int articleNr)
        {
            return Content(_apiService.AddCompare(articleNr).ToString());
        }

        [HttpGet("get-compare")]
        public IActionResult GetCompare()
        {
            string model = _apiService.GetCompare();
            return Content(model);
        }
        [HttpGet("remove-compare")]
        public IActionResult RemoveCompare()
        {
            _apiService.RemoveCompare();
            return Ok();
        }

        [HttpGet("add-favourite")]
        public IActionResult AddFavourite(int articleNr)
        {
            return Content(_apiService.AddFavourite(articleNr).ToString());
        }

        [HttpGet("get-favourites")]
        public IActionResult GetFavourites()
        {
            var model = _apiService.GetFavourites();
            return Content(model);
        }
    }
}

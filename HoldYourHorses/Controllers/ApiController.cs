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
        public IActionResult UpdateShoppingCart(int articleNr, int amount)
        {
            if (amount < 0 || amount > 100)
            {
                return BadRequest("Invalid amount parameter. Amount must be between 1 and 99.");
            }

            _apiService.AddToCart(articleNr, amount);
            return Ok(new { message = "Shopping cart updated successfully." });
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
            return Ok(new { message = "Article removed from shopping cart." });
        }

        [HttpDelete("clear-cart")]
        public IActionResult ClearCart()
        {
            _apiService.ClearCart();
            return Ok(new { message = "Shopping Cart cleared successfully" });

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
        [HttpDelete("remove-all-comparisons")]
        public IActionResult RemoveAllComparisons()
        {
            _apiService.RemoveAllComparisons();
            return Ok(new { message = "All comparison markings removed successfully" });

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

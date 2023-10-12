using HoldYourHorses.Exceptions;
using HoldYourHorses.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Controllers
{
    [ApiController]
    public class RestApiController : ControllerBase
    {
        private readonly IApiService _restApiService;

        public RestApiController(IApiService dataService)
        {
            this._restApiService = dataService;
        }

        [HttpGet("update-shopping-cart")]
        public IActionResult UpdateShoppingCart(int articleNr, int amount)
        {
            if (amount < 0 || amount > 100)
            {
                return BadRequest("Invalid amount parameter. Amount must be between 1 and 99.");
            }

            _restApiService.AddToCart(articleNr, amount);
            return Ok(new { message = "Shopping cart updated successfully." });
        }

        [HttpDelete("remove-from-shopping-cart/{articleNr}")]
        public IActionResult RemoveArticleFromShoppingCart(int articleNr)
        {
            try
            {
                _restApiService.RemoveItemFromShoppingCart(articleNr);
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
            _restApiService.ClearCart();
            return Ok(new { message = "Shopping Cart cleared successfully" });

        }

        [HttpGet("add-or-remove-compare")]
        public IActionResult AddOrRemoveCompare(int articleNr)
        {
            bool added = _restApiService.AddOrRemoveCompare(articleNr);
            string message = added ? "Item added successfully." : "Item removed successfully.";

            return Ok(new { added = added, message = message });
        }

        [HttpGet("get-compare")]
        public IActionResult GetCompare()
        {
            var model = _restApiService.GetCompare();
            return Ok(new { compareData = model });
        }
        [HttpDelete("remove-all-comparisons")]
        public IActionResult RemoveAllComparisons()
        {
            _restApiService.RemoveAllComparisons();
            return Ok(new { message = "All comparison markings removed successfully" });
        }

        [HttpGet("add-or-remove-favourite/{articleNr}")]
        public IActionResult AddFavourite(int articleNr)
        {
            bool added = _restApiService.AddOrRemoveFavourite(articleNr);
            string message = added ? "Favourite item added successfully." : "Favourite item removed successfully.";

            return Ok(new { added = added, message = message });
        }

        [HttpGet("get-favourites")]
        public IActionResult GetFavourites()
        {
            var model = _restApiService.GetFavourites();
            return Content(model);
        }

        [HttpGet("articles")]
        public async Task<IActionResult> GetArticleAsync()
        {
            var articles = await _restApiService.GetArticles();
            return Ok(articles);
        }
    }
}

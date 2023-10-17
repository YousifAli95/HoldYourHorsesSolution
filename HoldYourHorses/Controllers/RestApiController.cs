using HoldYourHorses.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Controllers
{
    [ApiController]
    [Route("api")]
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
            const int maxAmount = 100;
            const int minAmount = 0;

            if (amount < minAmount || amount > maxAmount)
            {
                return BadRequest($"Invalid amount parameter. Amount must be between {minAmount + 1} and {maxAmount - 1}.");
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
            catch (BadHttpRequestException)
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

        [HttpGet("add-or-remove-compare/{articleNr}")]
        public IActionResult AddOrRemoveCompare(int articleNr)
        {
            bool added = _restApiService.AddOrRemoveCompare(articleNr);
            string message = added ? "Item added successfully." : "Item was removed successfully.";

            return Ok(new { added = added, message = message });
        }

        [HttpGet("get-compare")]
        public IActionResult GetCompare()
        {
            var articleNrList = _restApiService.GetCompare();
            return Ok(new { compareData = articleNrList });
        }

        [HttpDelete("remove-all-comparisons")]
        public IActionResult RemoveAllComparisons()
        {
            _restApiService.RemoveAllComparisons();
            return Ok(new { message = "All comparison markings removed successfully" });
        }

        [HttpGet("add-or-remove-favourite/{articleNr}")]
        public async Task<IActionResult> AddFavourite(int articleNr)
        {
            bool added = await _restApiService.AddOrRemoveFavourite(articleNr);
            string message = added ? "Favourite item added successfully." : "Favourite item removed successfully.";

            return Ok(new { added = added, message = message });
        }

        [HttpGet("favourites")]
        public async Task<IActionResult> GetFavourites()
        {
            var favourites = await _restApiService.GetFavourites();
            return Ok(favourites);
        }

        [HttpGet("articles")]
        public async Task<IActionResult> GetArticle()
        {
            var articles = await _restApiService.GetArticles();
            return Ok(articles);
        }
    }
}

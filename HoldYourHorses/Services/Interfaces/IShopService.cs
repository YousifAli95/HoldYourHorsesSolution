using HoldYourHorses.ViewModels.Shop;

namespace HoldYourHorses.Services.Interfaces
{
    public interface IShopService
    {
        /// <summary>
        /// Asynchronously retrieves an array of ShoppingCartVM objects representing the items in the user's shopping cart.
        /// </summary>
        /// <returns>
        /// An array of ShoppingCartVM objects, each containing information such as article number, name, price, and quantity.
        /// </returns>
        Task<ShoppingCartVM[]> GetShoppingCartVM();

        Task SaveOrder(CheckoutVM checkoutVM);

        ArticleDetailsVM GetArticleDetailsVM(int articleNr);

        OrderConfirmationVM GetOrderConfirmationVM();


        Task<IndexVM> GetIndexVM(string search);

        IndexPartialVM[] GetIndexPartial(int minPrice, int maxPrice, int minHorsePower, int maxHorsePower, string types, string materials, bool isAscending, string sortOn);

        Task<CompareVM[]> GetCompareVM();

        int GetShoppingCartCount();


    }
}

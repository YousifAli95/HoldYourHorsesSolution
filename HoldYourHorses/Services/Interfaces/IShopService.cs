using HoldYourHorses.ViewModels.Shop;

namespace HoldYourHorses.Services.Interfaces
{
    public interface IShopService
    {
        Task SaveOrder(CheckoutVM checkoutVM);

        ArticleDetailsVM GetArticleDetailsVM(int articleNr);

        OrderConfirmationVM GetOrderConfirmationVM();

        Task<ShoppingCartVM[]> GetShoppingCartVM();

        Task<IndexVM> GetIndexVM(string search);

        IndexPartialVM[] GetIndexPartial(int minPrice, int maxPrice, int minHorsePower, int maxHorsePower, string types, string materials, bool isAscending, string sortOn);

        Task<CompareVM[]> GetCompareVM();

        int GetShoppingCartCount();


    }
}

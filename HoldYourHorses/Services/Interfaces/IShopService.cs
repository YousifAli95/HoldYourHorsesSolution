using HoldYourHorses.ViewModels.Shop;

namespace HoldYourHorses.Services.Interfaces
{
    public interface IShopService
    {
        void SaveOrder(CheckoutVM checkoutVM);

        ArticleDetailsVM GetArticleDetailsVM(int articleNr);

        OrderConfirmationVM GetOrderConfirmationVM();

        ShoppingCartVM[] GetShoppingCartVM();

        Task<IndexVM> GetIndexVMAsync(string search);

        IndexPartialVM[] GetIndexPartial(int minPrice, int maxPrice, int minHorsePower, int maxHorsePower, string types, string materials, bool isAscending, string sortOn);

        Task<CompareVM[]> GetCompareVMAsync();


    }
}

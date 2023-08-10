using HoldYourHorses.ViewModels.Shop;

namespace HoldYourHorses.Services.Interfaces
{
    public interface IShopService
    {
        void SaveOrder(CheckoutVM checkoutVM);

        DetailsVM GetDetailsVM(int artikelNr);

        KvittoVM GetReceipt();

        int AddToCart(int artikelNr, int antalVaror, string arikelNamn, int pris);

        void ClearCart();

        void DeleteItem(int artikelNr);

        KassaVM[] GetKassaVM();

        Task<IndexVM> GetIndexVMAsync(string search);

        IndexPartialVM[] GetIndexPartial(int minPrice, int maxPrice, int minHK, int maxHK, string typer, string materials, bool isAscending, string sortOn);

        int GetCart();

        bool addCompare(int artikelnr);

        Task<CompareVM[]> getCompareVMAsync();

        string getCompare();

        void removeCompare();

        bool AddFavourite(int artikelnr);

        string GetFavourites();
    }
}

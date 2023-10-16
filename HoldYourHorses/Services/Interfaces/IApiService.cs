namespace HoldYourHorses.Services.Interfaces
{
    public interface IApiService
    {
        void AddToCart(int articleNr, int amount);
        void RemoveItemFromShoppingCart(int articleNr);
        bool AddOrRemoveCompare(int articleNr);
        int[] GetCompare();
        void RemoveAllComparisons();
        Task<bool> AddOrRemoveFavourite(int articleNr);
        Task<string> GetFavourites();
        int GetNumberOfItemsInCart();
        void ClearCart();

        Task<string[]> GetArticles();

    }
}

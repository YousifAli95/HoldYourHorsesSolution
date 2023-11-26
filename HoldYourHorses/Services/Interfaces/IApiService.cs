namespace HoldYourHorses.Services.Interfaces
{
    public interface IApiService
    {
        void AddArticleToShoppingCart(int articleNr, int amount);
        void RemoveArticleFromShoppingCart(int articleNr);
        bool AddOrRemoveCompare(int articleNr);
        int[] GetCompareArticleNrArray();
        void RemoveAllComparisons();
        Task<bool> AddOrRemoveFavourite(int articleNr);
        Task<string> GetFavourites();
        int GetNumberOfItemsInCart();
        void ClearShoppingCart();

        Task<string[]> GetArticles();

    }
}

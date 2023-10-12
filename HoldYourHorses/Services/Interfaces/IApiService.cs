namespace HoldYourHorses.Services.Interfaces
{
    public interface IApiService
    {
        void AddToCart(int articleNr, int amount);
        void RemoveItemFromShoppingCart(int articleNr);
        bool AddOrRemoveCompare(int articleNr);
        int[] GetCompare();
        void RemoveAllComparisons();
        bool AddOrRemoveFavourite(int articleNr);
        string GetFavourites();
        int GetNumberOfItemsInCart();
        void ClearCart();

        Task<string[]> GetArticles();

    }
}

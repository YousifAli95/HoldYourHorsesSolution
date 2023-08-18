namespace HoldYourHorses.Services.Interfaces
{
    public interface IApiService
    {
        void AddToCart(int articleNr, int amount);
        void RemoveItemFromShoppingCart(int articleNr);
        bool AddCompare(int articleNr);
        string GetCompare();
        void RemoveAllComparisons();
        bool AddFavourite(int articleNr);
        string GetFavourites();
        int GetNumberOfItemsInCart();
        void ClearCart();

    }
}

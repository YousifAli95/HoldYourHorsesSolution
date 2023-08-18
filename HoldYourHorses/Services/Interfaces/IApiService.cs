namespace HoldYourHorses.Services.Interfaces
{
    public interface IApiService
    {
        void AddToCart(int articleNr, int amount, string articleName, int price);
        void RemoveItemFromShoppingCart(int articleNr);
        bool AddCompare(int articleNr);
        string GetCompare();
        void RemoveCompare();
        bool AddFavourite(int articleNr);
        string GetFavourites();
        int GetCart();
        void ClearCart();

    }
}

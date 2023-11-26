using HoldYourHorses.ViewModels.Account;

namespace HoldYourHorses.Services.Interfaces
{
    public interface IAccountService
    {
        Task<UserpageVM> GetUserPageVM(string name);

        Task<string> TryRegister(RegisterVM viewModel);

        Task<bool> TryLogin(LoginVM viewModel);

        Task LogOutUserAsync();

        Task<OrderhistoryVM> GetOrderHistoryVM();

        /// <summary>
        /// Asynchronously gets the user ID as a string if the user is logged in.
        /// Returns null if the user is not logged in.
        /// </summary>
        Task<string> GetUserId();

    }
}

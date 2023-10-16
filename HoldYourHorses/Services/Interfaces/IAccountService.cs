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

        Task<string> GetUserId();
    }
}

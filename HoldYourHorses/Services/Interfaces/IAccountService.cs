using HoldYourHorses.ViewModels.Account;

namespace HoldYourHorses.Services.Interfaces
{
    public interface IAccountService
    {
        Task<UserpageVM> GetUserPageVMAsync(string name);

        Task<string> TryRegisterAsync(RegisterVM viewModel);

        Task<bool> TryLogin(LoginVM viewModel);

        Task LogOutUserAsync();

        Task<OrderhistoryVM> GetOrderHistory();
    }
}

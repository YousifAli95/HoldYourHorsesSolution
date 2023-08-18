using HoldYourHorses.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Views.Shared.Components.ShoppingCart
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IApiService dataService;

        public ShoppingCartViewComponent(IApiService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(dataService.GetCart());
        }
    }
}

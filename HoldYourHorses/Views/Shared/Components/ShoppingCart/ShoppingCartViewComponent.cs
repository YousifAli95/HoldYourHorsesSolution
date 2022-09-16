using HoldYourHorses.Models;
using Microsoft.AspNetCore.Mvc;

namespace HoldYourHorses.Views.Shared.Components.ShoppingCart
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly DataService dataService;

        public ShoppingCartViewComponent(DataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(dataService.GetCart());
        }
    }
}

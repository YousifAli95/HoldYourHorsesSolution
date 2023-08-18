using HoldYourHorses.Models.Entities;
using HoldYourHorses.Services.Implementations;
using HoldYourHorses.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IShopService, ShopServiceDB>();
builder.Services.AddTransient<IAccountService, AccountServiceDB>();
builder.Services.AddTransient<IApiService, ApiServiceDB>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
});

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SticksDBContext>(o => o.UseSqlServer(connString));
builder.Services.AddDbContext<IdentityDbContext>(o => o.UseSqlServer(connString));
builder.Services.AddSession();

ConfigureIdentity(builder);

var app = builder.Build();

app.UseSession();
app.UseCookiePolicy();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(o => o.MapControllers());
app.Run();

static void ConfigureIdentity(WebApplicationBuilder builder)
{
    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
        .AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(o => o.LoginPath = "/Index");

    builder.Services.Configure<IdentityOptions>(options =>
    {
        // Changed Password settings.
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 0;
    });
}
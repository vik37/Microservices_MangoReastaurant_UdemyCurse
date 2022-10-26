using Mango.Web;
using Mango.Web.Services;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
var services = builder.Services;
// Add services to the container.
services.AddControllersWithViews();

services.AddRazorPages()
                .AddRazorRuntimeCompilation();

services.AddHttpClient<IProductService, ProductService>();
SD.ProductAPIBase = configuration["ServiceUrls:ProductAPI"];

services.AddScoped<IProductService,ProductService>();

services.AddAuthentication(opt =>
{
    opt.DefaultScheme = "Cookies";
    opt.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies",c => c.ExpireTimeSpan = TimeSpan.FromMinutes(10))
    .AddOpenIdConnect("oidc",opt =>
    {
        opt.Authority = configuration["ServiceUrls:IdentityAPI"];
        opt.GetClaimsFromUserInfoEndpoint = true;
        opt.ClientId = "mango";
        opt.ClientSecret = "secret";
        opt.ResponseType = "code";
        opt.ClaimActions.MapJsonKey("role", "role", "role");
        opt.ClaimActions.MapJsonKey("sub", "sub", "sub");
        opt.TokenValidationParameters.NameClaimType = "name";
        opt.TokenValidationParameters.RoleClaimType = "role";
        opt.Scope.Add("mango");
        opt.SaveTokens = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

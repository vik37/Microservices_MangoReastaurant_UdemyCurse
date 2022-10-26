using Mango.Services.Identity;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.DbContexts.Initializer;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
services.AddControllersWithViews();
services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
services.AddIdentity<ApplicationUser,IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

var identityBuilder = services.AddIdentityServer(opt =>
{
    opt.Events.RaiseErrorEvents = true;
    opt.Events.RaiseInformationEvents = true;
    opt.Events.RaiseFailureEvents = true;
    opt.Events.RaiseSuccessEvents = true;
    opt.EmitStaticAudienceClaim = true;
})
.AddInMemoryIdentityResources(SD.IdentityResources)
.AddInMemoryApiScopes(SD.ApiScopes)
.AddInMemoryClients(SD.Clients)
.AddAspNetIdentity<ApplicationUser>();

identityBuilder.AddDeveloperSigningCredential();

services.AddScoped<IDbInitializer, DbInitializer>();

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
app.UseIdentityServer();

app.UseAuthorization();
SeedHelper.SeedDb(app);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
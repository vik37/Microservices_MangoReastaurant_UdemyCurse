using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.DbContexts.Initializer;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.Identity
{
    public static class SeedHelper
    {
        public static void SeedDb(WebApplication app)
        {
            var context = app.Services.CreateScope().ServiceProvider.GetService<ApplicationDbContext>();

            var userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<IdentityRole>>();

            var dbInitializer = new DbInitializer(context, userManager, roleManager);

            dbInitializer.Initialize();
        }
    }
}

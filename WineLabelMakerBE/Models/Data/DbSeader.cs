using Microsoft.AspNetCore.Identity;
using WineLabelMakerBE.Models.Entity;

//Using the DbSeader class to create the admin
//DbSeeder is used to automatically create initial data in the database when the app starts
//It works using Identity Services and is executed at app startup via Program.cs
//Sensitive data in appsettings.Development.json - gitignore

namespace WineLabelMakerBE.Models.Data
{
    public static class DbSeader
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var adminSection = configuration.GetSection("Seeder:Admin");

            var email = adminSection["Email"];
            var password = adminSection["Password"];
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Name = adminSection["Name"],
                    Surname = adminSection["Surname"],
                    CompanyName = adminSection["CompanyName"],
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }

}


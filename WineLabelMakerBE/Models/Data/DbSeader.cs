using Microsoft.AspNetCore.Identity;
using WineLabelMakerBE.Models.Entity;

//Utilizzo della classe DbSeader per la creazione dell'admin
//DbSeeder serve a creare automaticamente dati iniziali nel database quando l'app parte
//Funziona grazie ai servizi di Identity e viene eseguito all'avvio dell'app tramite Program.cs.
//Dati sentibili in appsetting.Development.json- gitignore 
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


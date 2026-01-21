using Microsoft.AspNetCore.Identity;
using WineLabelMakerBE.Models.Entity;

//Utilizzo della classe DbSeader per la creazione dell'admin
//DbSeeder serve a creare automaticamente dati iniziali nel database quando l'app parte
//Funziona grazie ai servizi di Identity e viene eseguito all'avvio dell'app tramite Program.cs.
namespace WineLabelMakerBE.Models.Data
{
    public static class DbSeader
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            var email = "WineLabelMaker@gmail.com";

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Name = "Rebecca",
                    Surname = "Matarozzo",
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Birthday = new DateTime(1998, 12, 9),
                    IsDeleted = false
                };

                await userManager.CreateAsync(user, "Admin123!");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }

}


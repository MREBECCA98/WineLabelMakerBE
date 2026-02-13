using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WineLabelMakerBE.Models.Entity;

//ApplicationDbContext is the class that manages the application's database
//It inherits from IdentityDbContext because it includes everything needed for users and roles

namespace WineLabelMakerBE.Models.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Request> Requests { get; set; }
        //public DbSet<Message> Messages { get; set; }
        public DbSet<ApplicationUser> AspNetUsers { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //OnModelCreating serve a configurare il comportamento delle entity nel database,
        //proteggendo i dati dall'eliminazione a cascata accidentale:
        //- Una Request è collegata a un User tramite UserId, e se l'utente viene cancellato 
        //  le richieste non vengono eliminate automaticamente (DeleteBehavior.Restrict)
        //- Una Message è collegata a un User tramite UserId, e se l'utente viene cancellato 
        //  i messaggi non vengono eliminati automaticamente (DeleteBehavior.Restrict)
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Message>()
            //    .HasOne(m => m.User)
            //    .WithMany()
            //    .HasForeignKey(m => m.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

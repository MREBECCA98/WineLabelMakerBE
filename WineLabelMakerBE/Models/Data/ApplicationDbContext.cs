using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WineLabelMakerBE.Models.Entity;

//ApplicationDbContext è la classe che gestisce il database dell'applicazione
//Eredita da IdentityDbContext perchè include tutto il necessario per utenti e ruoli
namespace WineLabelMakerBE.Models.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //DbSet viene utilizzato per inserire le entità nel database
        public DbSet<Request> Requests { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ApplicationUser> AspNetUsers { get; set; }

        //Costruttore che accetta le opzioni del contesto e le passa alla classe base DbContext
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

            builder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

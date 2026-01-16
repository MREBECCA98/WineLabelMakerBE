using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WineLabelMakerBE.Models.Entity;

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Qui diciamo a EF Core di NON fare cascade delete
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

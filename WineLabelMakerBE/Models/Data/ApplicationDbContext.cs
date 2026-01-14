using Microsoft.EntityFrameworkCore;
using WineLabelMakerBE.Models.Entity;

namespace WineLabelMakerBE.Models.Data
{
    public class ApplicationDbContext : DbContext
    {
        //DbSet viene utilizzato per inserire le entità nel database
        public DbSet<Request> Requests { get; set; }
        public DbSet<Message> Messages { get; set; }

        //Costruttore che accetta le opzioni del contesto e le passa alla classe base DbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace GradeExpertCRM.Models.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Calculation> Calculations { get; set; }
        public DbSet<DismantlingWork> DismantlingWorks { get; set; }
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<DetailsSettings> DetailsSettings { get; set; }
        public DbSet<OverallCalculation> OverallCalculations { get; set; }

        public AppDbContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            //Task.Run(async () => await Database.EnsureCreatedAsync());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(App.ConnectionString);
        }
    }
}

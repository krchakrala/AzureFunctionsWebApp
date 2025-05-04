using AzureHttpTriggerFunction.Models;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;


namespace AzureHttpTriggerFunction.Data
{
    public class AzureFunctionDbContext : DbContext
    {
        public AzureFunctionDbContext(DbContextOptions<AzureFunctionDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<SalesRequest> SalesRequests { get; set; }
        public DbSet<GroceryItem> GroceryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SalesRequest>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            modelBuilder.Entity<GroceryItem>(entity =>
            {
                entity.HasKey(c => c.Id);
            });
        }
    }
}

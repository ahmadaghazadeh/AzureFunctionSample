using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace FunctionApp
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Use a default connection string for migrations
            var connectionString =
                "Server=localhost;Database=azureFunction;User Id=sadmin; Password=OrderClubPass!!!;TrustServerCertificate=True;Encrypt=false;MultipleActiveResultSets=true;";
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
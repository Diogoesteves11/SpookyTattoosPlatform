using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SpookyTattoos.Infrastructure.Persistence;

public class SpookyTattoosDbContextFactory : IDesignTimeDbContextFactory<SpookyTattoosDbContext>
{
    public SpookyTattoosDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SpookyTattoosDbContext>();

        var connectionString = "Host=localhost;Port=5432;Database=spookytattoos_db;Username=admin;Password=spooky_db_password_123";

        optionsBuilder.UseNpgsql(connectionString)
                      .UseSnakeCaseNamingConvention();

        return new SpookyTattoosDbContext(optionsBuilder.Options);
    }
}
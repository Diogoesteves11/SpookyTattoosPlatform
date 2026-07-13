using Microsoft.EntityFrameworkCore;
using SpookyTattoos.Infrastructure.Persistence; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SpookyTattoosDbContext>(options =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention());
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SpookyTattoosDbContext>();
    
    Console.WriteLine("Waiting for database init...");
    Task.Delay(5000).Wait(); 
    
    try 
    {
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Applying migrations...");
            dbContext.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Critical error on migrations: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
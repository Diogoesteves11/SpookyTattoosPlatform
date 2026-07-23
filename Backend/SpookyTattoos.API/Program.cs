/*
Copyright 2026 Diogo Esteves, Guilherme Mattos

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

using Minio;
using SpookyTattoos.Infrastructure.External.Storage;
using SpookyTattoos.Infrastructure.External.Email;

using SpookyTattoos.Infrastructure.Persistence; 
using SpookyTattoos.Domain.Repositories;
using SpookyTattoos.Infrastructure.Repositories;
using SpookyTattoos.Application.Interfaces.External;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SpookyTattoosDbContext>(options =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention());

// --- Repositories ---
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IPiercingRepository, PiercingRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPromoRepository, PromoRepository>();
builder.Services.AddScoped<ITattooRepository, TattooRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();

// --- Unit of Work ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- Security & External ---
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// --- Application Services ---
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IPiercingService, PiercingService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPromoService, PromoService>();
builder.Services.AddScoped<ITattooService, TattooService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();

// --- Configuração do MinIO ---
var minioEndpoint = builder.Configuration["Minio:Endpoint"] ?? "localhost:9000";
var minioAccessKey = builder.Configuration["MINIO_ROOT_USER"]; 
var minioSecretKey = builder.Configuration["MINIO_ROOT_PASSWORD"];

builder.Services.AddMinio(configureClient => configureClient
    .WithEndpoint(minioEndpoint)
    .WithCredentials(minioAccessKey, minioSecretKey)
    .Build());

builder.Services.AddScoped<IStorageService, MinioStorageService>();

// --- Configuração do JWT ---
var jwtSecret = builder.Configuration["JWT_SECRET_KEY"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT_SECRET_KEY not configured");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                
                var jti = context.Principal?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrEmpty(jti) && await authService.IsTokenRevokedAsync(jti))
                {
                    context.Fail("Revoked Token");
                }
            }
        };
    });


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

app.UseAuthentication();
app.UseAuthorization(); 

app.MapControllers();
app.Run();
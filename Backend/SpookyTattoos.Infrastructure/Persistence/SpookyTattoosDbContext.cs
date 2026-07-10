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
using SpookyTattoos.Domain.Entities;

namespace SpookyTattoos.Infrastructure.Persistence;

public class SpookyTattoosDbContext : DbContext
{
    public SpookyTattoosDbContext(DbContextOptions<SpookyTattoosDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Tattoo> Tattoos { get; set; }
    public DbSet<Piercing> Piercings { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostImage> PostImages { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Promo> Promos { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Admin>().HasIndex(a => a.Username).IsUnique();
        modelBuilder.Entity<Admin>().HasIndex(a => a.Email).IsUnique();
        
        modelBuilder.Entity<Client>().HasIndex(c => c.Email).IsUnique();
        

        modelBuilder.Entity<Client>().HasIndex(c => c.GhostPoints).IsDescending();

        modelBuilder.Entity<Job>().Property(j => j.Status).HasConversion<string>();
        modelBuilder.Entity<Job>().Property(j => j.Type).HasConversion<string>();
        modelBuilder.Entity<Tattoo>().Property(t => t.Style).HasConversion<string>();
        modelBuilder.Entity<Piercing>().Property(p => p.Type).HasConversion<string>();
        modelBuilder.Entity<Piercing>().Property(p => p.BodyPart).HasConversion<string>();

        
        modelBuilder.Entity<Tattoo>()
            .HasOne(t => t.Job)
            .WithMany(j => j.Tattoos)
            .HasForeignKey(t => t.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Piercing>()
            .HasOne(p => p.Job)
            .WithMany(j => j.Piercings)
            .HasForeignKey(p => p.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Job>()
            .HasOne(j => j.Client)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Job)
            .WithOne(j => j.CatalogPost)
            .HasForeignKey<Post>(p => p.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PostImage>()
            .HasOne(pi => pi.Post)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        
        modelBuilder.Entity<Tattoo>().ToTable(t => 
        {
            t.HasCheckConstraint("CK_Tattoo_FillScore", "\"FillScore\" BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_Tattoo_ShadowScore", "\"ShadowScore\" BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_Tattoo_DetailScore", "\"DetailScore\" BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_Tattoo_BodyZoneScore", "\"BodyZoneScore\" BETWEEN 1 AND 5");
        });

        modelBuilder.Entity<Event>().ToTable(e => 
            e.HasCheckConstraint("CK_Event_Dates", "\"EndDate\" >= \"StartDate\""));
            
        modelBuilder.Entity<Promo>().ToTable(p => 
            p.HasCheckConstraint("CK_Promo_Dates", "\"EndDate\" >= \"StartDate\""));

        modelBuilder.Entity<Coupon>()
            .HasOne(c => c.Client)
            .WithMany(cl => cl.Coupons)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Coupon>()
            .HasOne(c => c.Promo)
            .WithMany() 
            .HasForeignKey(c => c.PromoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Voucher>()
            .HasOne(v => v.Emitter)
            .WithMany(c => c.IssuedVouchers)
            .HasForeignKey(v => v.EmitterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
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

using System;
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


        modelBuilder.Entity<Admin>().ToTable("admins");
        modelBuilder.Entity<Client>().ToTable("clients");
        modelBuilder.Entity<Job>().ToTable("jobs");
        modelBuilder.Entity<Piercing>().ToTable("job_piercing_details");
        modelBuilder.Entity<Post>().ToTable("post_catalog_items");
        modelBuilder.Entity<PostImage>().ToTable("post_final_images");
        modelBuilder.Entity<Coupon>().ToTable("coupons");
        modelBuilder.Entity<Voucher>().ToTable("vouchers");


        modelBuilder.Entity<Tattoo>().Property(t => t.FillScore).HasColumnName("fill");
        modelBuilder.Entity<Tattoo>().Property(t => t.ShadowScore).HasColumnName("shadow");
        modelBuilder.Entity<Tattoo>().Property(t => t.DetailScore).HasColumnName("detail");
        modelBuilder.Entity<Tattoo>().Property(t => t.BodyZoneScore).HasColumnName("body_zone");

        modelBuilder.Entity<Admin>().HasIndex(a => a.Username).IsUnique();
        modelBuilder.Entity<Admin>().HasIndex(a => a.Email).IsUnique();
        
        modelBuilder.Entity<Client>().HasIndex(c => c.Email).IsUnique();
        modelBuilder.Entity<Client>().HasIndex(c => c.GhostPoints).IsDescending();

        modelBuilder.Entity<Job>().Property(j => j.Status).HasConversion<string>();
        modelBuilder.Entity<Job>().Property(j => j.Type).HasConversion<string>();
        modelBuilder.Entity<Tattoo>().Property(t => t.Style).HasConversion<string>();
        modelBuilder.Entity<Piercing>().Property(p => p.Type).HasConversion<string>();
        modelBuilder.Entity<Piercing>().Property(p => p.BodyPart).HasConversion<string>();


        modelBuilder.Entity<Admin>().HasData(
            new Admin 
            { 
                Id = 1, 
                Username = "admin", 
                Email = "admin@spookytattoos.com",
                Password = "Admin123!",
                CreatedAt = new DateTimeOffset(2026, 7, 13, 12, 0, 0, TimeSpan.Zero) 
            }
        );
        

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


        modelBuilder.Entity<Tattoo>().ToTable("job_tattoo_details", t => 
        {
            t.HasCheckConstraint("CK_Tattoo_FillScore", "\"fill\" BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_Tattoo_ShadowScore", "\"shadow\" BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_Tattoo_DetailScore", "\"detail\" BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_Tattoo_BodyZoneScore", "\"body_zone\" BETWEEN 1 AND 5");
        });

        modelBuilder.Entity<Event>().ToTable("events", e => 
            e.HasCheckConstraint("CK_Event_Dates", "end_date >= start_date")); 
            
        modelBuilder.Entity<Promo>().ToTable("promos", p => 
            p.HasCheckConstraint("CK_Promo_Dates", "end_date >= start_date")); 
    }
}
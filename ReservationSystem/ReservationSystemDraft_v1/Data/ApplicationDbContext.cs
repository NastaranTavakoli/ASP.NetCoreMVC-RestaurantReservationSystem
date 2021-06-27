using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReservationSystemDraft_v1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Area> Areas { get; set; }
        public DbSet<Person> People { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationSource> ReservationSources { get; set; }

        public DbSet<ReservationStatus> ReservationStatuses { get; set; }
        public DbSet<ReservationTable> ReservationTables { get; set; }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Sitting> Sittings { get; set; }
        public DbSet<SittingType> SittingTypes { get; set; }
        public DbSet<SittingTemplate> SittingTemplates { get; set; }
        public DbSet<Table> Tables { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Restaurant>()
                .HasData(new Restaurant { Id = 1, Name = "Bean Scene", Address = "10 Bean St", Email = "info@beanscene.com.au", Phone = "+61212345678" });
            builder.Entity<SittingType>()
                .HasData(
                    new SittingType { Id = 1, Description = "Breakfast" },
                    new SittingType { Id = 2, Description = "Lunch" },
                    new SittingType { Id = 3, Description = "Dinner" }
                );
            builder.Entity<ReservationSource>()
                .HasData(
                    new ReservationSource { Id = 1, Description = "Online" },
                    new ReservationSource { Id = 2, Description = "Email" },
                    new ReservationSource { Id = 3, Description = "Phone" },
                    new ReservationSource { Id = 4, Description = "In Person" }
                );
            builder.Entity<ReservationStatus>()
                .HasData(
                    new ReservationStatus { Id = 1, Description = "Pending" },
                    new ReservationStatus { Id = 2, Description = "Confirmed" },
                    new ReservationStatus { Id = 3, Description = "Cancelled" },
                    new ReservationStatus { Id = 4, Description = "Seated" },
                    new ReservationStatus { Id = 5, Description = "Completed" }
                );
            builder.Entity<Person>()
                .HasIndex(p => p.Email)
                .IsUnique();
        }
    }
}

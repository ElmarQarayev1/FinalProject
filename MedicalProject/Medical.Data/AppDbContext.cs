using System;
using Medical.Core.Entities;
using Medical.Data.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Medical.Data
{
	public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }    

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<BasketItem> BasketItems { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Feature> Features { get; set; }

        public DbSet<Medicine> Medicines { get; set; }

        public DbSet<MedicineImage> MedicineImages { get; set; }

        public DbSet<MedicineReview> MedicineReviews { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Slider> Sliders { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Feed> Feeds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalimageConfiguration());
            modelBuilder.ApplyConfiguration(new SettingConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}


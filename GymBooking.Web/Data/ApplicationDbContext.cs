using GymBooking.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymBooking.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUserGymClass>().HasKey(t => new { t.ApplicationUserId, t.GymClassId });

            modelBuilder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        }
        public DbSet<GymBooking.Web.Models.GymClass> GymClass { get; set; } = default!;
        public DbSet<GymBooking.Web.Models.ApplicationUserGymClass> ApplicationUserGymClass{ get; set; } = default!;
       // public DbSet<GymBooking.Web.Models.ApplicationUser> ApplicationUser { get; set; } = default!;

    }

    internal class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(25);
            builder.Property(u=>u.LastName).HasMaxLength(25);
        }
    }
   



}
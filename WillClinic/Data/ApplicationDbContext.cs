using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WillClinic.Models;

namespace WillClinic.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<Veteran> Veterans { get; set; }
        public DbSet<VeteranChild> VeteranChildren { get; set; }
        public DbSet<VeteranIntakeForm> VeteranIntakeForms { get; set; }
        public DbSet<VeteranQueue> VeteranQueue { get; set; }
        public DbSet<LawyerAvailability> LawyerAvailability { get; set; }
        public DbSet<VeteranLawyerMatch> VeteranLawyerMatches { get; set; }
        public DbSet<Library> Libraries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // This code is Fluent API. Make sure you are reading documentation for the most current version of .Net Core.
            // At the time of writing .Net Core 2.x doesn't support many to many relationships ( .HasMany().WithMany() ) but it should
            // by about Q2 2018 according to .Net Core roadmap. This could be used to simplify a few things
            // As a rule of thumb please use descriptive names in lambda statements, such as lawyer => lawyer.VetLawMatches instead of
            // x => x.VetLawMatches in order to keep things easily understandable for future teams.

            builder.Entity<VeteranLawyerMatch>()
                .HasOne(vlm => vlm.Lawyer)
                .WithMany(law => law.VetLawMatches);
            builder.Entity<VeteranLawyerMatch>()
                .HasOne(vlm => vlm.Veteran)
                .WithOne(vet => vet.VetLawMatch);

            builder.Entity<VeteranLawyerMatch>()
                // Potentially use these two properties as a composite key instead of requiring match to have it's own ID.
            //    .HasKey(a => new { a.LawyerApplicationUserId, a.VeteranApplicationUserId });
                .HasKey(vlm => vlm.ID);

            builder.Entity<VeteranQueue>()
                .HasOne(vq => vq.Veteran)
                .WithOne();

           // builder.Entity<VeteranQueue>()
            //    .HasKey(a => a.VeteranApplicationUserId);
             //   .HasKey(vq => vq.ID);

            builder.Entity<LawyerAvailability>()
                .HasOne(la => la.Lawyer)
                .WithMany(law => law.Availability);

            builder.Entity<LawyerAvailability>()
                .HasKey(la => la.ID);

             builder.Entity<VeteranChild>()
                .HasOne(child => child.Veteran)
                .WithMany(vet => vet.Children);

            builder.Entity<VeteranChild>()
                .HasKey(a => a.ID);

            builder.Entity<VeteranIntakeForm>()
                .HasOne(form => form.Veteran)
                .WithMany(vet => vet.IntakeForms);

            builder.Entity<VeteranIntakeForm>()
                .HasKey(a => a.ID);

            builder.Entity<Veteran>()
                .HasOne(vet => vet.ApplicationUser)
                .WithOne();

            builder.Entity<Veteran>()
                .HasKey(a => a.ApplicationUserId);

            builder.Entity<Lawyer>()
                .HasOne(vet => vet.ApplicationUser)
                .WithOne();

            builder.Entity<Lawyer>()
                .HasKey(a => a.ApplicationUserId);

            builder.Entity<Admin>()
                .HasOne(vet => vet.ApplicationUser)
                .WithOne();

            builder.Entity<Admin>()
                .HasKey(a => a.ApplicationUserId);
        }

        public DbSet<WillClinic.Models.ApplicationUser> ApplicationUser { get; set; }
    }
}

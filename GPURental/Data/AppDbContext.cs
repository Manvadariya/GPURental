using GPURental.Models;
using Microsoft.EntityFrameworkCore;

namespace GPURental.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<GpuListing> GpuListings { get; set; }
        public DbSet<ListingImage> ListingImages { get; set; }
        public DbSet<RentalJob> RentalJobs { get; set; }
        public DbSet<WalletLedgerEntry> WalletLedgerEntries { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ListingTag> ListingTags { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ListingAvailability> ListingAvailabilities { get; set; }
        public DbSet<JobTelemetry> JobTelemetries { get; set; }
        public DbSet<JobLog> JobLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- MANY-TO-MANY RELATIONSHIP: GpuListing <-> Tag ---
            modelBuilder.Entity<ListingTag>().HasKey(lt => new { lt.ListingId, lt.TagId });

            modelBuilder.Entity<ListingTag>()
                .HasOne(lt => lt.GpuListing)
                .WithMany(l => l.ListingTags)
                .HasForeignKey(lt => lt.ListingId);

            modelBuilder.Entity<ListingTag>()
                .HasOne(lt => lt.Tag)
                .WithMany(t => t.ListingTags)
                .HasForeignKey(lt => lt.TagId);


            // --- ONE-TO-MANY RELATIONSHIPS ---
            modelBuilder.Entity<GpuListing>()
                .HasOne(l => l.Provider)
                .WithMany(u => u.GpuListings)
                .HasForeignKey(l => l.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ListingImage>()
                .HasOne(i => i.GpuListing)
                .WithMany(l => l.ListingImages)
                .HasForeignKey(i => i.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RentalJob>()
                .HasOne(j => j.GpuListing)
                .WithMany(l => l.RentalJobs)
                .HasForeignKey(j => j.ListingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RentalJob>()
                .HasOne(j => j.Renter)
                .WithMany(u => u.RentalJobs)
                .HasForeignKey(j => j.RenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WalletLedgerEntry>()
                .HasOne(e => e.User)
                .WithMany(u => u.WalletLedgerEntries)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- NEWLY ADDED ONE-TO-MANY RELATIONSHIPS ---

            // GpuListing -> Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.GpuListing)
                .WithMany() // A GpuListing can have many reviews, but we don't need a navigation property on GpuListing for it
                .HasForeignKey(r => r.ListingId)
                .OnDelete(DeleteBehavior.Cascade); // If listing is deleted, delete reviews

            // GpuListing -> ListingAvailability
            modelBuilder.Entity<ListingAvailability>()
                .HasOne(a => a.GpuListing)
                .WithMany() // A GpuListing can have many availability blocks
                .HasForeignKey(a => a.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Review (as Author)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete reviews if user account is deleted

            // User -> Dispute (as Raiser)
            modelBuilder.Entity<Dispute>()
                .HasOne(d => d.RaisedByUser)
                .WithMany(u => u.Disputes)
                .HasForeignKey(d => d.RaisedByUserId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete disputes if user account is deleted


            // --- ONE-TO-ONE RELATIONSHIPS ---
            modelBuilder.Entity<RentalJob>()
                .HasOne(j => j.Review)
                .WithOne(r => r.RentalJob)
                .HasForeignKey<Review>(r => r.RentalJobId);

            modelBuilder.Entity<RentalJob>()
                .HasOne(j => j.Dispute)
                .WithOne(d => d.RentalJob)
                .HasForeignKey<Dispute>(d => d.RentalJobId);

            modelBuilder.Entity<RentalJob>()
                .HasOne(j => j.Invoice)
                .WithOne(i => i.RentalJob)
                .HasForeignKey<Invoice>(i => i.RentalJobId);
        }
    }
}
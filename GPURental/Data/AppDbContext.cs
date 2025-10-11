using GPURental.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace GPURental.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<GpuListing> GpuListings { get; set; }
        public DbSet<RentalJob> RentalJobs { get; set; }
        public DbSet<WalletLedgerEntry> WalletLedgerEntries { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Dispute> Disputes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- ONE-TO-MANY RELATIONSHIPS ---
            modelBuilder.Entity<GpuListing>()
                .HasOne(l => l.Provider)
                .WithMany(u => u.GpuListings)
                .HasForeignKey(l => l.ProviderId)
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

            // GpuListing -> Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.GpuListing)
                .WithMany()
                .HasForeignKey(r => r.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Review (as Author)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Dispute (as Raiser)
            modelBuilder.Entity<Dispute>()
                .HasOne(d => d.RaisedByUser)
                .WithMany(u => u.Disputes)
                .HasForeignKey(d => d.RaisedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            // --- ONE-TO-ONE RELATIONSHIPS ---
            modelBuilder.Entity<RentalJob>()
                .HasOne(j => j.Review)
                .WithOne(r => r.RentalJob)
                .HasForeignKey<Review>(r => r.RentalJobId);

            modelBuilder.Entity<RentalJob>()
                .HasOne(j => j.Dispute)
                .WithOne(d => d.RentalJob)
                .HasForeignKey<Dispute>(d => d.RentalJobId);

            modelBuilder.Entity<User>()
        .Property(u => u.BalanceInINR)
        .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<GpuListing>()
                .Property(l => l.PricePerHourInINR)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RentalJob>()
                .Property(j => j.FinalChargeInINR)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<WalletLedgerEntry>()
                .Property(e => e.AmountInINR)
                .HasColumnType("decimal(18, 2)");

            // THE ROLES
            const string ADMIN_ROLE_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e575";
            const string PROVIDER_ROLE_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e576";
            const string RENTER_ROLE_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e577";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = ADMIN_ROLE_ID,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = PROVIDER_ROLE_ID,
                    Name = "Provider",
                    NormalizedName = "PROVIDER"
                },
                new IdentityRole
                {
                    Id = RENTER_ROLE_ID,
                    Name = "Renter",
                    NormalizedName = "RENTER"
                }
            );
        }
    }
}
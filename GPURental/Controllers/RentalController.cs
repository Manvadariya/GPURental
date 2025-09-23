using GPURental.Data;
using GPURental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    [Authorize] // Only logged-in users can rent
    public class RentalController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public RentalController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Add this method INSIDE the RentalController class

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string listingId)
        {
            if (string.IsNullOrEmpty(listingId))
            {
                return BadRequest();
            }

            var listing = await _context.GpuListings.FirstOrDefaultAsync(l => l.ListingId == listingId && l.Status == GpuStatus.Published);

            if (listing == null)
            {
                TempData["ErrorMessage"] = "This GPU is not available for rent.";
                return RedirectToAction("Index", "Marketplace");
            }

            var renter = await _userManager.GetUserAsync(User); // Get the full user object to check balance

            // Prevent users from renting their own GPUs
            if (listing.ProviderId == renter.Id)
            {
                TempData["ErrorMessage"] = "You cannot rent your own GPU.";
                return RedirectToAction("Details", "Marketplace", new { id = listingId });
            }

            // --- NEW: WALLET INTEGRATION CHECK ---
            // Check if the user has enough balance for at least one hour of rental
            if (renter.BalanceInCents < listing.PricePerHourInCents)
            {
                TempData["ErrorMessage"] = "Insufficient funds. You need at least $"
                    + (listing.PricePerHourInCents / 100.0m).ToString("F2")
                    + " to start this rental. Please add funds to your wallet.";
                return RedirectToAction("Index", "Wallet"); // Redirect to wallet page
            }
            // ------------------------------------

            listing.Status = GpuStatus.InUse;

            var rentalJob = new RentalJob
            {
                RentalJobId = Guid.NewGuid().ToString(),
                ListingId = listingId,
                RenterId = renter.Id,
                ActualStartAt = DateTime.UtcNow,
                Status = JobStatus.Running,
                FinalChargeInCents = null
            };

            _context.RentalJobs.Add(rentalJob);
            await _context.SaveChangesAsync();

            return RedirectToAction("Status", "Rental", new { id = rentalJob.RentalJobId });
        }

        // REPLACE the placeholder Status method with this one
        public async Task<IActionResult> Status(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var renterId = _userManager.GetUserId(User);

            // Find the job, but also include the related GpuListing data
            var rentalJob = await _context.RentalJobs
                .Include(j => j.GpuListing)
                .FirstOrDefaultAsync(j => j.RentalJobId == id && j.RenterId == renterId);

            if (rentalJob == null)
            {
                // This prevents users from seeing other people's jobs
                return NotFound();
            }

            // We can pass the full RentalJob object to the view
            return View(rentalJob);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Stop(string rentalJobId)
        {
            if (string.IsNullOrEmpty(rentalJobId))
            {
                return BadRequest();
            }

            var renterId = _userManager.GetUserId(User);

            // Find the job, its listing, AND the provider user object all in one query
            var rentalJob = await _context.RentalJobs
                .Include(j => j.GpuListing)
                    .ThenInclude(l => l.Provider) // Important: This loads the User object of the provider
                .FirstOrDefaultAsync(j => j.RentalJobId == rentalJobId && j.RenterId == renterId);

            if (rentalJob == null || rentalJob.Status != JobStatus.Running)
            {
                TempData["ErrorMessage"] = "Unable to stop this job.";
                return RedirectToAction("Index", "Home");
            }

            // --- All checks passed, proceed with stopping the job and processing payment ---

            // 1. Mark job as completed and calculate final cost
            rentalJob.ActualEndAt = DateTime.UtcNow;
            rentalJob.Status = JobStatus.Completed;

            var duration = rentalJob.ActualEndAt.Value - rentalJob.ActualStartAt.Value;
            double minutesRented = Math.Max(1, duration.TotalMinutes); // Ensure a minimum charge
            int finalCharge = (int)((minutesRented / 60.0) * rentalJob.GpuListing.PricePerHourInCents);
            rentalJob.FinalChargeInCents = finalCharge;

            // 2. Get the renter user object
            var renter = await _userManager.FindByIdAsync(renterId);

            // --- THIS IS THE NEW LOGIC ---

            // 3. Get the provider user object (which we already loaded via .ThenInclude())
            var provider = rentalJob.GpuListing.Provider;

            // Check to ensure we found both users
            if (renter == null || provider == null)
            {
                TempData["ErrorMessage"] = "A user account error occurred. Please contact support.";
                // We don't save changes here, effectively rolling back the transaction
                return RedirectToAction("Index", "Home");
            }

            // 4. Deduct funds from the renter's balance
            renter.BalanceInCents -= finalCharge;

            // Create a "Charge" ledger entry for the renter
            var chargeLedgerEntry = new WalletLedgerEntry
            {
                LedgerId = Guid.NewGuid().ToString(),
                UserId = renter.Id,
                RentalJobId = rentalJob.RentalJobId,
                Type = LedgerEntryType.Charge,
                AmountInCents = finalCharge,
                Status = LedgerEntryStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };
            _context.WalletLedgerEntries.Add(chargeLedgerEntry);

            // 5. Add funds to the provider's balance
            provider.BalanceInCents += finalCharge;

            // Create a "Payout" ledger entry to represent the provider's earnings
            var providerEarningsEntry = new WalletLedgerEntry
            {
                LedgerId = Guid.NewGuid().ToString(),
                UserId = provider.Id,
                RentalJobId = rentalJob.RentalJobId,
                Type = LedgerEntryType.Payout, // <-- CHANGE THIS from TopUp to Payout
                AmountInCents = finalCharge,
                Status = LedgerEntryStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };
            _context.WalletLedgerEntries.Add(providerEarningsEntry);

            // ------------------------------------

            // 6. Set the GpuListing status back to Published
            rentalJob.GpuListing.Status = GpuStatus.Published;

            // 7. Save all changes to the database
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rental completed successfully! You were charged $" + (finalCharge / 100.0m).ToString("F2");
            return RedirectToAction("Index", "Marketplace");
        }
    }
}
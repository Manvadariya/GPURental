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
    [Authorize(Roles = "Renter")]
    public class RentalController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public RentalController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

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

            var renter = await _userManager.GetUserAsync(User);
            if (listing.ProviderId == renter.Id)
            {
                TempData["ErrorMessage"] = "You cannot rent your own GPU.";
                return RedirectToAction("Details", "Marketplace", new { id = listingId });
            }
            if (renter.BalanceInINR < listing.PricePerHourInINR)
            {
                TempData["ErrorMessage"] = "Insufficient funds. You need at least $"
                    + (listing.PricePerHourInINR).ToString("F2")
                    + " to start this rental. Please add funds to your wallet.";
                return RedirectToAction("Index", "Wallet");
            }
            listing.Status = GpuStatus.InUse;

            var rentalJob = new RentalJob
            {
                RentalJobId = Guid.NewGuid().ToString(),
                ListingId = listingId,
                RenterId = renter.Id,
                ActualStartAt = DateTime.UtcNow,
                Status = JobStatus.Running,
                FinalChargeInINR = null
            };

            _context.RentalJobs.Add(rentalJob);
            await _context.SaveChangesAsync();

            return RedirectToAction("Status", "Rental", new { id = rentalJob.RentalJobId });
        }

        public async Task<IActionResult> Status(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var renterId = _userManager.GetUserId(User);

            var rentalJob = await _context.RentalJobs
                .Include(j => j.GpuListing)
                .FirstOrDefaultAsync(j => j.RentalJobId == id && j.RenterId == renterId);

            if (rentalJob == null)
            {
                return NotFound();
            }

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
                    .ThenInclude(l => l.Provider)
                .FirstOrDefaultAsync(j => j.RentalJobId == rentalJobId && j.RenterId == renterId);

            if (rentalJob == null || rentalJob.Status != JobStatus.Running)
            {
                TempData["ErrorMessage"] = "This job is not available to be stopped.";
                return RedirectToAction("Index", "Dashboard");
            }

            // --- ACCURATE FINANCIAL CALCULATION IN SECONDS AND INR ---
            rentalJob.ActualEndAt = DateTime.UtcNow;
            rentalJob.Status = JobStatus.Completed;

            var duration = rentalJob.ActualEndAt.Value - rentalJob.ActualStartAt.Value;
            // 1. Calculate the total duration in seconds. Ensure a minimum of 1 second.
            decimal totalSeconds = (decimal)Math.Max(1, duration.TotalSeconds);

            // 2. Calculate price per second using high-precision decimal math
            decimal pricePerSecondInINR = rentalJob.GpuListing.PricePerHourInINR / 3600.0m;

            // 3. Calculate the exact final charge and store it.
            // We don't round here to store the most precise value possible.
            rentalJob.FinalChargeInINR = totalSeconds * pricePerSecondInINR;
            // ---------------------------------------------------------------

            var renter = await _userManager.FindByIdAsync(renterId);
            var provider = rentalJob.GpuListing.Provider;

            if (renter == null || provider == null)
            {
                TempData["ErrorMessage"] = "A user account error occurred while processing payment. Please contact support.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Update Balances using the new decimal properties
            renter.BalanceInINR -= rentalJob.FinalChargeInINR.Value;
            provider.BalanceInINR += rentalJob.FinalChargeInINR.Value;

            // Create Ledger Entry for the Renter's Charge
            _context.WalletLedgerEntries.Add(new WalletLedgerEntry
            {
                LedgerId = Guid.NewGuid().ToString(),
                UserId = renter.Id,
                RentalJobId = rentalJob.RentalJobId,
                Type = LedgerEntryType.Charge,
                AmountInINR = rentalJob.FinalChargeInINR.Value,
                Status = LedgerEntryStatus.Completed,
                CreatedAt = DateTime.UtcNow
            });

            // Create Ledger Entry for the Provider's Payout
            _context.WalletLedgerEntries.Add(new WalletLedgerEntry
            {
                LedgerId = Guid.NewGuid().ToString(),
                UserId = provider.Id,
                RentalJobId = rentalJob.RentalJobId,
                Type = LedgerEntryType.Payout,
                AmountInINR = rentalJob.FinalChargeInINR.Value,
                Status = LedgerEntryStatus.Completed,
                CreatedAt = DateTime.UtcNow
            });

            rentalJob.GpuListing.Status = GpuStatus.Published;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Marketplace");
        }
    }
}
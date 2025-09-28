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
            if (renter.BalanceInCents < listing.PricePerHourInCents)
            {
                TempData["ErrorMessage"] = "Insufficient funds. You need at least $"
                    + (listing.PricePerHourInCents / 100.0m).ToString("F2")
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
                FinalChargeInCents = null
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

            var rentalJob = await _context.RentalJobs
                .Include(j => j.GpuListing)
                    .ThenInclude(l => l.Provider)
                .FirstOrDefaultAsync(j => j.RentalJobId == rentalJobId && j.RenterId == renterId);

            if (rentalJob == null || rentalJob.Status != JobStatus.Running)
            {
                TempData["ErrorMessage"] = "Unable to stop this job.";
                return RedirectToAction("Index", "Home");
            }


            // Mark job as completed and calculate final cost
            rentalJob.ActualEndAt = DateTime.UtcNow;
            rentalJob.Status = JobStatus.Completed;

            var duration = rentalJob.ActualEndAt.Value - rentalJob.ActualStartAt.Value;
            double minutesRented = Math.Max(1, duration.TotalMinutes); // Ensure a minimum charge
            int finalCharge = (int)((minutesRented / 60.0) * rentalJob.GpuListing.PricePerHourInCents);
            rentalJob.FinalChargeInCents = finalCharge;

            var renter = await _userManager.FindByIdAsync(renterId);


            var provider = rentalJob.GpuListing.Provider;

            if (renter == null || provider == null)
            {
                TempData["ErrorMessage"] = "A user account error occurred. Please contact support.";
                return RedirectToAction("Index", "Home");
            }

            renter.BalanceInCents -= finalCharge;

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

            provider.BalanceInCents += finalCharge;

            var providerEarningsEntry = new WalletLedgerEntry
            {
                LedgerId = Guid.NewGuid().ToString(),
                UserId = provider.Id,
                RentalJobId = rentalJob.RentalJobId,
                Type = LedgerEntryType.Payout,
                AmountInCents = finalCharge,
                Status = LedgerEntryStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };
            _context.WalletLedgerEntries.Add(providerEarningsEntry);

            rentalJob.GpuListing.Status = GpuStatus.Published;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rental completed successfully! You were charged $" + (finalCharge / 100.0m).ToString("F2");
            return RedirectToAction("Index", "Marketplace");
        }
    }
}
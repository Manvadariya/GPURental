using GPURental.Data;
using GPURental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Action for the main Admin Dashboard (can be built out later)
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard"); // Redirect to the disputes page for now
        }

        // GET: /Admin/Disputes
        // Displays a list of all pending disputes
        public async Task<IActionResult> Disputes()
        {
            var submittedDisputes = await _context.Disputes
                .Where(d => d.Status == Models.DisputeStatus.Submitted)
                .Include(d => d.RentalJob)
                .Include(d => d.RaisedByUser) // <-- THIS LINE WAS MISSING
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();

            return View(submittedDisputes);
        }

        // GET: /Admin/ResolveDispute/some-id
        // Displays the details of a single dispute
        [HttpGet]
        public async Task<IActionResult> ResolveDispute(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var dispute = await _context.Disputes
                .Include(d => d.RaisedByUser)
                .Include(d => d.RentalJob)
                    .ThenInclude(j => j.GpuListing)
                        .ThenInclude(l => l.Provider)
                .FirstOrDefaultAsync(d => d.DisputeId == id);

            if (dispute == null)
            {
                return NotFound();
            }

            return View(dispute);
        }

        // POST: /Admin/ResolveDispute
        // Processes the admin's resolution decision
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveDispute(string disputeId, string resolution, decimal refundAmount)
        {
            if (string.IsNullOrEmpty(disputeId))
            {
                return BadRequest();
            }

            // --- 1. Fetch all necessary data in one query ---
            var dispute = await _context.Disputes
                .Include(d => d.RaisedByUser) // Renter
                .Include(d => d.RentalJob)
                    .ThenInclude(j => j.GpuListing)
                        .ThenInclude(l => l.Provider) // Provider
                .FirstOrDefaultAsync(d => d.DisputeId == disputeId);

            if (dispute == null || dispute.Status != DisputeStatus.Submitted)
            {
                TempData["ErrorMessage"] = "This dispute could not be found or has already been resolved.";
                return RedirectToAction("Index", "Dashboard");
            }

            // --- 2. Handle Rejection ---
            if (resolution == "reject")
            {
                dispute.Status = DisputeStatus.Rejected;
                _context.Disputes.Update(dispute);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The dispute has been rejected.";
                return RedirectToAction("Index", "Dashboard");
            }

            // --- 3. Handle Approval and Refund ---
            if (resolution == "approve")
            {
                // Convert the incoming dollar amount from the form to cents for database operations
                int refundAmountCents = (int)(refundAmount * 100);
                int originalCharge = dispute.RentalJob.FinalChargeInCents ?? 0;

                // Validation checks
                if (refundAmountCents < 0)
                {
                    TempData["ErrorMessage"] = "Refund amount cannot be negative.";
                    return RedirectToAction("ResolveDispute", new { id = disputeId });
                }
                if (refundAmountCents > originalCharge)
                {
                    TempData["ErrorMessage"] = $"Refund amount cannot exceed the original charge of ${(originalCharge / 100.0m):F2}.";
                    return RedirectToAction("ResolveDispute", new { id = disputeId });
                }

                // If a refund is being processed, handle the financial transaction
                if (refundAmountCents > 0)
                {
                    var renter = dispute.RaisedByUser;
                    var provider = dispute.RentalJob.GpuListing.Provider;

                    if (provider.BalanceInCents < refundAmountCents)
                    {
                        TempData["WarningMessage"] = "Warning: Provider's balance is now negative.";
                    }

                    // Adjust balances
                    renter.BalanceInCents += refundAmountCents;
                    provider.BalanceInCents -= refundAmountCents;

                    // Create ledger entry for Renter's refund
                    _context.WalletLedgerEntries.Add(new WalletLedgerEntry
                    {
                        LedgerId = Guid.NewGuid().ToString(),
                        UserId = renter.Id,
                        Type = LedgerEntryType.Refund,
                        AmountInCents = refundAmountCents,
                        Status = LedgerEntryStatus.Completed,
                        CreatedAt = DateTime.UtcNow
                    });

                    // Create ledger entry for Provider's debit
                    _context.WalletLedgerEntries.Add(new WalletLedgerEntry
                    {
                        LedgerId = Guid.NewGuid().ToString(),
                        UserId = provider.Id,
                        Type = LedgerEntryType.Charge,
                        AmountInCents = refundAmountCents,
                        Status = LedgerEntryStatus.Completed,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // Finally, update the dispute status
                dispute.Status = DisputeStatus.Resolved;
                _context.Disputes.Update(dispute);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The dispute has been successfully resolved.";
                return RedirectToAction("Index", "Dashboard");
            }

            // If resolution type is unknown, return to the disputes list
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
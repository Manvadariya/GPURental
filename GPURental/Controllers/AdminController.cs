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

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Admin/Disputes => Displays a list of all pending disputes
        public async Task<IActionResult> Disputes()
        {
            var submittedDisputes = await _context.Disputes
                .Where(d => d.Status == Models.DisputeStatus.Submitted)
                .Include(d => d.RentalJob)
                .Include(d => d.RaisedByUser)
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();

            return View(submittedDisputes);
        }

        // GET: /Admin/ResolveDispute/some-id => Displays the details of a single dispute
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

        // POST: /Admin/ResolveDispute => Processes the admin's resolution decision
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveDispute(string disputeId, string resolution, decimal refundAmount)
        {
            if (string.IsNullOrEmpty(disputeId))
            {
                return BadRequest();
            }

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

            if (resolution == "reject")
            {
                dispute.Status = DisputeStatus.Rejected;
                _context.Disputes.Update(dispute);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The dispute has been rejected.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (resolution == "approve")
            {
                int refundAmountCents = (int)(refundAmount * 100);
                int originalCharge = dispute.RentalJob.FinalChargeInCents ?? 0;

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

                if (refundAmountCents > 0)
                {
                    var renter = dispute.RaisedByUser;
                    var provider = dispute.RentalJob.GpuListing.Provider;

                    if (provider.BalanceInCents < refundAmountCents)
                    {
                        TempData["WarningMessage"] = "Warning: Provider's balance is now negative.";
                    }

                    renter.BalanceInCents += refundAmountCents;
                    provider.BalanceInCents -= refundAmountCents;

                    _context.WalletLedgerEntries.Add(new WalletLedgerEntry
                    {
                        LedgerId = Guid.NewGuid().ToString(),
                        UserId = renter.Id,
                        Type = LedgerEntryType.Refund,
                        AmountInCents = refundAmountCents,
                        Status = LedgerEntryStatus.Completed,
                        CreatedAt = DateTime.UtcNow
                    });

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
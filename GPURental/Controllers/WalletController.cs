using GPURental.Data;
using GPURental.Models;
using GPURental.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    [Authorize(Roles = "Renter, Provider")]  // All actions in this controller require a user to be logged in
    public class WalletController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public WalletController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Add this method INSIDE the WalletController class
        public async Task<IActionResult> Index()
        {
            // 1. Get the current user object
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // This case should rarely happen if [Authorize] is working
                return Challenge(); // Forces a login
            }

            // 2. Get the user's transaction history from the database
            var transactionHistory = await _context.WalletLedgerEntries
                .Where(e => e.UserId == user.Id)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            // 3. Create the ViewModel and populate it
            var viewModel = new WalletDashboardViewModel
            {
                CurrentBalanceInCents = user.BalanceInCents,
                TransactionHistory = transactionHistory
            };

            // 4. Pass the ViewModel to the view
            return View(viewModel);
        }

        // Add this method INSIDE the WalletController class
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFunds(decimal amount)
        {
            if (amount <= 0)
            {
                // Add a friendly error message
                TempData["ErrorMessage"] = "Please enter a positive amount to add.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            int amountInCents = (int)(amount * 100);

            // 1. Create a ledger entry for the top-up
            var ledgerEntry = new WalletLedgerEntry
            {
                LedgerId = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Type = LedgerEntryType.TopUp,
                AmountInCents = amountInCents,
                Status = LedgerEntryStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };
            _context.WalletLedgerEntries.Add(ledgerEntry);

            // 2. Update the user's balance
            user.BalanceInCents += amountInCents;

            // 3. Save both changes to the database
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully added ${amount.ToString("F2")} to your wallet!";
            return RedirectToAction("Index");
        }
    }
}
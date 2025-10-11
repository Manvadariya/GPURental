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
    [Authorize(Roles = "Renter, Provider")]  
    public class WalletController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public WalletController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                
                return Challenge(); 
            }

            
            var transactionHistory = await _context.WalletLedgerEntries
                .Where(e => e.UserId == user.Id)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            
            var viewModel = new WalletDashboardViewModel
            {
                CurrentBalanceInINR = user.BalanceInINR,
                TransactionHistory = transactionHistory
            };

            
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFunds(decimal amount)
        {
            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Please enter a positive amount to add.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);

            var ledgerEntry = new WalletLedgerEntry
            {
                LedgerId = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Type = LedgerEntryType.TopUp,
                AmountInINR = amount, // Use the decimal amount directly
                Status = LedgerEntryStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };
            _context.WalletLedgerEntries.Add(ledgerEntry);

            // Add the decimal amount directly to the user's balance
            user.BalanceInINR += amount;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully added ₹{amount:F2} to your wallet!";
            return RedirectToAction("Index");
        }
    }
}
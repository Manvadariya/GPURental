using GPURental.Data;
using GPURental.Models;
using GPURental.ViewModels; // We will create new ViewModels
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    [Authorize] // All dashboard access requires a user to be logged in
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin"))
            {
                // --- Admin Logic ---
                var viewModel = new AdminDashboardViewModel
                {
                    TotalUsers = await _context.Users.CountAsync(),
                    TotalListings = await _context.GpuListings.CountAsync(),
                    PendingDisputes = await _context.Disputes
                        .Where(d => d.Status == DisputeStatus.Submitted)
                        .Include(d => d.RaisedByUser) // <-- THIS IS THE CRUCIAL FIX
                        .ToListAsync(),
                    ActiveJobs = await _context.RentalJobs
                        .Where(j => j.Status == JobStatus.Running)
                        .Include(j => j.Renter)
                        .Include(j => j.GpuListing)
                        .OrderBy(j => j.ActualStartAt)
                        .ToListAsync()
                };
                return View("AdminDashboard", viewModel);
            }
            else if (User.IsInRole("Provider"))
            {
                // --- Provider Logic ---
                var viewModel = new ProviderDashboardViewModel
                {
                    MyListings = await _context.GpuListings
                        .Where(l => l.ProviderId == userId)
                        .OrderByDescending(l => l.CreatedAt)
                        .ToListAsync(),
                    DisputesAgainstMe = await _context.Disputes
                        .Where(d => d.RentalJob.GpuListing.ProviderId == userId)
                        .Include(d => d.RaisedByUser)
                        .OrderByDescending(d => d.CreatedAt)
                        .ToListAsync()
                };
                return View("ProviderDashboard", viewModel);
            }
            else // Default to Renter
            {
                // --- Renter Logic ---
                var viewModel = new RenterDashboardViewModel
                {
                    ActiveJobs = await _context.RentalJobs
                        .Where(j => j.RenterId == userId && j.Status == JobStatus.Running)
                        .Include(j => j.GpuListing)
                        .ToListAsync(),
                    JobHistory = await _context.RentalJobs
                        .Where(j => j.RenterId == userId && j.Status != JobStatus.Running)
                        .Include(j => j.GpuListing)
                        .OrderByDescending(j => j.ActualEndAt)
                        .Take(5) // Show the last 5 completed/failed jobs
                        .ToListAsync(),
                    MyReviews = await _context.Reviews
                        .Where(r => r.AuthorId == userId)
                        .OrderByDescending(r => r.CreatedAt)
                        .ToListAsync(),
                    MyDisputes = await _context.Disputes
                        .Where(d => d.RaisedByUserId == userId)
                        .OrderByDescending(d => d.CreatedAt)
                        .ToListAsync(),
                    TransactionHistory = await _context.WalletLedgerEntries
                        .Where(e => e.UserId == userId)
                        .OrderByDescending(e => e.CreatedAt)
                        .Take(10) // Show the last 10 transactions
                        .ToListAsync()
                };
                return View("RenterDashboard", viewModel);
            }
        }
    }
}
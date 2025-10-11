using GPURental.Data;
using GPURental.Models;
using GPURental.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    [Authorize]
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
                var viewModel = new AdminDashboardViewModel
                {
                    TotalUsers = await _context.Users.CountAsync(),
                    TotalListings = await _context.GpuListings.CountAsync(),
                    PendingDisputes = await _context.Disputes
                        .Where(d => d.Status == DisputeStatus.Submitted)
                        .Include(d => d.RaisedByUser)
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

                // 1. Get all of the provider's listings first
                var myListings = await _context.GpuListings
                    .Where(l => l.ProviderId == userId)
                    .OrderByDescending(l => l.CreatedAt)
                    .ToListAsync();

                // 2. Get all completed jobs for this provider for calculations
                var completedJobs = await _context.RentalJobs
                    .Where(j => j.GpuListing.ProviderId == userId &&
                                j.Status == JobStatus.Completed &&
                                j.FinalChargeInINR.HasValue)
                    .Include(j => j.GpuListing)
                    .ToListAsync();

                // 3. Calculate Chart Data (Profit and Usage)
                var profitData = completedJobs
                    .GroupBy(j => j.GpuListing.Title)
                    .Select(group => new ChartDataViewModel
                    {
                        Label = group.Key,
                        Value = group.Sum(j => (decimal)j.FinalChargeInINR.Value)
                    })
                    .OrderByDescending(d => d.Value)
                    .ToList();

                var usageData = completedJobs
                    .GroupBy(j => j.GpuListing.Title)
                    .Select(group => new ChartDataViewModel
                    {
                        Label = group.Key,
                        Value = (decimal)group.Sum(j => (j.ActualEndAt.Value - j.ActualStartAt.Value).TotalHours)
                    })
                    .OrderByDescending(d => d.Value)
                    .ToList();

                // 4. Populate the final ViewModel
                var viewModel = new ProviderDashboardViewModel
                {
                    MyListings = myListings,
                    DisputesAgainstMe = await _context.Disputes
                        .Where(d => d.RentalJob.GpuListing.ProviderId == userId)
                        .Include(d => d.RaisedByUser)
                        .OrderByDescending(d => d.CreatedAt)
                        .ToListAsync(),

                    ProfitByListingData = profitData,
                    UsageByListingData = usageData,

                    // Calculate stats for the top cards
                    ActiveListingsCount = myListings.Count(l => l.Status == GpuStatus.Published || l.Status == GpuStatus.InUse),
                    TotalEarnings = await _context.WalletLedgerEntries.Where(e => e.UserId == userId && e.Type == LedgerEntryType.Payout).SumAsync(e => e.AmountInINR),
                    RunningJobsCount = myListings.Count(l => l.Status == GpuStatus.InUse),
                    PendingDisputesCount = await _context.Disputes.CountAsync(d => d.RentalJob.GpuListing.ProviderId == userId && d.Status == DisputeStatus.Submitted)
                };

                return View("ProviderDashboard", viewModel);
            }
            else // Default to Renter
            {
                // --- Renter Logic ---

                // 1. Get the current user's ID

                // 2. Get ALL of the user's jobs (active and historical) in one efficient query
                var allMyJobs = await _context.RentalJobs
                    .Where(j => j.RenterId == userId)
                    .Include(j => j.GpuListing) // Include listing data for titles, models, etc.
                    .OrderByDescending(j => j.ActualEndAt ?? j.ActualStartAt) // Order all jobs by their most recent date
                    .ToListAsync();

                // 3. Filter for completed jobs to be used in calculations
                var completedJobs = allMyJobs
                    .Where(j => j.Status == JobStatus.Completed && j.FinalChargeInINR.HasValue)
                    .ToList();

                // 4. Process data for the "Spending by GPU" chart, grouped by GPU MODEL
                var spendingData = completedJobs
                    .GroupBy(j => j.GpuListing.GpuModel)
                    .Select(group => new ChartDataViewModel
                    {
                        Label = group.Key,
                        Value = group.Sum(j => j.FinalChargeInINR.Value)
                    })
                    .OrderByDescending(d => d.Value)
                    .ToList();

                // 5. Process data for the "Job Count by GPU" chart, grouped by GPU MODEL
                var jobCountData = completedJobs
                    .GroupBy(j => j.GpuListing.GpuModel)
                    .Select(group => new ChartDataViewModel
                    {
                        Label = group.Key,
                        Value = group.Count()
                    })
                    .OrderByDescending(d => d.Value)
                    .ToList();

                // 6. Populate the final ViewModel
                var viewModel = new RenterDashboardViewModel
                {
                    // Stats for Cards are calculated from the complete lists
                    TotalSpent = completedJobs.Sum(j => j.FinalChargeInINR.Value),
                    JobsCompletedCount = completedJobs.Count(),
                    ActiveJobsCount = allMyJobs.Count(j => j.Status == JobStatus.Running),
                    OpenDisputesCount = await _context.Disputes.CountAsync(d => d.RaisedByUserId == userId && d.Status == DisputeStatus.Submitted),

                    // Data for Tables
                    ActiveJobs = allMyJobs.Where(j => j.Status == JobStatus.Running).ToList(),
                    JobHistory = allMyJobs.Where(j => j.Status != JobStatus.Running).ToList(), // Show ALL history
                    MyReviews = await _context.Reviews.Where(r => r.AuthorId == userId).Include(r => r.GpuListing).ToListAsync(),
                    MyDisputes = await _context.Disputes.Where(d => d.RaisedByUserId == userId).Include(d => d.RentalJob.GpuListing).ToListAsync(),

                    // Data for Charts
                    SpendingByGpuData = spendingData,
                    JobCountByGpuData = jobCountData
                };

                return View("RenterDashboard", viewModel);
            }
        }
    }
}
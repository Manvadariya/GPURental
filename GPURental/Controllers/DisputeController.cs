using GPURental.Data;
using GPURental.Models;
using GPURental.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    [Authorize(Roles = "Renter")]
    public class DisputeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public DisputeController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string rentalJobId)
        {
            if (string.IsNullOrEmpty(rentalJobId))
            {
                return BadRequest();
            }

            var userId = _userManager.GetUserId(User);

            var job = await _context.RentalJobs
                .Include(j => j.GpuListing)
                .FirstOrDefaultAsync(j =>
                    j.RentalJobId == rentalJobId &&
                    j.RenterId == userId &&
                    j.Status != JobStatus.Running);

            if (job == null)
            {
                TempData["ErrorMessage"] = "This job is not eligible for a dispute.";
                return RedirectToAction("Index", "Wallet");
            }

            bool alreadyDisputed = await _context.Disputes.AnyAsync(d => d.RentalJobId == rentalJobId);
            if (alreadyDisputed)
            {
                TempData["ErrorMessage"] = "A dispute has already been filed for this job.";
                return RedirectToAction("Index", "Wallet");
            }

            var viewModel = new SubmitDisputeViewModel
            {
                RentalJobId = job.RentalJobId
            };

            ViewData["ListingTitle"] = job.GpuListing.Title;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubmitDisputeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                var job = await _context.RentalJobs.FirstOrDefaultAsync(j =>
                    j.RentalJobId == model.RentalJobId &&
                    j.RenterId == userId);

                if (job == null)
                {
                    return Forbid();
                }

                var dispute = new Dispute
                {
                    DisputeId = Guid.NewGuid().ToString(),
                    RentalJobId = model.RentalJobId,
                    RaisedByUserId = userId,
                    Reason = model.Reason,
                    Description = model.Description,
                    Status = DisputeStatus.Submitted,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Disputes.Add(dispute);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your dispute has been successfully submitted. Our team will review it shortly.";
                return RedirectToAction("Index", "Wallet");
            }

            // If model state is invalid, re-display the form, reload the ListingTitle for the view
            var jobForTitle = await _context.RentalJobs
                                    .Include(j => j.GpuListing)
                                    .FirstOrDefaultAsync(j => j.RentalJobId == model.RentalJobId);
            ViewData["ListingTitle"] = jobForTitle?.GpuListing.Title ?? "Unknown Listing";
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(string id)
        {
            var userId = _userManager.GetUserId(User);
            var dispute = await _context.Disputes.FindAsync(id);

            if (dispute == null || dispute.RaisedByUserId != userId || dispute.Status != DisputeStatus.Submitted)
            {
                TempData["ErrorMessage"] = "This dispute cannot be withdrawn.";
                return RedirectToAction("Index", "Dashboard");
            }

            _context.Disputes.Remove(dispute);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Your dispute has been withdrawn.";

            return RedirectToAction("Index", "Dashboard");
        }
    }
}
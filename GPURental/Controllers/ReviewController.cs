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
    [Authorize(Roles = "Renter")] 
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReviewController(AppDbContext context, UserManager<User> userManager)
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
                    j.Status == JobStatus.Completed);    

            if (job == null)
            {
                TempData["ErrorMessage"] = "This job is not eligible for a review.";
                return RedirectToAction("Index", "Home");
            }

            
            bool alreadyReviewed = await _context.Reviews.AnyAsync(r => r.RentalJobId == rentalJobId);
            if (alreadyReviewed)
            {
                TempData["ErrorMessage"] = "You have already submitted a review for this job.";
                return RedirectToAction("Details", "Marketplace", new { id = job.ListingId });
            }

            
            var viewModel = new SubmitReviewViewModel
            {
                RentalJobId = job.RentalJobId,
                ListingId = job.ListingId
            };

            ViewData["ListingTitle"] = job.GpuListing.Title; 

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubmitReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                
                var job = await _context.RentalJobs.FirstOrDefaultAsync(j =>
                    j.RentalJobId == model.RentalJobId &&
                    j.RenterId == userId &&
                    j.Status == JobStatus.Completed);

                if (job == null)
                {
                    return Forbid(); 
                }

                
                var review = new Review
                {
                    ReviewId = Guid.NewGuid().ToString(),
                    RentalJobId = model.RentalJobId,
                    ListingId = model.ListingId,
                    AuthorId = userId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you! Your review has been submitted.";
                return RedirectToAction("Details", "Marketplace", new { id = model.ListingId });
            }

            
            return View(model);
        }
    }
}
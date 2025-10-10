using GPURental.Data;
using GPURental.Models;
using GPURental.Services;
using GPURental.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    [Authorize(Roles = "Provider")]
    public class ListingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IAiService _aiService;

        public ListingsController(AppDbContext context,
                                  UserManager<User> userManager,
                                  IWebHostEnvironment hostingEnvironment,
                                  IAiService aiService)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _aiService = aiService;
        }

        // Provider Dashboard
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            var userListings = await _context.GpuListings
                                             .Where(l => l.ProviderId == currentUserId)
                                             .OrderByDescending(l => l.CreatedAt)
                                             .ToListAsync();
            return View(userListings);
        }

        // GET: /Listings/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Listings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = await ProcessUploadedFile(model.Image);
                var providerId = _userManager.GetUserId(User);

                var newListing = new GpuListing
                {
                    ListingId = Guid.NewGuid().ToString(),
                    ProviderId = providerId,
                    Title = model.Title,
                    GpuModel = model.GpuModel,
                    VramInGB = model.VramInGB,
                    RamInGB = model.RamInGB,
                    DiskInGB = model.DiskInGB,
                    CpuModel = model.CpuModel,
                    OperatingSystem = model.OperatingSystem,
                    Location = model.Location,
                    PricePerHourInCents = model.PricePerHourInCents,
                    Status = GpuStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    ImagePath = uniqueFileName
                };

                _context.GpuListings.Add(newListing);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var listing = await _context.GpuListings.FirstOrDefaultAsync(l => l.ListingId == id);

            var currentUserId = _userManager.GetUserId(User);
            if (listing == null || listing.ProviderId != currentUserId)
            {
                return NotFound();
            }

            var viewModel = new ListingEditViewModel
            {
                ListingId = listing.ListingId,
                Title = listing.Title,
                GpuModel = listing.GpuModel,
                CpuModel = listing.CpuModel,
                VramInGB = listing.VramInGB,
                RamInGB = listing.RamInGB,
                DiskInGB = listing.DiskInGB,
                OperatingSystem = listing.OperatingSystem,
                Location = listing.Location,
                PricePerHourInCents = listing.PricePerHourInCents,
                ExistingImagePath = listing.ImagePath
            };

            return View(viewModel);
        }

        // POST: /Listings/Edit/some-id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ListingEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var listing = await _context.GpuListings.FindAsync(model.ListingId);
            var currentUserId = _userManager.GetUserId(User);

            if (listing == null || listing.ProviderId != currentUserId)
            {
                return NotFound();
            }

            listing.Title = model.Title;
            listing.GpuModel = model.GpuModel;
            listing.CpuModel = model.CpuModel;
            listing.VramInGB = model.VramInGB;
            listing.RamInGB = model.RamInGB;
            listing.DiskInGB = model.DiskInGB;
            listing.OperatingSystem = model.OperatingSystem;
            listing.Location = model.Location;
            listing.PricePerHourInCents = model.PricePerHourInCents;

            if (model.Image != null)
            {
                if (!string.IsNullOrEmpty(model.ExistingImagePath))
                {
                    string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", "listings", model.ExistingImagePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                listing.ImagePath = await ProcessUploadedFile(model.Image);
            }

            _context.GpuListings.Update(listing);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Listing updated successfully!";
            return RedirectToAction("Index", "Dashboard");
        }

        // POST: /Listings/Publish/some-id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(string id)
        {
            return await UpdateListingStatus(id, GpuStatus.Published);
        }

        // POST: /Listings/Pause/some-id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pause(string id)
        {
            return await UpdateListingStatus(id, GpuStatus.Paused);
        }

        // GET: /Listings/Delete/some-id
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var listing = await _context.GpuListings.FindAsync(id);
            var currentUserId = _userManager.GetUserId(User);

            if (listing == null || listing.ProviderId != currentUserId)
            {
                return NotFound();
            }
            return View(listing);
        }

        // POST: /Listings/Delete/some-id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var listing = await _context.GpuListings.FindAsync(id);
            var currentUserId = _userManager.GetUserId(User);

            if (listing == null || listing.ProviderId != currentUserId)
            {
                return NotFound();
            }

            _context.GpuListings.Remove(listing);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Listing deleted successfully.";
            return RedirectToAction("Index", "Dashboard");
        }

        private async Task<string> ProcessUploadedFile(IFormFile file)
        {
            string uniqueFileName = null;
            if (file != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "listings");
                Directory.CreateDirectory(uploadsFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            return uniqueFileName;
        }
        private async Task<IActionResult> UpdateListingStatus(string id, GpuStatus newStatus)
        {
            var listing = await _context.GpuListings.FindAsync(id);
            var currentUserId = _userManager.GetUserId(User);

            if (listing == null || listing.ProviderId != currentUserId)
            {
                return NotFound();
            }

            listing.Status = newStatus;
            _context.GpuListings.Update(listing);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> SuggestPrice(string gpuModel, int vram)
        {
            if (string.IsNullOrEmpty(gpuModel) || vram <= 0)
            {
                return Json(new { success = false });
            }

            var existingListings = await _context.GpuListings
                .Where(l => l.Status == GpuStatus.Published)
                .Take(10) // Get a sample of market prices
                .ToListAsync();

            var suggestedPrice = await _aiService.SuggestPriceAsync(gpuModel, vram, existingListings);

            return Json(new { success = true, price = suggestedPrice });
        }
    }
}
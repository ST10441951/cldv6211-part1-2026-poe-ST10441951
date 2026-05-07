/* EventEase Elite - Venues Controller
   Author: Joshua Marc Lourens
   Description: Manages the lifecycle of venue records, including cloud-hosted 
   imagery via Azure Blob Storage and administrative search logic.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly EventEaseContext _context;
        private readonly IConfiguration _configuration;

        public VenuesController(EventEaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Venues
        // Logic: Accept a search string to filter the directory by Name or Location.
        public async Task<IActionResult> Index(string searchString)
        {
            // Maintaining the filter state in ViewData for the UI search bar
            ViewData["CurrentFilter"] = searchString;

            var venues = _context.Venue.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                // Querying the database for partial matches in Identity or Geography
                venues = venues.Where(v => v.VenueName!.Contains(searchString)
                                        || v.VenueLocation!.Contains(searchString));
            }

            return View(await venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueID == id);

            if (venue == null) return NotFound();

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueID,VenueName,VenueLocation,VenueCapacity,ImageUrl,ImageFile")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                // Cloud Media Logic: Check for a file upload and push to Azure Blob Storage
                if (venue.ImageFile != null && venue.ImageFile.Length > 0)
                {
                    string connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("venue-images");

                    // Ensuring the container exists before attempting upload
                    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                    // Generate a Unique ID for the filename to prevent overwriting existing assets
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(venue.ImageFile.FileName);
                    BlobClient blobClient = containerClient.GetBlobClient(fileName);

                    using (var stream = venue.ImageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = venue.ImageFile.ContentType });
                    }
                    // Mapping the resulting URI to our model for DB storage
                    venue.ImageUrl = blobClient.Uri.ToString();
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue record successfully registered in the directory.";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueID,VenueName,VenueLocation,VenueCapacity,ImageUrl,ImageFile")] Venue venue)
        {
            if (id != venue.VenueID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Update Cloud Media: If a new file is provided, replace the ImageUrl
                    if (venue.ImageFile != null && venue.ImageFile.Length > 0)
                    {
                        string connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("venue-images");

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(venue.ImageFile.FileName);
                        BlobClient blobClient = containerClient.GetBlobClient(fileName);

                        using (var stream = venue.ImageFile.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = venue.ImageFile.ContentType });
                        }
                        venue.ImageUrl = blobClient.Uri.ToString();
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue record successfully updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueID == id);

            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // REFERENTIAL INTEGRITY CHECK: Prevent deletion if bookings are linked
            bool hasActiveBookings = await _context.Booking.AnyAsync(b => b.VenueID == id);

            if (hasActiveBookings)
            {
                // UI Feedback for Action Blocked
                TempData["ErrorMessage"] = "CANNOT DELETE: This venue is linked to active bookings. Purge associated records first.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue != null)
            {
                _context.Venue.Remove(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue record successfully decommissioned.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueID == id);
        }
    }
}

/* TECHNICAL REFERENCES
   ---------------------------------------------------
   1. Azure Blob Storage SDK: Implementing BlobServiceClient for cloud media management.
   2. IConfiguration Pattern: Dependency Injection for secure connection string retrieval.
   3. Referential Integrity Logic: Using .AnyAsync() to validate foreign key constraints before deletion.
   4. Search Implementation: Use of IQueryable and partial string matching for administrative filtering.
   5. TempData Messaging: Providing tactical specialist feedback for global state changes.
*/
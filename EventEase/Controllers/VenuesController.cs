using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            return View(await _context.Venue.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueID,VenueName,VenueLocation,VenueCapacity,ImageUrl,ImageFile")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null && venue.ImageFile.Length > 0)
                {
                    string connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("venue-images");
                    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(venue.ImageFile.FileName);
                    BlobClient blobClient = containerClient.GetBlobClient(fileName);

                    using (var stream = venue.ImageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = venue.ImageFile.ContentType });
                    }
                    venue.ImageUrl = blobClient.Uri.ToString();
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasActiveBookings = await _context.Booking.AnyAsync(b => b.VenueID == id);

            if (hasActiveBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it currently has active bookings.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue != null) _context.Venue.Remove(venue);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueID == id);
        }
    }
}
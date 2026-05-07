using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using Microsoft.Extensions.Configuration; // Added for IConfiguration
using Azure.Storage.Blobs;              // Added for Blob Storage
using Azure.Storage.Blobs.Models;       // Added for Blob Storage
using System.IO;                        // Added for Path.GetExtension

// Microsoft Corporation (2024). Overview of ASP.NET Core MVC. [Online] Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/overview [Accessed 8 April 2026].
namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly EventEaseContext _context;
        private readonly IConfiguration _configuration; // Added IConfiguration

        // Inject IConfiguration into the constructor
        public VenuesController(EventEaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Venue.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Added ImageFile to the Bind list to capture the upload from the UI
        public async Task<IActionResult> Create([Bind("VenueID,VenueName,VenueLocation,VenueCapacity,ImageUrl,ImageFile")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                // Check if a file was uploaded
                if (venue.ImageFile != null && venue.ImageFile.Length > 0)
                {
                    // Get connection string for Azurite from appsettings.json
                    string connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Create/Get the container named 'venue-images' and set access to public
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("venue-images");
                    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                    // Generate a unique file name using a GUID to prevent overwriting
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(venue.ImageFile.FileName);
                    BlobClient blobClient = containerClient.GetBlobClient(fileName);

                    // Upload the stream to local Azurite storage
                    using (var stream = venue.ImageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = venue.ImageFile.ContentType });
                    }

                    // Save the generated Blob URI to the ImageUrl property to store in the database
                    venue.ImageUrl = blobClient.Uri.ToString();
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Added ImageFile to the Bind list here
        public async Task<IActionResult> Edit(int id, [Bind("VenueID,VenueName,VenueLocation,VenueCapacity,ImageUrl,ImageFile")] Venue venue)
        {
            if (id != venue.VenueID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Optional logic: If they upload a new image during Edit, replace the old URL
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

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
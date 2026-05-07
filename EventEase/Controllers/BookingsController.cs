using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseContext _context;

        public BookingsController(EventEaseContext context)
        {
            _context = context;
        }

        // PHASE 3: Search by BookingID or Event Name (Consolidated View)
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var bookings = _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                // Checks for Event Name (case-insensitive) or exact Booking ID
                bookings = bookings.Where(b => b.Event!.EventName!.Contains(searchString)
                                            || b.BookingID.ToString() == searchString);
            }

            return View(await bookings.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        public IActionResult Create()
        {
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName");
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,VenueID,EventID,BookingStartDate,BookingEndDate,BookingStatus")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // PHASE 2: Inclusive Overlap Check (Blocks same-day double bookings)
                var existingBookings = await _context.Booking
                    .Where(b => b.VenueID == booking.VenueID)
                    .ToListAsync();

                bool isOverlap = existingBookings.Any(b =>
                    booking.BookingStartDate <= b.BookingEndDate &&
                    booking.BookingEndDate >= b.BookingStartDate);

                if (isOverlap)
                {
                    TempData["ErrorMessage"] = "The venue is already booked for the selected date. Please choose a different venue or date.";
                    ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
                    ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking successfully created!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) return NotFound();

            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,VenueID,EventID,BookingStartDate,BookingEndDate,BookingStatus")] Booking booking)
        {
            if (id != booking.BookingID) return NotFound();

            if (ModelState.IsValid)
            {
                var existingBookings = await _context.Booking
                    .Where(b => b.VenueID == booking.VenueID && b.BookingID != booking.BookingID)
                    .ToListAsync();

                bool isOverlap = existingBookings.Any(b =>
                    booking.BookingStartDate <= b.BookingEndDate &&
                    booking.BookingEndDate >= b.BookingStartDate);

                if (isOverlap)
                {
                    TempData["ErrorMessage"] = "Update failed: The venue is already reserved for the new timeframe.";
                    ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
                    ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
                    return View(booking);
                }

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null) _context.Booking.Remove(booking);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Booking deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingID == id);
        }
    }
}
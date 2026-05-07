/* EventEase Elite - Bookings Infrastructure
   Author: Joshua Marc Lourens
   Description: The primary logic engine for venue allocation. 
   Implements high-precision conflict detection (inclusive overlaps) and 
   pre-flight infrastructure checks for data integrity.
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

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseContext _context;

        public BookingsController(EventEaseContext context)
        {
            _context = context;
        }

        // GET: Bookings
        // Logic: Implements administrative search functionality (Requirement C)
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            // Using Eager Loading to consolidate Venue and Event data (Requirement C)
            var bookings = _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                // Specialist search by Booking ID or partial Event Name
                bookings = bookings.Where(b => b.Event!.EventName!.Contains(searchString)
                                            || b.BookingID.ToString() == searchString);
            }

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
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

        // GET: Bookings/Create
        public IActionResult Create()
        {
            // PRE-FLIGHT CHECK: Block access if basic infrastructure (Venues/Events) is missing
            var venueCount = _context.Venue.Count();
            var eventCount = _context.Event.Count();

            if (venueCount == 0 || eventCount == 0)
            {
                TempData["ErrorMessage"] = "SYSTEM ALERT: Register at least one Venue and one Event before creating a Booking.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName");
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,VenueID,EventID,BookingStartDate,BookingEndDate,BookingStatus")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                /* --- REINFORCED INCLUSIVE CONFLICT CHECK --- 
                   Formula: (StartA <= EndB) AND (EndA >= StartB)
                   This logic captures same-day overlaps and ensures no venue is double-booked.
                */
                bool isDoubleBooked = await _context.Booking.AnyAsync(b =>
                    b.VenueID == booking.VenueID &&
                    b.BookingStatus != "Cancelled" &&
                    booking.BookingStartDate <= b.BookingEndDate &&
                    booking.BookingEndDate >= b.BookingStartDate);

                if (isDoubleBooked)
                {
                    // Adding a model error so the specialist sees the conflict in the form summary
                    ModelState.AddModelError("", "CONFLICT DETECTED: This venue is already reserved for the selected date(s).");
                }
                else
                {
                    try
                    {
                        _context.Add(booking);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Booking confirmed and record generated.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException)
                    {
                        ModelState.AddModelError("", "DATA ERROR: A synchronization issue occurred. Verify your selection.");
                    }
                }
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) return NotFound();

            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,VenueID,EventID,BookingStartDate,BookingEndDate,BookingStatus")] Booking booking)
        {
            if (id != booking.BookingID) return NotFound();

            if (ModelState.IsValid)
            {
                /* --- INCLUSIVE CONFLICT CHECK (EDIT MODE) --- 
                   Ensuring we don't compare the booking against itself (b.BookingID != id).
                */
                bool isConflict = await _context.Booking.AnyAsync(b =>
                    b.BookingID != id &&
                    b.VenueID == booking.VenueID &&
                    b.BookingStatus != "Cancelled" &&
                    booking.BookingStartDate <= b.BookingEndDate &&
                    booking.BookingEndDate >= b.BookingStartDate);

                if (isConflict)
                {
                    ModelState.AddModelError("", "MODIFICATION BLOCKED: Updated dates conflict with another confirmed booking.");
                }
                else
                {
                    try
                    {
                        _context.Update(booking);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Booking record updated.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BookingExists(booking.BookingID)) return NotFound();
                        else throw;
                    }
                }
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
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

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking record successfully purged from the ledger.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id) => _context.Booking.Any(e => e.BookingID == id);
    }
}

/* TECHNICAL REFERENCES
   ---------------------------------------------------
   1. Inclusive Boundary Detection: Implementing (StartA <= EndB && EndA >= StartB) to catch same-day overlaps.
   2. Referential Integrity: Utilizing pre-flight checks to ensure parent records (Venue/Event) exist.
   3. Eager Loading (.Include): Consolidating multiple entities for administrative displays (Requirement C).
   4. ModelState Validation: Mapping server-side business rules to the UI alert system (Requirement B).
*/
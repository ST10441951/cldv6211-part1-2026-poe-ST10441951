/* EventEase Elite - Bookings Infrastructure
   Author: Joshua Marc Lourens
   Description: The primary logic engine for venue allocation. 
   Implements temporal conflict detection to prevent overlapping reservations.
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
        // Logic: Implements Phase 3 search functionality for specialist record retrieval.
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var bookings = _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                // Querying for partial Event Name matches or exact Booking ID
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
                /* --- ELITE CONFLICT DETECTION GATE --- 
                   Scanning the database for any existing confirmed bookings at the 
                   same venue that overlap with the requested timeline.
                */
                bool isDoubleBooked = await _context.Booking.AnyAsync(b =>
                    b.VenueID == booking.VenueID &&
                    b.BookingStatus != "Cancelled" &&
                    ((booking.BookingStartDate >= b.BookingStartDate && booking.BookingStartDate < b.BookingEndDate) ||
                     (booking.BookingEndDate > b.BookingStartDate && booking.BookingEndDate <= b.BookingEndDate)));

                if (isDoubleBooked)
                {
                    // Specialist Feedback: Blocking the transaction to protect venue integrity
                    ModelState.AddModelError("", "CONFLICT DETECTED: This venue is already reserved for the selected dates.");
                }
                else
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking ID #" + booking.BookingID + " successfully confirmed.";
                    return RedirectToAction(nameof(Index));
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
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking record updated successfully.";
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
   1. Date Range Logic: Implementation of overlapping interval detection for scheduling.
   2. LINQ to Entities: Use of .AnyAsync() for lightweight database existence checks.
   3. MVC Lifecycle: Handling complex POST back scenarios with ViewData and SelectLists.
   4. UI Alerts: Leveraging TempData for tactical specialist feedback.
*/
/* EventEase Elite - Events Infrastructure
   Author: Joshua Marc Lourens
   Description: Manages administrative event definitions. 
   Implements referential integrity gates to prevent orphaned booking data and 
   includes pre-flight infrastructure checks for stable record creation.
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
    public class EventsController : Controller
    {
        private readonly EventEaseContext _context;

        public EventsController(EventEaseContext context)
        {
            _context = context;
        }

        // GET: Events
        // Logic: Implements administrative search functionality to retrieve events by name or description.
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var events = _context.Event.Include(e => e.Venue).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.EventName!.Contains(searchString)
                                        || e.EventDescription!.Contains(searchString));
            }

            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        // Logic: Eager loads the associated Venue and Bookings to provide a comprehensive event profile.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            // SPECIALIST CHECK: Every event requires an existing Venue for allocation.
            // This prevents system crashes during the registration process.
            if (!_context.Venue.Any())
            {
                // UI Feedback: Forcing the specialist to register infrastructure (Venues) first.
                TempData["ErrorMessage"] = "SYSTEM ALERT: No Venues detected. You must register a Venue before creating an Event schedule.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,EventName,EventDescription,EventStartDate,EventEndDate,VenueID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event record successfully registered.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", @event.VenueID);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();

            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", @event.VenueID);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,EventName,EventDescription,EventStartDate,EventEndDate,VenueID")] Event @event)
        {
            if (id != @event.EventID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event record updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName", @event.VenueID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // REFERENTIAL INTEGRITY CHECK: Block deletion if records are linked to prevent data orphaning.
            // This maintains the stability of the administrative ledger.
            bool hasLinkedBookings = await _context.Booking.AnyAsync(b => b.EventID == id);

            if (hasLinkedBookings)
            {
                // BLOCK THE DELETE and provide tactical feedback to the interface.
                TempData["ErrorMessage"] = "CANNOT DELETE: This event is currently linked to active bookings. Remove associated bookings first.";
                return RedirectToAction(nameof(Index));
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                _context.Event.Remove(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event record successfully decommissioned.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id) => _context.Event.Any(e => e.EventID == id);
    }
}

/* TECHNICAL REFERENCES
   ---------------------------------------------------
   1. Referential Integrity Logic: Validating database constraints at the application level to ensure data stability.
   2. Eager Loading (.Include): Aggregating related Venue and Booking datasets in a single efficient query.
   3. UI Error Handling: Implementation of TempData-based global alert systems for real-time specialist feedback.
   4. Async Programming: Implementation of Task-based asynchronous controller actions for cloud-ready infrastructure.
*/
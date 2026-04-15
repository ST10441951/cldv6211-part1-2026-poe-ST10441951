using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Data
{
    public class EventEaseContext : DbContext
    {
        public EventEaseContext(DbContextOptions<EventEaseContext> options)
            : base(options)
        {
        }

        public DbSet<EventEase.Models.Venue> Venue { get; set; } = default!;
        public DbSet<EventEase.Models.Booking> Booking { get; set; } = default!;
        public DbSet<EventEase.Models.Event> Event { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Booking -> Venue relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Venue)
                .WithMany() // A Venue can have many Bookings
                .HasForeignKey(b => b.VenueID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Booking -> Event relationship
            // This clears up the 'Unable to determine relationship' error
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany()
                .HasForeignKey(b => b.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Event -> Booking relationship
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Booking)
                .WithMany()
                .HasForeignKey(e => e.BookingID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
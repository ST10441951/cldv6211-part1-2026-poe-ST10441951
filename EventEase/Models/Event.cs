using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        [Display(Name = "Event Name")]
        public string? EventName { get; set; }

        [Display(Name = "Description")]
        public string? EventDescription { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateOnly EventStartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateOnly EventEndDate { get; set; }

        // NEW: Link to Venue
        [Display(Name = "Primary Venue")]
        public int? VenueID { get; set; }

        [ForeignKey("VenueID")]
        public virtual Venue? Venue { get; set; }

        // Navigation property for bookings
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

    // Microsoft Corporation(2024). Relationships - EF Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/modeling/relationships [Accessed 10 April 2026].
    // Microsoft Corporation (2024). Introduction to Entity Framework Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/ [Accessed 11 April 2026].
}

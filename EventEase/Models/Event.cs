using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Display(Name = "Booking Reference")]
        public int BookingID { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateOnly EventStartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateOnly EventEndDate { get; set; }

        [Required]
        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Display(Name = "Description")]
        public string? EventDescription { get; set; }

        // Navigation properties
        public virtual Booking? Booking { get; set; }

        // Microsoft Corporation(2024). Relationships - EF Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/modeling/relationships [Accessed 10 April 2026].

        // Microsoft Corporation (2024). Introduction to Entity Framework Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/ [Accessed 11 April 2026].

    }
}
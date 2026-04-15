using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required]
        [Display(Name = "Venue")]
        public int VenueID { get; set; }

        [Display(Name = "Event")]
        public int? EventID { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateOnly BookingStartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateOnly BookingEndDate { get; set; }

        [Display(Name = "Status")]
        public string? BookingStatus { get; set; }

        // Navigation properties
        public virtual Venue? Venue { get; set; }
        public virtual Event? Event { get; set; }

        // Microsoft Corporation(2024). Relationships - EF Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/modeling/relationships [Accessed 10 April 2026].

        // Microsoft Corporation (2024). Introduction to Entity Framework Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/ [Accessed 11 April 2026].

    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Added for [NotMapped]
using Microsoft.AspNetCore.Http; // Added for IFormFile
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }

        [Required]
        [Display(Name = "Venue Name")]
        public string VenueName { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string VenueLocation { get; set; }

        [Required]
        [Display(Name = "Capacity")]
        public int VenueCapacity { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        // Handles the actual file upload from the UI (Not saved as a DB column)
        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        // A venue can be used for many bookings
        public virtual ICollection<Booking>? Bookings { get; set; }

        // Microsoft Corporation(2024). Relationships - EF Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/modeling/relationships [Accessed 10 April 2026].

        // Microsoft Corporation (2024). Introduction to Entity Framework Core. [Online] Available at: https://learn.microsoft.com/en-us/ef/core/ [Accessed 11 April 2026].
    }
}
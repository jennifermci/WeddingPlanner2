using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage="First name must be at least 2 characters")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2, ErrorMessage="Last name must be at least 2 characters")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } =  DateTime.Now;

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public List<Wedding> WeddingList { get; set; }
        public List<RSVP> RSVPList { get; set; }

    }
}
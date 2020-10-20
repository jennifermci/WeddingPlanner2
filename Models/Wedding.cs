using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }

        [Required]
        public string WedderOne { get; set; }

        [Required]        
        public string WedderTwo { get; set; }

        [Required]
        [RestrictedDate (ErrorMessage="Date must be in the future")]

        [Display(Name = "Date of Wedding")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        public string WeddingAddress { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } =  DateTime.Now;

        public User Creator { get; set; }
        public int CreatorId { get; set; }
        public List<RSVP> RSVPList { get; set; }

    }

    public class RestrictedDate : ValidationAttribute
    {
        public override bool IsValid(object date) 
        {
            return (DateTime)date > DateTime.Now;
        }
    }

}
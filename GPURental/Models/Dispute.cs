using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public enum DisputeStatus
    {
        Submitted,
        UnderReview,
        Resolved,
        Rejected
    }

    public class Dispute
    {
        [Key]
        public string DisputeId { get; set; }

        [Required]
        public string RentalJobId { get; set; } // Foreign Key
        public RentalJob RentalJob { get; set; }

        [Required]
        public string RaisedByUserId { get; set; } // Foreign Key
        public User RaisedByUser { get; set; }

        public string Reason { get; set; }
        public string Description { get; set; }
        public DisputeStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
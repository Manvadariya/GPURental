using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public class Invoice
    {
        [Key]
        public string InvoiceId { get; set; } // Primary Key

        [Required]
        public string RentalJobId { get; set; } // Foreign Key
        public RentalJob RentalJob { get; set; }

        [Required]
        public string UserId { get; set; } // Foreign Key
        public User User { get; set; }

        public DateTime IssueDate { get; set; }
        public int TotalInCents { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
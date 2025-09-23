using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public class JobLog
    {
        [Key]
        public string LogId { get; set; } // Primary Key

        [Required]
        public string RentalJobId { get; set; } // Foreign Key
        public RentalJob RentalJob { get; set; }

        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}
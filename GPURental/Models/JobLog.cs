using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public class JobLog
    {
        [Key]
        public int LogId { get; set; } // Primary Key

        [Required]
        public int RentalJobId { get; set; } // Foreign Key
        public RentalJob RentalJob { get; set; }

        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}
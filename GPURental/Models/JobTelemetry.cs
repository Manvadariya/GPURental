using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public class JobTelemetry
    {
        [Key]
        public int TelemetryId { get; set; } // Primary Key

        [Required]
        public int RentalJobId { get; set; } // Foreign Key
        public RentalJob RentalJob { get; set; }

        public DateTime Timestamp { get; set; }
        public int GpuUtilizationPercent { get; set; }
        public int GpuMemoryUsedMB { get; set; }
    }
}
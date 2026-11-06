using System.ComponentModel.DataAnnotations;

namespace AgriStreamIoT.Models
{
    public class Device
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DeviceType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FieldLocation { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string OperationalStatus { get; set; } = "Online";

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}

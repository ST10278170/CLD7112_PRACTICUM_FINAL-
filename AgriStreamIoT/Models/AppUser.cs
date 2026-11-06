using System.ComponentModel.DataAnnotations;

namespace AgriStreamIoT.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Farmer";
    }
}
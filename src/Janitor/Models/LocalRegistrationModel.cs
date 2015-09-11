using System.ComponentModel.DataAnnotations;

namespace Janitor.Models
{
    public class LocalRegistrationModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string First { get; set; }
        [Required]
        public string Last { get; set; }
    }
}
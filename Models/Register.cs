using System.ComponentModel.DataAnnotations;

namespace Reddit.Models
{
    public class Register
    {
        [Required]
        public string? Email {  get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}

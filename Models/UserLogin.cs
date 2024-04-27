using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Pole jest wymagane")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy adres Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane")]
		public string Password { get; set; }
    }
}

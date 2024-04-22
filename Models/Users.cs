using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Users
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy adres Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane")]
        [StringLength(99, MinimumLength = 6, ErrorMessage = "Hasło musi zawierać conajmiej 6 znaków")]
        public string Password { get; set; }
    }
}

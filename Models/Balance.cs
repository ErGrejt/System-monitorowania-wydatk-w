using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Balance
    {
        public int Id { get; set; }

        public int UserID { get; set; }
		[Required(ErrorMessage = "Pole jest wymagane")]
		[IsDecimal(ErrorMessage = "Proszę wprowadzić prawidłową liczbę.")]
		public decimal PLN { get; set; }
		[Required(ErrorMessage = "Pole jest wymagane")]
		[IsDecimal(ErrorMessage = "Proszę wprowadzić prawidłową liczbę.")]
		public decimal EURO { get; set;}
    }

    
}

using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Balance
    {
        public int Id { get; set; }
        public int UserID { get; set; }
		[Required(ErrorMessage = "Pole jest wymagane")]
		[Range(0, double.MaxValue, ErrorMessage = "Wpisz poprawną liczbę")]
		public decimal PLN { get; set; }
		[Required(ErrorMessage = "Pole jest wymagane")]
		[Range(0, double.MaxValue, ErrorMessage = "Wpisz poprawną liczbę")]
		public decimal EURO { get; set;}
    }

    
}

using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class FormData
    {
		[Required(ErrorMessage = "Pole jest wymagane")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Pole jest wymagane")]
		[Range(0, double.MaxValue, ErrorMessage = "Wpisz poprawną liczbę")]
		public decimal Price { get; set; }
        public int Currency {  get; set; }
        public int Category {  get; set; }
        public int Who { get; set; }
    }

}

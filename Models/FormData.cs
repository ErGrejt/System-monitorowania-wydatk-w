using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class FormData
    {
		[Required(ErrorMessage = "Pole jest wymagane")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Pole jest wymagane")]
		[IsDecimal(ErrorMessage = "Proszę wprowadzić prawidłową liczbę.")]
		public decimal Price { get; set; }

        public int Currency {  get; set; }

        public int Category {  get; set; }

        public int Who { get; set; }
		
		//public decimal PLN { get; set; }
		
		//public decimal EURO { get; set; }
    }

	public class IsDecimalAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			decimal number;
			if (decimal.TryParse(value.ToString(), out number))
			{
				return ValidationResult.Success;
			}
			else
			{
				return new ValidationResult("Wprowadź prawidłową liczbę.");
			}
		}
	}
}

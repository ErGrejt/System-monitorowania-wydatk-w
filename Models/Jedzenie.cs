namespace WebApplication1.Models
{
    public class Jedzenie
    {
        public int Id { get; set; }

        public int UserID { get; set; }

        public string Nazwa { get; set; }

        public decimal Cena { get; set; }

        public int Waluta { get; set; }
    }
}

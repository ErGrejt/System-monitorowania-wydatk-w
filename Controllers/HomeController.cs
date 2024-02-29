using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using MySql.Data.MySqlClient;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.Http;
using System.Text.Json;
using freecurrencyapi;
using freecurrencyapi.Helpers;



namespace WebApplication1.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
            
        }

          
        


        //Baza danych?
        [HttpPost]
        public IActionResult AddTransaction(FormData model)
        {
            
            if (ModelState.IsValid)
            {
                if(model.Category == 1)
                {
                    AddPrzelew(model);
                } else if(model.Category == 2)
                {
                    AddFood(model);
                } else if(model.Category == 3)
                {
                    AddHealth(model);
                } else if(model.Category == 4)
                {
                    AddZach(model);
                } 
            }
            return RedirectToAction("index","Home");
        }
        
        public IActionResult AddSaldoTransaction(FormData model)
        {
            AddSaldo(model);
            return RedirectToAction("Form", "Home");
        }



        public IActionResult Index()
        {
            string connectionString = "Server=localhost;Database=System monitorowania wydatków;User=root;Password=;";
            MySqlConnection connection = new MySqlConnection(connectionString);

           

            connection.Open();
            //Zapytanie przelewy
            string queryprzelew = "SELECT * FROM Przelewy";
            MySqlCommand commandprzelew = new MySqlCommand(queryprzelew, connection);
            using (MySqlDataReader readerPrzelew = commandprzelew.ExecuteReader())
            {
                DataTable przelewTable = new DataTable();
                przelewTable.Load(readerPrzelew);
                ViewData["PrzelewyData"] = przelewTable;
            }
            //Zapytanie jedzenie
            string queryfood = "SELECT * FROM jedzenie";
            MySqlCommand commandfood = new MySqlCommand(queryfood, connection);
            using (MySqlDataReader readerFood = commandfood.ExecuteReader())
            {
                DataTable foodTable = new DataTable();
                foodTable.Load(readerFood);
                ViewData["JedzenieData"] = foodTable;
            }
            //Zapytanie zdrowie
            string queryhealth = "SELECT * FROM Zdrowie";
            MySqlCommand commandhealth = new MySqlCommand(queryhealth, connection);
            using (MySqlDataReader readerHealth = commandhealth.ExecuteReader())
            {
                DataTable healthTable = new DataTable();
                healthTable.Load(readerHealth);
                ViewData["ZdrowieData"] = healthTable;
            }
            //Zapytanie zachcianki
            string queryzachcianka = "SELECT * FROM Zachcianki";
            MySqlCommand commandzachcianka = new MySqlCommand(queryzachcianka, connection);
            using (MySqlDataReader readerZachcianka = commandzachcianka.ExecuteReader())
            {
                DataTable ZachciankaTable = new DataTable();
                ZachciankaTable.Load(readerZachcianka);
                ViewData["ZachciankaData"] = ZachciankaTable;
            }
            //Pobierz saldo
            string querysaldo = "SELECT * FROM saldo";
            MySqlCommand commandsaldo = new MySqlCommand(querysaldo, connection);
            using (MySqlDataReader readerSaldo = commandsaldo.ExecuteReader())
            {
                DataTable saldoTable = new DataTable();
                saldoTable.Load(readerSaldo);
                ViewData["SaldoData"] = saldoTable;
            }
            //Zapytania pobieraj¹ce iloœæ wydanych pieniêdzy w z³otówkach z ka¿dej tabeli (poza przelewami)
            //Zachcianki
            string queryobliczaniezachcianki = "SELECT COALESCE(SUM(cena),0) FROM zachcianki WHERE waluta='1'";
            MySqlCommand comobzachcianki = new MySqlCommand(queryobliczaniezachcianki, connection);
            decimal sumawydatkowzachcianki = Convert.ToDecimal(comobzachcianki.ExecuteScalar());
            ViewData["wydatkizachcianki"] = sumawydatkowzachcianki;
            //Jedzenie
            string queryobliczaniejedzenie = "SELECT COALESCE(SUM(cena),0) FROM jedzenie WHERE waluta='1'";
            MySqlCommand comobjedzenie = new MySqlCommand(queryobliczaniejedzenie, connection);
            decimal sumawydatkowjedzenie = Convert.ToDecimal(comobjedzenie.ExecuteScalar());
            ViewData["wydatkijedzenie"] = sumawydatkowjedzenie;
            //Zdrowie
            string queryobliczaniezdrowie = "SELECT COALESCE(SUM(cena),0) FROM zdrowie WHERE waluta='1'";
            MySqlCommand comobzdrowie = new MySqlCommand(queryobliczaniezdrowie, connection);
            decimal sumawydatkowzdrowie = Convert.ToDecimal(comobzdrowie.ExecuteScalar());
            ViewData["wydatkizdrowie"] = sumawydatkowzdrowie;

            //Zapytania pobieraj¹ce iloœæ wydanych pieniêdzy w euro z ka¿dej tabeli (poza przelewami)
            string queryobliczaniezachciankieur = "SELECT COALESCE(SUM(cena),0) FROM zachcianki WHERE waluta='2'";
            MySqlCommand comobzachciankieur = new MySqlCommand(queryobliczaniezachciankieur, connection);
            decimal sumawydatkowzachciankieur = Convert.ToDecimal(comobzachciankieur.ExecuteScalar());
            ViewData["wydatkizachciankieur"] = sumawydatkowzachciankieur;
            //Jedzenie
            string queryobliczaniejedzenieeur = "SELECT COALESCE(SUM(cena),0) FROM jedzenie WHERE waluta='2'";
            MySqlCommand comobjedzenieeur = new MySqlCommand(queryobliczaniejedzenieeur, connection);
            decimal sumawydatkowjedzenieeur = Convert.ToDecimal(comobjedzenieeur.ExecuteScalar());
            ViewData["wydatkijedzenieeur"] = sumawydatkowjedzenieeur;
            //Zdrowie
            string queryobliczaniezdrowieeur = "SELECT COALESCE(SUM(cena),0) FROM zdrowie WHERE waluta='2'";
            MySqlCommand comobzdrowieeur = new MySqlCommand(queryobliczaniezdrowieeur, connection);
            decimal sumawydatkowzdrowieeur = Convert.ToDecimal(comobzdrowieeur.ExecuteScalar());
            ViewData["wydatkizdrowieeur"] = sumawydatkowzdrowieeur;
            //Przelew wychodz¹cy w z³otówkach
            string queryobliczanieprzelewy1 = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='1' AND Kierunek='1'";
            MySqlCommand comobprzelewy1 = new MySqlCommand(queryobliczanieprzelewy1 , connection);
            decimal sumaprzelewowzl = Convert.ToDecimal(comobprzelewy1.ExecuteScalar());
            ViewData["przelewwychodzacyzl"] = sumaprzelewowzl;
            //Przelew przychodz¹cy w z³otówkach
            string queryobliczanieprzelewy2 = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='1' AND Kierunek='2'";
            MySqlCommand comobprzelewy2 = new MySqlCommand(queryobliczanieprzelewy2, connection);
            decimal sumaprzelewowzlwych = Convert.ToDecimal(comobprzelewy2.ExecuteScalar());
            ViewData["przelewprzychodzacyzl"] = sumaprzelewowzlwych;
            //Przelew wychodz¹cy w euro
            string queryobliczanieprzelewy1eur = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='2' AND Kierunek='1'";
            MySqlCommand comobprzelewy1eur = new MySqlCommand(queryobliczanieprzelewy1eur, connection);
            decimal sumaprzelewowzleur = Convert.ToDecimal(comobprzelewy1eur.ExecuteScalar());
            ViewData["przelewwychodzacyeuro"] = sumaprzelewowzleur;
            //Przelew przychodz¹cy w euro
            string queryobliczanieprzelewy2eur = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='2' AND Kierunek='2'";
            MySqlCommand comobprzelewy2eur = new MySqlCommand(queryobliczanieprzelewy2eur, connection);
            decimal sumaprzelewowzlwycheur = Convert.ToDecimal(comobprzelewy2eur.ExecuteScalar());
            ViewData["przelewprzychodzacyeuro"] = sumaprzelewowzlwycheur;

            

            connection.Close();

            //Pobieranie wartoœci EUR do PLN z freecurrencyApi
            var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
            string waluty = fx.Latest("EUR", "PLN");
            ViewData["KursEuro"] = waluty;


            return View();
        }

        public IActionResult Form()
        {
            string connectionString = "Server=localhost;Database=System monitorowania wydatków;User=root;Password=;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            //Zapytanie saldo
            string querysaldo = "SELECT * FROM saldo;";
            MySqlCommand commandsaldo = new MySqlCommand(querysaldo, connection);
            connection.Open();
            using (MySqlDataReader readerSaldo = commandsaldo.ExecuteReader())
            {
                DataTable SaldoTable = new DataTable();
                SaldoTable.Load(readerSaldo);
                ViewData["SaldoData"] = SaldoTable;
            }
            connection.Close();
            return View();
        }
        public IActionResult ExChange()
        {
            var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
            string kursEURtoPLN = fx.Latest("EUR", "PLN");
            ViewData["kursEURtoPLN"] = kursEURtoPLN;

            string kursPLNtoEUR = fx.Latest("PLN", "EUR");
            ViewData["kursPLNtoEUR"] = kursPLNtoEUR;

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        //Dodanie do tabeli Jedzenie
        public IActionResult AddFood(FormData model)
        {
            if (ModelState.IsValid)
            {
                Jedzenie newFood = new Jedzenie
                {
                    Nazwa = model.Name,
                Cena = model.Price,
                Waluta = model.Currency
        

                };

                _context.Jedzenie.Add(newFood);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        //Dodanie do tabeli Zdrowie
        public IActionResult AddHealth(FormData model)
        {
            if (ModelState.IsValid)
            {
                Zdrowie newHealth = new Zdrowie
                {
                    Nazwa = model.Name,
                    Cena = model.Price,
                    Waluta = model.Currency


                };

                _context.Zdrowie.Add(newHealth);
                _context.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            return RedirectToAction("Index", "Home");
        }
        //Dodanie do tabeli Zachcianki
        public IActionResult AddZach(FormData model)
        {
            if (ModelState.IsValid)
            {
                Zachcianki newZach = new Zachcianki
                {
                    Nazwa = model.Name,
                    Cena = model.Price,
                    Waluta = model.Currency


                };

                _context.Zachcianki.Add(newZach);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        //Dodanie do tabeli Przelewy
        public IActionResult AddPrzelew(FormData model)
        {
            if (ModelState.IsValid)
            {
                Przelewy newPrzelew = new Przelewy
                {
                    Nazwa = model.Name,
                    Cena = model.Price,
                    Waluta = model.Currency,
                    Kierunek = model.Who


                };

                _context.Przelewy.Add(newPrzelew);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        //Dodanie do tabeli Saldo
        public IActionResult AddSaldo(FormData model)
        {
            
                Saldo newSaldo = new Saldo
                {
                    PLN = model.PLN,
                    EURO = model.EURO,


                };

                _context.Saldo.Add(newSaldo);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            
            return RedirectToAction("Index", "Home");
        }

    }
}

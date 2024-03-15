using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using MySql.Data.MySqlClient;
using System.Data;
using WebApplication1.Controllers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.Http;
using System.Text.Json;
using freecurrencyapi;
using freecurrencyapi.Helpers;
using Microsoft.AspNetCore.Http;

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
        public DataTable Queries(MySqlConnection connection, string query)
        {
            MySqlCommand command = new MySqlCommand(query, connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                DataTable Table = new DataTable();
                Table.Load(reader);
                return Table;
            }
        }
        public decimal Expenses(MySqlConnection connection, string query)
        {
            MySqlCommand command = new MySqlCommand(query, connection);
            decimal SUM = Convert.ToDecimal(command.ExecuteScalar());
            return SUM;
        }

        public IActionResult Index()
        {
            string connectionString = "Server=localhost;Database=System monitorowania wydatków;User=root;Password=;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
           
            //Zapytanie przelewy
            string queryprzelew = "SELECT * FROM Przelewy";
            ViewData["PrzelewyData"] = Queries(connection, queryprzelew);
            //Zapytanie jedzenie
            string queryfood = "SELECT * FROM jedzenie";
            ViewData["JedzenieData"] = Queries(connection, queryfood);
            //Zapytanie zdrowie
            string queryhealth = "SELECT * FROM Zdrowie";
            ViewData["ZdrowieData"] = Queries(connection, queryhealth);
            //Zapytanie zachcianki
            string queryzachcianka = "SELECT * FROM Zachcianki";
            ViewData["ZachciankaData"] = Queries(connection, queryzachcianka);
            //Pobierz saldo
            string querysaldo = "SELECT * FROM saldo";
            ViewData["SaldoData"] = Queries(connection, querysaldo);
            //Zapytania pobieraj¹ce iloœæ wydanych pieniêdzy w z³otówkach z ka¿dej tabeli (poza przelewami)
            //Zachcianki
            string queryobliczaniezachcianki = "SELECT COALESCE(SUM(cena),0) FROM zachcianki WHERE waluta='1'";
            ViewData["wydatkizachcianki"] = Expenses(connection, queryobliczaniezachcianki);
            //Jedzenie
            string queryobliczaniejedzenie = "SELECT COALESCE(SUM(cena),0) FROM jedzenie WHERE waluta='1'";
            ViewData["wydatkijedzenie"] = Expenses(connection, queryobliczaniejedzenie);
            //Zdrowie
            string queryobliczaniezdrowie = "SELECT COALESCE(SUM(cena),0) FROM zdrowie WHERE waluta='1'";
            ViewData["wydatkizdrowie"] = Expenses(connection, queryobliczaniezdrowie);
            //Zapytania pobieraj¹ce iloœæ wydanych pieniêdzy w euro z ka¿dej tabeli (poza przelewami)
            string queryobliczaniezachciankieur = "SELECT COALESCE(SUM(cena),0) FROM zachcianki WHERE waluta='2'";
            ViewData["wydatkizachciankieur"] = Expenses(connection, queryobliczaniezachciankieur);
            //Jedzenie
            string queryobliczaniejedzenieeur = "SELECT COALESCE(SUM(cena),0) FROM jedzenie WHERE waluta='2'";
            ViewData["wydatkijedzenieeur"] = Expenses(connection, queryobliczaniejedzenieeur);
            //Zdrowie
            string queryobliczaniezdrowieeur = "SELECT COALESCE(SUM(cena),0) FROM zdrowie WHERE waluta='2'";
            ViewData["wydatkizdrowieeur"] = Expenses(connection, queryobliczaniezdrowieeur); 
            //Przelew wychodz¹cy w z³otówkach
            string queryobliczanieprzelewy1 = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='1' AND Kierunek='1'";
            ViewData["przelewwychodzacyzl"] = Expenses(connection, queryobliczanieprzelewy1);
            //Przelew przychodz¹cy w z³otówkach
            string queryobliczanieprzelewy2 = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='1' AND Kierunek='2'";
            ViewData["przelewprzychodzacyzl"] = Expenses(connection, queryobliczanieprzelewy2);
            //Przelew wychodz¹cy w euro
            string queryobliczanieprzelewy1eur = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='2' AND Kierunek='1'";
            ViewData["przelewwychodzacyeuro"] = Expenses(connection, queryobliczanieprzelewy1eur);
            //Przelew przychodz¹cy w euro
            string queryobliczanieprzelewy2eur = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='2' AND Kierunek='2'";
            ViewData["przelewprzychodzacyeuro"] = Expenses(connection, queryobliczanieprzelewy2eur);

            connection.Close();

            //Pobieranie wartoœci EUR do PLN z freecurrencyApi
            var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
            string waluty = fx.Latest("EUR", "PLN");
            waluty = waluty.Substring(15, 12);
            waluty = waluty.Remove(1, 1).Insert(1, ",").Remove(4, 8);
            ViewData["kursEuro"] = waluty;

            //Test
            var IDuzytkownika = HttpContext.Session.GetInt32("UserId");
            ViewData["ID"] = IDuzytkownika;

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
            //Klucz freecurrency
            var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
            //Wyci¹ganie kursu eur -> pln
            string kursEURtoPLN = fx.Latest("EUR", "PLN");
            kursEURtoPLN = kursEURtoPLN.Substring(15, 12);
            kursEURtoPLN = kursEURtoPLN.Remove(1, 1).Insert(1, ",").Remove(4, 8);
            ViewData["kursEURtoPLN"] = kursEURtoPLN;
            //Wyci¹ganie kursu pln -> eur
            string kursPLNtoEUR = fx.Latest("PLN", "EUR");
            kursPLNtoEUR = kursPLNtoEUR.Substring(15, 12);
            kursPLNtoEUR = kursPLNtoEUR.Remove(1, 1).Insert(1, ",").Remove(4, 8);
            ViewData["kursPLNtoEUR"] = kursPLNtoEUR;

            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register() 
        { 
            return View(); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}

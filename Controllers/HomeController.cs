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
        public DataTable Queries(MySqlConnection connection, string query, int ID)
        {
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IDuzytkownika", ID);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                DataTable Table = new DataTable();
                Table.Load(reader);
                return Table;
            }
        }
        public decimal Expenses(MySqlConnection connection, string query, int ID)
        {
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IDuzytkownika", ID);
            decimal SUM = Convert.ToDecimal(command.ExecuteScalar());
            return SUM;
        }

        public IActionResult Index()
        {
            var IDuzytkownika = (int)HttpContext.Session.GetInt32("UserId");
            string connectionString = "Server=localhost;Database=System monitorowania wydatków;User=root;Password=;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
           
            //Zapytanie przelewy
            string queryprzelew = "SELECT * FROM Przelewy WHERE UserID = @IDuzytkownika";
            ViewData["PrzelewyData"] = Queries(connection, queryprzelew, IDuzytkownika);
            //Zapytanie jedzenie
            string queryfood = "SELECT * FROM jedzenie WHERE UserID = @IDuzytkownika";
            ViewData["JedzenieData"] = Queries(connection, queryfood, IDuzytkownika);
            //Zapytanie zdrowie
            string queryhealth = "SELECT * FROM Zdrowie WHERE UserID = @IDuzytkownika";
            ViewData["ZdrowieData"] = Queries(connection, queryhealth, IDuzytkownika);
            //Zapytanie zachcianki
            string queryzachcianka = "SELECT * FROM Zachcianki WHERE UserID = @IDuzytkownika";
            ViewData["ZachciankaData"] = Queries(connection, queryzachcianka, IDuzytkownika);
            //Pobierz saldo
            string querysaldo = "SELECT * FROM saldo WHERE UserID = @IDuzytkownika";
            ViewData["SaldoData"] = Queries(connection, querysaldo, IDuzytkownika);
            //Zapytania pobieraj¹ce iloœæ wydanych pieniêdzy w z³otówkach z ka¿dej tabeli (poza przelewami)
            //Zachcianki
            string queryobliczaniezachcianki = "SELECT COALESCE(SUM(cena),0) FROM zachcianki WHERE waluta='1' AND UserID = @IDuzytkownika";
            ViewData["wydatkizachcianki"] = Expenses(connection, queryobliczaniezachcianki, IDuzytkownika);
            //Jedzenie
            string queryobliczaniejedzenie = "SELECT COALESCE(SUM(cena),0) FROM jedzenie WHERE waluta='1' AND UserID = @IDuzytkownika";
            ViewData["wydatkijedzenie"] = Expenses(connection, queryobliczaniejedzenie, IDuzytkownika);
            //Zdrowie
            string queryobliczaniezdrowie = "SELECT COALESCE(SUM(cena),0) FROM zdrowie WHERE waluta='1' AND UserID = @IDuzytkownika";
            ViewData["wydatkizdrowie"] = Expenses(connection, queryobliczaniezdrowie, IDuzytkownika);
            //Zapytania pobieraj¹ce iloœæ wydanych pieniêdzy w euro z ka¿dej tabeli (poza przelewami)
            string queryobliczaniezachciankieur = "SELECT COALESCE(SUM(cena),0) FROM zachcianki WHERE waluta='2' AND UserID = @IDuzytkownika";
            ViewData["wydatkizachciankieur"] = Expenses(connection, queryobliczaniezachciankieur, IDuzytkownika);
            //Jedzenie
            string queryobliczaniejedzenieeur = "SELECT COALESCE(SUM(cena),0) FROM jedzenie WHERE waluta='2' AND UserID = @IDuzytkownika";
            ViewData["wydatkijedzenieeur"] = Expenses(connection, queryobliczaniejedzenieeur, IDuzytkownika);
            //Zdrowie
            string queryobliczaniezdrowieeur = "SELECT COALESCE(SUM(cena),0) FROM zdrowie WHERE waluta='2' AND UserID = @IDuzytkownika";
            ViewData["wydatkizdrowieeur"] = Expenses(connection, queryobliczaniezdrowieeur, IDuzytkownika); 
            //Przelew wychodz¹cy w z³otówkach
            string queryobliczanieprzelewy1 = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='1' AND Kierunek='1' AND UserID = @IDuzytkownika";
            ViewData["przelewwychodzacyzl"] = Expenses(connection, queryobliczanieprzelewy1, IDuzytkownika);
            //Przelew przychodz¹cy w z³otówkach
            string queryobliczanieprzelewy2 = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='1' AND Kierunek='2' AND UserID = @IDuzytkownika";
            ViewData["przelewprzychodzacyzl"] = Expenses(connection, queryobliczanieprzelewy2, IDuzytkownika);
            //Przelew wychodz¹cy w euro
            string queryobliczanieprzelewy1eur = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='2' AND Kierunek='1' AND UserID = @IDuzytkownika";
            ViewData["przelewwychodzacyeuro"] = Expenses(connection, queryobliczanieprzelewy1eur, IDuzytkownika);
            //Przelew przychodz¹cy w euro
            string queryobliczanieprzelewy2eur = "SELECT COALESCE(SUM(cena),0) FROM przelewy WHERE waluta='2' AND Kierunek='2' AND UserID = @IDuzytkownika";
            ViewData["przelewprzychodzacyeuro"] = Expenses(connection, queryobliczanieprzelewy2eur, IDuzytkownika);

            connection.Close();

            //Pobieranie wartoœci EUR do PLN z freecurrencyApi
            var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
            string waluty = fx.Latest("EUR", "PLN");
            waluty = waluty.Substring(15, 12);
            waluty = waluty.Remove(1, 1).Insert(1, ",").Remove(4, 8);
            ViewData["kursEuro"] = waluty;

            

            return View();
        }

        public IActionResult Form()
        {
            var IDuzytkownika = (int)HttpContext.Session.GetInt32("UserId");
            string connectionString = "Server=localhost;Database=System monitorowania wydatków;User=root;Password=;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            //Zapytanie saldo
            string querysaldo = "SELECT * FROM saldo WHERE UserID =@IDuzytkownika";
            MySqlCommand commandsaldo = new MySqlCommand(querysaldo, connection);
            commandsaldo.Parameters.AddWithValue("@IDuzytkownika", IDuzytkownika);
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

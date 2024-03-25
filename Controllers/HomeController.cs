using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
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
			if (HttpContext.Session.GetInt32("UserId") != null)
			{

				int IDuzytkownika = (int)HttpContext.Session.GetInt32("UserId");
				ViewData["ID"] = IDuzytkownika;

				string connectionString = "Server=localhost;Database=System monitorowania wydatków;User=root;Password=;";
				MySqlConnection connection = new MySqlConnection(connectionString);
				connection.Open();

				//Zapytanie przelewy
				ViewData["PrzelewyData"] = _context.Przelewy.Where(p => p.UserID == IDuzytkownika).ToList();
				//Zapytanie jedzenie
				ViewData["JedzenieData"] = _context.Jedzenie.Where(p => p.UserID == IDuzytkownika).ToList();
				//Zapytanie zdrowie
				ViewData["ZdrowieData"] = _context.Zdrowie.Where(p => p.UserID == IDuzytkownika).ToList();
				//Zapytanie zachcianki
				ViewData["ZachciankaData"] = _context.Zachcianki.Where(p => p.UserID == IDuzytkownika).ToList();
				//Pobierz saldo
				ViewData["SaldoData"] = _context.Saldo.Where(p => p.UserID == IDuzytkownika).ToList();
				//Zapytania pobieraj¹ce iloœæ wydanych pieniêdzy w z³otówkach z ka¿dej tabeli (poza przelewami)
				//Zachcianki
				ViewData["wydatkizachcianki"] = _context.Zachcianki
					.Where(z => z.Waluta == 1 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();
				//Jedzenie
				ViewData["wydatkijedzenie"] = _context.Jedzenie
					.Where(z => z.Waluta == 1 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();
				//Zdrowie
				ViewData["wydatkizdrowie"] = _context.Zdrowie
					.Where(z => z.Waluta == 1 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();
				//Zapytania pobieraj¹ce iloœæ wydanych pieniêdzy w euro z ka¿dej tabeli (poza przelewami)
				ViewData["wydatkizachciankieur"] = _context.Zachcianki
					.Where(z => z.Waluta == 2 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();
				//Jedzenie
				ViewData["wydatkijedzenieeur"] = _context.Jedzenie
					.Where(z => z.Waluta == 2 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();
				//Zdrowie
				ViewData["wydatkizdrowieeur"] = _context.Zdrowie
					.Where(z => z.Waluta == 2 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();
				//Przelew wychodz¹cy w z³otówkach
				ViewData["przelewwychodzacyzl"] = _context.Przelewy
					.Where(z => z.Waluta == 1 && z.Kierunek == 1 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();
				//Przelew przychodz¹cy w z³otówkach
				ViewData["przelewprzychodzacyzl"] = _context.Przelewy
					.Where(z => z.Waluta == 1 && z.Kierunek == 2 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();
				//Przelew wychodz¹cy w euro
				ViewData["przelewwychodzacyeuro"] = _context.Przelewy
					.Where(z => z.Waluta == 2 && z.Kierunek == 1 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();
				//Przelew przychodz¹cy w euro
				ViewData["przelewprzychodzacyeuro"] = _context.Przelewy
					.Where(z => z.Waluta == 2 && z.Kierunek == 2 && z.UserID == IDuzytkownika)
					.Select(z => z.Cena)
					.Sum();

				connection.Close();

				//Pobieranie wartoœci EUR do PLN z freecurrencyApi
				var fx = new FreeCurrencyApi("fca_live_ivHc8n89DK5t3yqGMryyu2RO2vzyxLV2zuQYg51T");
				string waluty = fx.Latest("EUR", "PLN");
				waluty = waluty.Substring(15, 12);
				waluty = waluty.Remove(1, 1).Insert(1, ",").Remove(4, 8);
				ViewData["kursEuro"] = waluty;

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		public IActionResult Form()
		{
			if (HttpContext.Session.GetInt32("UserId") != null)
			{
				var IDuzytkownika = (int)HttpContext.Session.GetInt32("UserId");
				var saldoData = _context.Saldo
							.Where(s => s.UserID == IDuzytkownika)
							.ToList();
				ViewData["SaldoData"] = saldoData;
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}
		public IActionResult ExChange()
		{
			if (HttpContext.Session.GetInt32("UserId") != null)
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
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}
		public IActionResult Login()
		{
			var error = TempData["error"] as string;
			ViewData["Error"] = error;
			return View();
		}
		public IActionResult Register()
		{
			var errorregister = TempData["Email"] as string;
			ViewData["ErrorReg"] = errorregister;
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

	}
}
